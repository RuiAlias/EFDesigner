﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;

using ParsingModels;

// ReSharper disable UseObjectOrCollectionInitializer
#pragma warning disable IDE0017 // Simplify object initialization

namespace EFCoreParser
{
   public class Parser
   {
      private readonly DbContext dbContext;

      private IModel model;

      public Parser(Assembly assembly, string dbContextTypeName = null)
      {
         Type contextType;

         if (dbContextTypeName != null)
            contextType = assembly.GetExportedTypes().FirstOrDefault(t => t.FullName == dbContextTypeName);
         else
         {
            List<Type> types = assembly.GetExportedTypes().Where(t => typeof(DbContext).IsAssignableFrom(t)).ToList();

            // ReSharper disable once UnthrowableException
            if (types.Count != 1)
               throw new AmbiguousMatchException("Found more than one class derived from DbContext");

            contextType = types[0];
         }

         Type optionsBuilderType = typeof(DbContextOptionsBuilder<>).MakeGenericType(contextType);
         DbContextOptionsBuilder optionsBuilder = Activator.CreateInstance(optionsBuilderType) as DbContextOptionsBuilder;

         Type optionsType = typeof(DbContextOptions<>).MakeGenericType(contextType);
         DbContextOptions options = optionsBuilder.UseInMemoryDatabase("Parser").Options;

         ConstructorInfo constructor = contextType.GetConstructor(new[] { optionsType });

         // ReSharper disable once UnthrowableException
         if (constructor == null)
            throw new MissingMethodException("Can't find appropriate constructor");

         dbContext = assembly.CreateInstance(contextType.FullName, true, BindingFlags.Default, null, new object[] { options }, null, null) as DbContext;
         model = dbContext.Model;
      }

      private static Multiplicity ConvertMultiplicity(RelationshipMultiplicity relationshipMultiplicity)
      {
         Multiplicity multiplicity = Multiplicity.ZeroOne;

         switch (relationshipMultiplicity)
         {
            case RelationshipMultiplicity.ZeroOrOne:
               multiplicity = Multiplicity.ZeroOne;

               break;

            case RelationshipMultiplicity.One:
               multiplicity = Multiplicity.One;

               break;

            case RelationshipMultiplicity.Many:
               multiplicity = Multiplicity.ZeroMany;

               break;
         }

         return multiplicity;
      }

      #region Associations

      private List<ModelUnidirectionalAssociation> GetUnidirectionalAssociations(IEntityType entityType)
      {
         List<ModelUnidirectionalAssociation> result = new List<ModelUnidirectionalAssociation>();

         foreach (INavigation navigationProperty in entityType.GetDeclaredNavigations().Where(n => n.FindInverse() == null))
         {
            ModelUnidirectionalAssociation association = new ModelUnidirectionalAssociation();

            association.SourceClassName = navigationProperty.DeclaringType.ClrType.Name;
            association.SourceClassNamespace = navigationProperty.DeclaringType.ClrType.Namespace;

            Type targetType = navigationProperty.GetTargetType().ClrType.Unwrap();
            association.TargetClassName = targetType.Name;
            association.TargetClassNamespace = targetType.Namespace;

            // the property in the source class (referencing the target class)
            association.TargetPropertyTypeName = navigationProperty.PropertyInfo.PropertyType.Unwrap().Name;
            association.TargetPropertyName = navigationProperty.Name;
            association.TargetMultiplicity = ConvertMultiplicity(navigationProperty.GetTargetMultiplicity());

            // the property in the target class (referencing the source class)
            association.SourceMultiplicity = ConvertMultiplicity(navigationProperty.GetSourceMultiplicity());

            // unfortunately, EFCore doesn't serialize documentation like EF6 did

            //association.TargetSummary = navigationProperty.ToEndMember.Documentation?.Summary;
            //association.TargetDescription = navigationProperty.ToEndMember.Documentation?.LongDescription;
            //association.SourceSummary = navigationProperty.FromEndMember.Documentation?.Summary;
            //association.SourceDescription = navigationProperty.FromEndMember.Documentation?.LongDescription;

            result.Add(association);
         }

         return result;
      }

      private List<ModelBidirectionalAssociation> GetBidirectionalAssociations(IEntityType entityType)
      {
         List<ModelBidirectionalAssociation> result = new List<ModelBidirectionalAssociation>();

         foreach (INavigation navigationProperty in entityType.GetDeclaredNavigations().Where(n => n.FindInverse() != null))
         {
            ModelBidirectionalAssociation association = new ModelBidirectionalAssociation();

            Type sourceType = navigationProperty.GetSourceType().ClrType.Unwrap();
            association.SourceClassName = sourceType.Name;
            association.SourceClassNamespace = sourceType.Namespace;
         
            Type targetType = navigationProperty.GetTargetType().ClrType.Unwrap();
            association.TargetClassName = targetType.Name;
            association.TargetClassNamespace = targetType.Namespace;

            INavigation inverse = navigationProperty.FindInverse();

            // the property in the source class (referencing the target class)
            association.TargetPropertyTypeName = navigationProperty.PropertyInfo.PropertyType.Unwrap().Name;
            association.TargetPropertyName = navigationProperty.Name;
            association.TargetMultiplicity = ConvertMultiplicity(navigationProperty.GetTargetMultiplicity());

            //association.TargetSummary = navigationProperty.ToEndMember.Documentation?.Summary;
            //association.TargetDescription = navigationProperty.ToEndMember.Documentation?.LongDescription;

            // the property in the target class (referencing the source class)
            association.SourcePropertyTypeName = inverse.PropertyInfo.PropertyType.Unwrap().Name;
            association.SourcePropertyName = inverse.Name;
            association.SourceMultiplicity = ConvertMultiplicity(navigationProperty.GetSourceMultiplicity());

            //association.SourceSummary = navigationProperty.FromEndMember.Documentation?.Summary;
            //association.SourceDescription = navigationProperty.FromEndMember.Documentation?.LongDescription;

            result.Add(association);
         }

         return result;
      }

      #endregion

      private static string GetCustomAttributes(Type type)
      {
         return type == null
                   ? string.Empty
                   : GetCustomAttributes(type.CustomAttributes);
      }

      private static string GetCustomAttributes(IEnumerable<CustomAttributeData> customAttributeData)
      {
         List<string> customAttributes = customAttributeData.Select(a => a.ToString()).ToList();
         customAttributes.Remove("[System.SerializableAttribute()]");
         customAttributes.Remove("[System.Runtime.CompilerServices.TypeForwardedFromAttribute(\"mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\")]");
         customAttributes.Remove("[__DynamicallyInvokableAttribute()]");
         customAttributes.Remove("[System.Reflection.DefaultMemberAttribute(\"Chars\")]");
         customAttributes.Remove("[System.Runtime.Versioning.NonVersionableAttribute()]");
         customAttributes.Remove("[System.FlagsAttribute()]");

         return string.Join("", customAttributes);
      }

      public string Process()
      {
         if (dbContext == null)

            // ReSharper disable once NotResolvedInText
            throw new ArgumentNullException("dbContext");

         model = dbContext.Model;

         ModelRoot modelRoot = ProcessRoot();

         List<ModelClass> modelClasses = model.GetEntityTypes()
                                              .Select(type => ProcessEntity(type, modelRoot))
                                              .Where(x => x != null)
                                              .ToList();

         modelRoot.Classes.AddRange(modelClasses);

         return JsonConvert.SerializeObject(modelRoot);
      }

      private ModelClass ProcessEntity(IEntityType entityType, ModelRoot modelRoot)
      {
         ModelClass result = new ModelClass();
         Type type = entityType.ClrType;

         result.Name = type.Name;
         result.Namespace = type.Namespace;
         result.IsAbstract = type.IsAbstract;

         result.BaseClass = type.BaseType.FullName == "System.Object"
                               ? null
                               : type.BaseType.Name;

         result.TableName = entityType.Relational().TableName;
         result.IsDependentType = entityType.IsOwned();
         result.CustomAttributes = GetCustomAttributes(type.CustomAttributes);

         result.CustomInterfaces = type.GetInterfaces().Any()
                                      ? string.Join(",", type.GetInterfaces().Select(t => t.FullName))
                                      : null;

         // TODO continue here
         result.Properties = entityType.GetDeclaredProperties().Where(p => !p.IsShadowProperty).Select(p => ProcessProperty(p, modelRoot)).Where(x => x != null).ToList();
         result.UnidirectionalAssociations = GetUnidirectionalAssociations(entityType);
         result.BidirectionalAssociations = GetBidirectionalAssociations(entityType);

         return result;
      }

      private void ProcessEnum(Type enumType, ModelRoot modelRoot)
      {
         string customAttributes = GetCustomAttributes(enumType);

         ModelEnum result = new ModelEnum();
         result.Name = enumType.Name;
         result.Namespace = enumType.Namespace;

         if (modelRoot.Enumerations.All(e => e.FullName != result.FullName))
         {
            result.IsFlags = enumType.GetTypeInfo().GetCustomAttribute(typeof(FlagsAttribute)) is FlagsAttribute ;
            result.ValueType = Enum.GetUnderlyingType(enumType).Name;

            result.CustomAttributes = customAttributes.Length > 2
                                         ? customAttributes
                                         : null;

            result.Values = Enum.GetNames(enumType)
                                .Select(name => new ModelEnumValue { Name = name, Value = Enum.Parse(enumType, name).ToString() })
                                .ToList();

            modelRoot.Enumerations.Add(result);
         }
      }

      private ModelProperty ProcessProperty(IProperty propertyData, ModelRoot modelRoot)
      {
         Type type = propertyData.ClrType;

         List<CustomAttributeData> attributes = type.CustomAttributes.ToList();

         ModelProperty result = new ModelProperty();

         if (type.IsEnum)
            ProcessEnum(propertyData.ClrType, modelRoot);

         // If it is NULLABLE, then get the underlying type. eg if "Nullable<int>" then this will return just "int"
         if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            type = type.GetGenericArguments()[0];

         result.TypeName = type.Name;
         result.Name = propertyData.Name;
         result.IsIdentity = propertyData.IsKey();
         result.Required = !propertyData.IsNullable;
         result.Indexed = propertyData.IsIndex();

         CustomAttributeData maxLengthAttribute = attributes.FirstOrDefault(a => a.AttributeType.Name == "MaxLength" || a.AttributeType.Name == "StringLength");
         result.MaxStringLength = (int?)maxLengthAttribute?.ConstructorArguments.First().Value ?? 0;

         if (maxLengthAttribute != null)
            attributes.Remove(maxLengthAttribute);

         CustomAttributeData minLengthAttribute = attributes.FirstOrDefault(a => a.AttributeType.Name == "MinLength");
         result.MinStringLength = (int?)minLengthAttribute?.ConstructorArguments.First().Value ?? 0;

         if (minLengthAttribute != null)
            attributes.Remove(minLengthAttribute);

         string customAttributes = GetCustomAttributes(attributes);

         result.CustomAttributes = customAttributes.Length > 2
                                      ? customAttributes
                                      : null;

         return result;
      }

      private ModelRoot ProcessRoot()
      {
         ModelRoot result = new ModelRoot();
         Type contextType = dbContext.GetType();

         result.EntityContainerName = contextType.Name;
         result.Namespace = contextType.Namespace;

         return result;
      }
   }
}