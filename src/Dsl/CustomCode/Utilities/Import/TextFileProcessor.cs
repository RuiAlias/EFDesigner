﻿// 

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.Modeling;

using Sawczyn.EFDesigner.EFModel.Annotations;
using Sawczyn.EFDesigner.EFModel.Extensions;

using static System.Int32;

namespace Sawczyn.EFDesigner.EFModel
{
   public class TextFileProcessor: IFileProcessor
   {
      private List<string> KnownInterfaces;


      // ReSharper disable FieldCanBeMadeReadOnly.Local
#pragma warning disable IDE0044 // Add readonly modifier
      private List<string> KnownClasses;
      private List<string> KnownEnums;
#pragma warning restore IDE0044 // Add readonly modifier
      // ReSharper restore FieldCanBeMadeReadOnly.Local

      private readonly Store Store;

      public TextFileProcessor(Store store)
      {
         Store = store;

         KnownInterfaces = new List<string>();
         KnownEnums = new List<string>();
         KnownClasses = new List<string>();
      }

      private List<string> HarvestInterfaces(string filename)
      {
         if (!string.IsNullOrEmpty(filename))
         {
            string fileContents = File.ReadAllText(filename);
            SyntaxTree tree = CSharpSyntaxTree.ParseText(fileContents);

            if (tree.GetRoot() is CompilationUnitSyntax root)
            {
               return root.DescendantNodes()
                          .OfType<InterfaceDeclarationSyntax>()
                          .Select(decl => decl.Identifier.Text)
                          .ToList();
            }
         }

         return new List<string>();
      }

      private List<string> HarvestEnums(string filename)
      {
         if (!String.IsNullOrEmpty(filename))
         {
            string fileContents = File.ReadAllText(filename);
            SyntaxTree tree = CSharpSyntaxTree.ParseText(fileContents);

            if (tree.GetRoot() is CompilationUnitSyntax root)
            {
               return root.DescendantNodes()
                          .OfType<EnumDeclarationSyntax>()
                          .Select(decl => decl.Identifier.Text)
                          .ToList();
            }
         }

         return new List<string>();
      }

      private List<string> HarvestClasses(string filename)
      {
         if (!String.IsNullOrEmpty(filename))
         {
            string fileContents = File.ReadAllText(filename);
            SyntaxTree tree = CSharpSyntaxTree.ParseText(fileContents);

            if (tree.GetRoot() is CompilationUnitSyntax root)
            {
               return root.DescendantNodes()
                          .OfType<ClassDeclarationSyntax>()
                          .Where(decl => decl.BaseList == null ||
                                         decl.BaseList.Types.FirstOrDefault()?.ToString() != "DbContext")
                          .Select(decl => decl.Identifier.Text)
                          .ToList();
            }
         }

         return new List<string>();
      }

      public bool Process(string filename)
      {
         if (String.IsNullOrEmpty(filename))
            throw new ArgumentNullException(nameof(filename));

         try
         {
            // read the file
            string fileContents = File.ReadAllText(filename);

            // parse the contents
            SyntaxTree tree = CSharpSyntaxTree.ParseText(fileContents);

            if (tree.GetRoot() is CompilationUnitSyntax root)
            {
               List<ClassDeclarationSyntax> classDecls = root.DescendantNodes().OfType<ClassDeclarationSyntax>().Where(classDecl => classDecl.BaseList == null || classDecl.BaseList.Types.FirstOrDefault()?.ToString() != "DbContext").ToList();
               List<EnumDeclarationSyntax> enumDecls = root.DescendantNodes().OfType<EnumDeclarationSyntax>().ToList();

               if (!classDecls.Any() && !enumDecls.Any())
               {
                  WarningDisplay.Show($"Couldn't find any classes or enums to add to the model in {filename}");

                  return false;
               }

               // keep this order: enums, classes, class properties

               foreach (EnumDeclarationSyntax enumDecl in enumDecls)
                  ProcessEnum(enumDecl);

               List<ModelClass> processedClasses = new List<ModelClass>();
               foreach (ClassDeclarationSyntax classDecl in classDecls)
                  processedClasses.Add(ProcessClass(classDecl));

               // process last so all classes and enums are already in the model
               foreach (ClassDeclarationSyntax classDecl in classDecls)
                  ProcessProperties(classDecl);

               // now that all the properties are in, go through the classes again and ensure identities are present based on convention
               // ReSharper disable once LoopCanBePartlyConvertedToQuery
               foreach (ModelClass modelClass in processedClasses.Where(c => !c.AllIdentityAttributes.Any()))
               {
                  // no identity attribute. Only look in current class for attributes that could be identity by convention
                  List<ModelAttribute> identitiesByConvention = modelClass.Attributes.Where(a => a.Name == "Id" || a.Name == $"{modelClass.Name}Id").ToList();

                  // if both 'Id' and '[ClassName]Id' are present, don't do anything since we don't know which to make the identity
                  if (identitiesByConvention.Count == 1)
                  {
                     using (Transaction transaction = Store.TransactionManager.BeginTransaction("Add identity"))
                     {
                        identitiesByConvention[0].IsIdentity = true;
                        transaction.Commit();
                     }
                  }
               }
            }
         }
         catch
         {
            ErrorDisplay.Show("Error interpreting " + filename);

            return false;
         }

         return true;
      }

      private void ProcessProperties([NotNull] ClassDeclarationSyntax classDecl)
      {
         if (classDecl == null)
            throw new ArgumentNullException(nameof(classDecl));

         Transaction tx = Store.TransactionManager.CurrentTransaction == null
                             ? Store.TransactionManager.BeginTransaction()
                             : null;

         try
         {
            string className = classDecl.Identifier.Text;
            ModelRoot modelRoot = Store.ModelRoot();
            ModelClass modelClass = Store.Get<ModelClass>().FirstOrDefault(c => c.Name == className);
            modelClass.Attributes.Clear();

            foreach (PropertyDeclarationSyntax propertyDecl in classDecl.DescendantNodes().OfType<PropertyDeclarationSyntax>())
            {
               // if the property has a fat arrow expression as its direct descendent, it's a readonly calculated property
               // TODO: we should handle this
               // but for this release, ignore it
               if (propertyDecl.ChildNodes().OfType<ArrowExpressionClauseSyntax>().Any())
                  continue;

               AccessorDeclarationSyntax getAccessor = (AccessorDeclarationSyntax)propertyDecl.DescendantNodes().FirstOrDefault(node => node.IsKind(SyntaxKind.GetAccessorDeclaration));
               AccessorDeclarationSyntax setAccessor = (AccessorDeclarationSyntax)propertyDecl.DescendantNodes().FirstOrDefault(node => node.IsKind(SyntaxKind.SetAccessorDeclaration));

               // if there's no getAccessor, why are we bothering?
               if (getAccessor == null) continue;

               string propertyName = propertyDecl.Identifier.ToString();
               string propertyType = propertyDecl.Type.ToString();
               ModelClass target = modelRoot.Classes.FirstOrDefault(t => t.Name == propertyType);

               // is the property type a generic?
               // assume it's a list
               // TODO: this really isn't a good assumption. Fix later
               if (propertyDecl.ChildNodes().OfType<GenericNameSyntax>().Any())
               {
                  GenericNameSyntax genericDecl = propertyDecl.ChildNodes().OfType<GenericNameSyntax>().FirstOrDefault();
                  List<string> contentTypes = genericDecl.DescendantNodes().OfType<IdentifierNameSyntax>().Select(i => i.Identifier.ToString()).ToList();

                  // there can only be one generic argument
                  if (contentTypes.Count != 1)
                  {
                     WarningDisplay.Show($"Found {className}.{propertyName}, but its type ({genericDecl.Identifier}<{String.Join(", ", contentTypes)}>) isn't anything expected. Ignoring...");

                     continue;
                  }

                  propertyType = contentTypes[0];
                  target = modelRoot.Classes.FirstOrDefault(t => t.Name == propertyType);

                  if (target == null)
                  {
                     target = new ModelClass(Store, new PropertyAssignment(ModelClass.NameDomainPropertyId, propertyType));
                     modelRoot.Classes.Add(target);
                  }

                  ProcessAssociation(modelClass, target, propertyDecl, true);

                  continue;
               }


               // is the property type an existing ModelClass?
               if (target != null)
               {
                  ProcessAssociation(modelClass, target, propertyDecl);

                  continue;
               }

               bool propertyShowsNullable = propertyDecl.DescendantNodes().OfType<NullableTypeSyntax>().Any();

               // is the property type something we don't know about?
               if (!modelRoot.IsValidCLRType(propertyType))
               {
                  // might be an enum. If so, we'll handle it like a CLR type
                  // if it's nullable, it's definitely an enum, but if we don't know about it, it could be an enum or a class
                  if (!KnownEnums.Contains(propertyType) && !propertyShowsNullable)
                  {
                     // assume it's a class and create the class
                     target = new ModelClass(Store, new PropertyAssignment(ModelClass.NameDomainPropertyId, propertyType));
                     modelRoot.Classes.Add(target);

                     ProcessAssociation(modelClass, target, propertyDecl);

                     continue;
                  }
               }

               // if we're here, it's just a property (CLR or enum)
               try
               {
                  // ReSharper disable once UseObjectOrCollectionInitializer
                  ModelAttribute modelAttribute = new ModelAttribute(Store, new PropertyAssignment(ModelAttribute.NameDomainPropertyId, propertyName))
                  {
                     Type = ModelAttribute.ToCLRType(propertyDecl.Type.ToString()).Trim('?'),
                     Required = propertyDecl.HasAttribute("RequiredAttribute") || !propertyShowsNullable,
                     Indexed = propertyDecl.HasAttribute("IndexedAttribute"),
                     IsIdentity = propertyDecl.HasAttribute("KeyAttribute"),
                     Virtual = propertyDecl.DescendantTokens().Any(t => t.IsKind(SyntaxKind.VirtualKeyword))
                  };

                  if (modelAttribute.Type.ToLower() == "string")
                  {
                     AttributeSyntax maxLengthAttribute = propertyDecl.GetAttribute("MaxLengthAttribute");
                     AttributeArgumentSyntax maxLength = maxLengthAttribute?.GetAttributeArguments()?.FirstOrDefault();

                     if (maxLength != null)
                        modelAttribute.MaxLength = TryParse(maxLength.Expression.ToString(), out int _max) ? _max : -1;

                     AttributeSyntax minLengthAttribute = propertyDecl.GetAttribute("MinLengthAttribute");
                     AttributeArgumentSyntax minLength = minLengthAttribute?.GetAttributeArguments()?.FirstOrDefault();

                     if (minLength != null)
                        modelAttribute.MinLength = TryParse(minLength.Expression.ToString(), out int _min) ? _min : 0;
                  }
                  else
                  {
                     modelAttribute.MaxLength = -1;
                     modelAttribute.MinLength = 0;
                  }

                  // if no setAccessor, it's a calculated readonly property
                  if (setAccessor == null)
                  {
                     modelAttribute.Persistent = false;
                     modelAttribute.ReadOnly = true;
                  }

                  modelAttribute.AutoProperty = !getAccessor.DescendantNodes().Any(node => node.IsKind(SyntaxKind.Block)) && !setAccessor.DescendantNodes().Any(node => node.IsKind(SyntaxKind.Block));

                  modelAttribute.SetterVisibility = setAccessor.Modifiers.Any(m => m.ToString() == "protected")
                                                       ? SetterAccessModifier.Protected
                                                       : setAccessor.Modifiers.Any(m => m.ToString() == "internal")
                                                          ? SetterAccessModifier.Internal
                                                          : SetterAccessModifier.Public;

                  XMLDocumentation xmlDocumentation = new XMLDocumentation(propertyDecl);
                  modelAttribute.Summary = xmlDocumentation.Summary;
                  modelAttribute.Description = xmlDocumentation.Description;
                  modelClass.Attributes.Add(modelAttribute);
               }
               catch
               {
                  WarningDisplay.Show($"Could not parse '{className}.{propertyDecl.Identifier}'.");
               }
            }
         }
         catch
         {
            tx = null;

            throw;
         }
         finally
         {
            tx?.Commit();
         }
      }

      private void ProcessAssociation([NotNull] ModelClass source, [NotNull] ModelClass target, [NotNull] PropertyDeclarationSyntax propertyDecl, bool toMany = false)
      {
         if (source == null)
            throw new ArgumentNullException(nameof(source));

         if (target == null)
            throw new ArgumentNullException(nameof(target));

         if (propertyDecl == null)
            throw new ArgumentNullException(nameof(propertyDecl));

         Transaction tx = Store.TransactionManager.CurrentTransaction == null
                             ? Store.TransactionManager.BeginTransaction()
                             : null;

         try
         {
            string propertyName = propertyDecl.Identifier.ToString();

            // since we don't have enough information from the code, we'll create unidirectional associations
            // cardinality 1 on the source end, 0..1 or 0..* on the target, depending on the parameter

            XMLDocumentation xmlDocumentation = new XMLDocumentation(propertyDecl);

            // if the association doesn't yet exist, create it
            if (!Store.ElementDirectory
                      .AllElements
                      .OfType<UnidirectionalAssociation>()
                      .Any(a => a.Source == source &&
                                a.Target == target &&
                                a.TargetPropertyName == propertyName))
            {
               // if there's a unidirectional going the other direction, we'll whack that one and make a bidirectional
               // otherwise, proceed as planned
               UnidirectionalAssociation compliment = Store.ElementDirectory
                                                           .AllElements
                                                           .OfType<UnidirectionalAssociation>()
                                                           .FirstOrDefault(a => a.Source == target &&
                                                                                a.Target == source);

               if (compliment == null)
               {
                  UnidirectionalAssociation _ =
                     new UnidirectionalAssociation(Store,
                                                   new[]
                                                   {
                                                      new RoleAssignment(UnidirectionalAssociation.UnidirectionalSourceDomainRoleId, source),
                                                      new RoleAssignment(UnidirectionalAssociation.UnidirectionalTargetDomainRoleId, target)
                                                   },
                                                   new[]
                                                   {
                                                      new PropertyAssignment(Association.SourceMultiplicityDomainPropertyId, Multiplicity.One),

                                                      new PropertyAssignment(Association.TargetMultiplicityDomainPropertyId, toMany ? Multiplicity.ZeroMany : Multiplicity.ZeroOne),
                                                      new PropertyAssignment(Association.TargetPropertyNameDomainPropertyId, propertyName),
                                                      new PropertyAssignment(Association.TargetSummaryDomainPropertyId, xmlDocumentation.Summary),
                                                      new PropertyAssignment(Association.TargetDescriptionDomainPropertyId, xmlDocumentation.Description)
                                                   });
               }
               else
               {
                  compliment.Delete();

                  BidirectionalAssociation _ =
                     new BidirectionalAssociation(Store,
                                                  new[]
                                                  {
                                                     new RoleAssignment(BidirectionalAssociation.BidirectionalSourceDomainRoleId, source),
                                                     new RoleAssignment(BidirectionalAssociation.BidirectionalTargetDomainRoleId, target)
                                                  },
                                                  new[]
                                                  {
                                                     new PropertyAssignment(Association.SourceMultiplicityDomainPropertyId, compliment.TargetMultiplicity),
                                                     new PropertyAssignment(BidirectionalAssociation.SourcePropertyNameDomainPropertyId, compliment.TargetPropertyName),
                                                     new PropertyAssignment(BidirectionalAssociation.SourceSummaryDomainPropertyId, compliment.TargetSummary),
                                                     new PropertyAssignment(BidirectionalAssociation.SourceDescriptionDomainPropertyId, compliment.TargetDescription),

                                                     new PropertyAssignment(Association.TargetMultiplicityDomainPropertyId, toMany ? Multiplicity.ZeroMany : Multiplicity.ZeroOne),
                                                     new PropertyAssignment(Association.TargetPropertyNameDomainPropertyId, propertyName),
                                                     new PropertyAssignment(Association.TargetSummaryDomainPropertyId, xmlDocumentation.Summary),
                                                     new PropertyAssignment(Association.TargetDescriptionDomainPropertyId, xmlDocumentation.Description)
                                                  });
               }
            }
         }
         catch
         {
            tx.Rollback();
            tx = null;

            throw;
         }
         finally
         {
            tx?.Commit();
         }
      }

      private void ProcessEnum([NotNull] EnumDeclarationSyntax enumDecl, NamespaceDeclarationSyntax namespaceDecl = null)
      {
         if (enumDecl == null)
            throw new ArgumentNullException(nameof(enumDecl));

         ModelRoot modelRoot = Store.ModelRoot();
         string enumName = enumDecl.Identifier.Text;

         if (namespaceDecl == null && enumDecl.Parent is NamespaceDeclarationSyntax enumDeclParent)
            namespaceDecl = enumDeclParent;

         string namespaceName = namespaceDecl?.Name?.ToString() ?? modelRoot.Namespace;

         if (Store.Get<ModelClass>().Any(c => c.Name == enumName) || Store.Get<ModelEnum>().Any(c => c.Name == enumName))
         {
            ErrorDisplay.Show($"'{enumName}' already exists in model.");

            // ReSharper disable once ExpressionIsAlwaysNull
            return;
         }

         Transaction tx = Store.TransactionManager.CurrentTransaction == null
                             ? Store.TransactionManager.BeginTransaction()
                             : null;

         try
         {
            ModelEnum result = new ModelEnum(Store,
                                             new PropertyAssignment(ModelEnum.NameDomainPropertyId, enumName))
            {
               Namespace = namespaceName,
               IsFlags = enumDecl.HasAttribute("Flags")
            };

            SimpleBaseTypeSyntax baseTypeSyntax = enumDecl.DescendantNodes().OfType<SimpleBaseTypeSyntax>().FirstOrDefault();

            if (baseTypeSyntax != null)
            {
               switch (baseTypeSyntax.Type.ToString())
               {
                  case "Int16":
                  case "short":
                     result.ValueType = EnumValueType.Int16;

                     break;
                  case "Int32":
                  case "int":
                     result.ValueType = EnumValueType.Int32;

                     break;
                  case "Int64":
                  case "long":
                     result.ValueType = EnumValueType.Int64;

                     break;
                  default:
                     WarningDisplay.Show($"Could not resolve value type for '{enumName}'. The enum will default to an Int32 value type.");

                     break;
               }
            }

            XMLDocumentation xmlDocumentation;

            foreach (EnumMemberDeclarationSyntax enumValueDecl in enumDecl.DescendantNodes().OfType<EnumMemberDeclarationSyntax>())
            {
               ModelEnumValue enumValue = new ModelEnumValue(Store, new PropertyAssignment(ModelEnumValue.NameDomainPropertyId, enumValueDecl.Identifier.ToString()));
               EqualsValueClauseSyntax valueDecl = enumValueDecl.DescendantNodes().OfType<EqualsValueClauseSyntax>().FirstOrDefault();

               if (valueDecl != null)
                  enumValue.Value = valueDecl.Value.ToString();

               xmlDocumentation = new XMLDocumentation(enumValueDecl);
               enumValue.Summary = xmlDocumentation.Summary;
               enumValue.Description = xmlDocumentation.Description;

               result.Values.Add(enumValue);
            }

            xmlDocumentation = new XMLDocumentation(enumDecl);
            result.Summary = xmlDocumentation.Summary;
            result.Description = xmlDocumentation.Description;

            modelRoot.Enums.Add(result);
         }
         catch
         {
            tx = null;

            throw;
         }
         finally
         {
            tx?.Commit();
         }
      }

      private ModelClass ProcessClass([NotNull] ClassDeclarationSyntax classDecl, NamespaceDeclarationSyntax namespaceDecl = null)
      {
         ModelClass result = null;

         if (classDecl == null)
            throw new ArgumentNullException(nameof(classDecl));

         ModelRoot modelRoot = Store.ModelRoot();
         string className = classDecl.Identifier.Text;

         if (namespaceDecl == null && classDecl.Parent is NamespaceDeclarationSyntax classDeclParent)
            namespaceDecl = classDeclParent;

         if (Store.Get<ModelEnum>().Any(c => c.Name == className))
         {
            ErrorDisplay.Show($"'{className}' already exists in model as an Enum.");

            // ReSharper disable once ExpressionIsAlwaysNull
            return result;
         }

         if (classDecl.TypeParameterList != null)
         {
            ErrorDisplay.Show($"Can't add generic class '{className}'.");

            // ReSharper disable once ExpressionIsAlwaysNull
            return result;
         }

         Transaction tx = Store.TransactionManager.CurrentTransaction == null
                             ? Store.TransactionManager.BeginTransaction()
                             : null;

         List<string> customInterfaces = new List<string>();
         try
         {
            ModelClass superClass = null;
            result = Store.Get<ModelClass>().FirstOrDefault(c => c.Name == className);

            // Base classes and interfaces
            // Check these first. If we need to add new models, we want the base class already in the store
            if (classDecl.BaseList != null)
            {
               foreach (BaseTypeSyntax type in classDecl.BaseList.Types)
               {
                  string baseName = type.ToString();

                  // Do we know this is an interface?
                  if (KnownInterfaces.Contains(baseName) || superClass != null || result?.Superclass != null)
                  {
                     customInterfaces.Add(baseName);
                     if (!KnownInterfaces.Contains(baseName))
                        KnownInterfaces.Add(baseName);

                     continue;
                  }

                  // is it inheritance or an interface?
                  superClass = modelRoot.Classes.FirstOrDefault(c => c.Name == baseName);

                  // if it's not in the model, we just don't know. Ask the user
                  if (superClass == null && (KnownClasses.Contains(baseName) || QuestionDisplay.Show($"For class {className}, is {baseName} the base class?") == true))
                  {
                     superClass = new ModelClass(Store, new PropertyAssignment(ModelClass.NameDomainPropertyId, baseName));
                     modelRoot.Classes.Add(superClass);
                  }
                  else
                  {
                     customInterfaces.Add(baseName);
                     KnownInterfaces.Add(baseName);
                  }
               }
            }

            if (result == null)
            {
               result = new ModelClass(Store, new PropertyAssignment(ModelClass.NameDomainPropertyId, className))
               {
                  Namespace = namespaceDecl?.Name?.ToString() ?? modelRoot.Namespace,
                  IsAbstract = classDecl.DescendantNodes().Any(n => n.Kind() == SyntaxKind.AbstractKeyword)
               };

               modelRoot.Classes.Add(result);
            }

            if (superClass != null)
               result.Superclass = superClass;

            if (result.CustomInterfaces != null)
            {
               customInterfaces.AddRange(result.CustomInterfaces
                                               .Split(',')
                                               .Where(i => !String.IsNullOrEmpty(i))
                                               .Select(i => i.Trim()));
            }

            if (customInterfaces.Contains("INotifyPropertyChanged"))
            {
               result.ImplementNotify = true;
               customInterfaces.Remove("INotifyPropertyChanged");
            }

            if (result.Superclass != null && customInterfaces.Contains(result.Superclass.Name))
               customInterfaces.Remove(result.Superclass.Name);

            result.CustomInterfaces = customInterfaces.Any()
                                         ? String.Join(",", customInterfaces.Distinct())
                                         : null;


            XMLDocumentation xmlDocumentation = new XMLDocumentation(classDecl);
            result.Summary = xmlDocumentation.Summary;
            result.Description = xmlDocumentation.Description;
         }
         catch
         {
            tx = null;

            throw;
         }
         finally
         {
            tx?.Commit();
         }

         return result;
      }

      public void LoadCache(List<string> textFileList)
      {
         foreach (string textFile in textFileList)
            LoadCache(textFile);

         KnownInterfaces = KnownInterfaces.Distinct().ToList();
      }

      public void LoadCache(string textFile)
      {
         KnownInterfaces.AddRange(HarvestInterfaces(textFile).Union(new List<string>(new[] {"INotifyPropertyChanged"})));
         KnownEnums.AddRange(HarvestEnums(textFile));
         KnownClasses.AddRange(HarvestClasses(textFile));
      }
   }
}