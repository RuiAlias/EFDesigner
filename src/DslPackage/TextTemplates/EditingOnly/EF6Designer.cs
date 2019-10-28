﻿using System;
using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace Sawczyn.EFDesigner.EFModel.DslPackage.TextTemplates.EditingOnly
{
   [SuppressMessage("ReSharper", "UnusedMember.Local")]
   [SuppressMessage("ReSharper", "UnusedMember.Global")]
   partial class EditOnly
   {
      string CreateForeignKeyColumnSegmentEF6(Association association, List<string> foreignKeyColumns)
      {
         // foreign key definitions always go in the table representing the Dependent end of the association
         // if there is no dependent end (i.e., many-to-many), there are no foreign keys
         string nameBase;

         if (association.SourceRole == EndpointRole.Dependent)
            nameBase = association.TargetPropertyName;
         else if (association.TargetRole == EndpointRole.Dependent)
         {
            nameBase = association is BidirectionalAssociation b
                          ? b.SourcePropertyName
                          : $"{association.Source.Name}.{association.TargetPropertyName}";
         }
         else
            return null;

         string columnName = $"{nameBase}_Id";

         if (foreignKeyColumns.Contains(columnName))
         {
            int index = 0;

            do
            {
               columnName = $"{nameBase}{++index}_Id";
            } while (foreignKeyColumns.Contains(columnName));
         }

         foreignKeyColumns.Add(columnName);

         return $@"Map(x => x.MapKey(""{columnName}""))";
      }

      void GenerateEF6(Manager manager, ModelRoot modelRoot)
      {
         // Entities

         foreach (ModelClass modelClass in modelRoot.Classes)
         {
            string dir = modelClass.IsDependentType
                            ? modelRoot.StructOutputDirectory
                            : modelRoot.EntityOutputDirectory;

            if (!string.IsNullOrEmpty(modelClass.OutputDirectory))
               dir = modelClass.OutputDirectory;

            manager.StartNewFile(Path.Combine(dir, $"{modelClass.Name}.{modelRoot.FileNameMarker}.cs"));
            WriteClass(modelClass);
         }

         // Enums

         foreach (ModelEnum modelEnum in modelRoot.Enums)
         {
            string dir = !string.IsNullOrEmpty(modelEnum.OutputDirectory)
                            ? modelEnum.OutputDirectory
                            : modelRoot.EnumOutputDirectory;

            manager.StartNewFile(Path.Combine(dir, $"{modelEnum.Name}.{modelRoot.FileNameMarker}.cs"));
            WriteEnum(modelEnum);
         }

         // Context

         if (modelRoot.DatabaseInitializerType != DatabaseInitializerKind.None)
         {
            manager.StartNewFile(Path.Combine(modelRoot.ContextOutputDirectory, $"{modelRoot.EntityContainerName}DatabaseInitializer.{modelRoot.FileNameMarker}.cs"));
            WriteDatabaseInitializerEF6(modelRoot);
         }

         manager.StartNewFile(Path.Combine(modelRoot.ContextOutputDirectory, $"{modelRoot.EntityContainerName}DbMigrationConfiguration.{modelRoot.FileNameMarker}.cs"));
         WriteMigrationConfigurationEF6(modelRoot);

         manager.StartNewFile(Path.Combine(modelRoot.ContextOutputDirectory, $"{modelRoot.EntityContainerName}.{modelRoot.FileNameMarker}.cs"));
         WriteDbContextEF6(modelRoot);
      }

      List<string> GetAdditionalUsingStatementsEF6(ModelRoot modelRoot)
      {
         List<string> result = new List<string>();
         List<string> attributeTypes = modelRoot.Classes.SelectMany(c => c.Attributes).Select(a => a.Type).Distinct().ToList();

         if (attributeTypes.Any(t => t.IndexOf("Geometry", StringComparison.Ordinal) > -1 || t.IndexOf("Geography", StringComparison.Ordinal) > -1))
            result.Add("using System.Data.Entity.Spatial;");

         return result;
      }

      void WriteDatabaseInitializerEF6(ModelRoot modelRoot)
      {
         Output("using System.Data.Entity;");
         NL();

         BeginNamespace(modelRoot.Namespace);

         Output("/// <inheritdoc/>");

         Output(modelRoot.DatabaseInitializerType == DatabaseInitializerKind.MigrateDatabaseToLatestVersion
                   ? $"public partial class {modelRoot.EntityContainerName}DatabaseInitializer : MigrateDatabaseToLatestVersion<{modelRoot.EntityContainerName}, {modelRoot.EntityContainerName}DbMigrationConfiguration>"
                   : $"public partial class {modelRoot.EntityContainerName}DatabaseInitializer : {modelRoot.DatabaseInitializerType}<{modelRoot.EntityContainerName}>");

         Output("{");
         Output("}");
         EndNamespace(modelRoot.Namespace);
      }

      void WriteDbContextEF6(ModelRoot modelRoot)
      {
         Output("using System;");
         Output("using System.Collections.Generic;");
         Output("using System.Linq;");
         Output("using System.ComponentModel.DataAnnotations.Schema;");
         Output("using System.Data.Entity;");
         Output("using System.Data.Entity.Infrastructure.Annotations;");
         NL();

         BeginNamespace(modelRoot.Namespace);

         if (!string.IsNullOrEmpty(modelRoot.Summary))
         {
            Output("/// <summary>");
            WriteCommentBody(modelRoot.Summary);
            Output("/// </summary>");

            if (!string.IsNullOrEmpty(modelRoot.Description))
            {
               Output("/// <remarks>");
               WriteCommentBody(modelRoot.Description);
               Output("/// </remarks>");
            }
         }
         else
            Output("/// <inheritdoc/>");

         Output($"{modelRoot.EntityContainerAccess.ToString().ToLower()} partial class {modelRoot.EntityContainerName} : System.Data.Entity.DbContext");
         Output("{");

         PluralizationService pluralizationService = ModelRoot.PluralizationService;

         /***********************************************************************/
         // generate DBSets
         /***********************************************************************/

         ModelClass[] classesWithTables = null;

         switch (modelRoot.InheritanceStrategy)
         {
            case CodeStrategy.TablePerType:
               classesWithTables = modelRoot.Classes.Where(mc => !mc.IsDependentType).OrderBy(x => x.Name).ToArray();

               break;

            case CodeStrategy.TablePerConcreteType:
               classesWithTables = modelRoot.Classes.Where(mc => !mc.IsDependentType && !mc.IsAbstract).OrderBy(x => x.Name).ToArray();

               break;

            case CodeStrategy.TablePerHierarchy:
               classesWithTables = modelRoot.Classes.Where(mc => !mc.IsDependentType && mc.Superclass == null).OrderBy(x => x.Name).ToArray();

               break;
         }

         if (classesWithTables?.Any() == true)
         {
            Output("#region DbSets");

            foreach (ModelClass modelClass in modelRoot.Classes.Where(x => !x.IsDependentType).OrderBy(x => x.Name))
            {
               string dbSetName;

               if (!string.IsNullOrEmpty(modelClass.DbSetName))
                  dbSetName = modelClass.DbSetName;
               else
               {
                  dbSetName = pluralizationService?.IsSingular(modelClass.Name) == true
                                 ? pluralizationService.Pluralize(modelClass.Name)
                                 : modelClass.Name;
               }

               if (!string.IsNullOrEmpty(modelClass.Summary))
               {
                  NL();
                  Output("/// <summary>");
                  WriteCommentBody($"Repository for {modelClass.FullName} - {modelClass.Summary}");
                  Output("/// </summary>");
               }

               Output($"{modelRoot.DbSetAccess.ToString().ToLower()} virtual System.Data.Entity.DbSet<{modelClass.FullName}> {dbSetName} {{ get; set; }}");
            }

            Output("#endregion DbSets");
            NL();
         }

         Output("#region Constructors");
         NL();
         Output("partial void CustomInit();");
         NL();

         /***********************************************************************/
         // constructors
         /***********************************************************************/

         if (!string.IsNullOrEmpty(modelRoot.ConnectionString) || !string.IsNullOrEmpty(modelRoot.ConnectionStringName))
         {
            string connectionString = string.IsNullOrEmpty(modelRoot.ConnectionString)
                                         ? $"Name={modelRoot.ConnectionStringName}"
                                         : modelRoot.ConnectionString;

            Output("/// <summary>");
            Output("/// Default connection string");
            Output("/// </summary>");
            Output($"public static string ConnectionString {{ get; set; }} = @\"{connectionString}\";");

            Output("/// <inheritdoc />");
            Output($"public {modelRoot.EntityContainerName}() : base(ConnectionString)");
            Output("{");
            Output($"Configuration.LazyLoadingEnabled = {modelRoot.LazyLoadingEnabled.ToString().ToLower()};");
            Output($"Configuration.ProxyCreationEnabled = {modelRoot.ProxyGenerationEnabled.ToString().ToLower()};");

            Output(modelRoot.DatabaseInitializerType == DatabaseInitializerKind.None
                      ? $"System.Data.Entity.Database.SetInitializer<{modelRoot.EntityContainerName}>(null);"
                      : $"System.Data.Entity.Database.SetInitializer<{modelRoot.EntityContainerName}>(new {modelRoot.EntityContainerName}DatabaseInitializer());");

            Output("CustomInit();");
            Output("}");
            NL();
         }
         else
         {
            Output($"#warning Default constructor not generated for {modelRoot.EntityContainerName} since no default connection string was specified in the model");
            NL();
         }

         Output("/// <inheritdoc />");
         Output($"public {modelRoot.EntityContainerName}(string connectionString) : base(connectionString)");
         Output("{");
         Output($"Configuration.LazyLoadingEnabled = {modelRoot.LazyLoadingEnabled.ToString().ToLower()};");
         Output($"Configuration.ProxyCreationEnabled = {modelRoot.ProxyGenerationEnabled.ToString().ToLower()};");

         Output(modelRoot.DatabaseInitializerType == DatabaseInitializerKind.None
                   ? $"System.Data.Entity.Database.SetInitializer<{modelRoot.EntityContainerName}>(null);"
                   : $"System.Data.Entity.Database.SetInitializer<{modelRoot.EntityContainerName}>(new {modelRoot.EntityContainerName}DatabaseInitializer());");

         Output("CustomInit();");
         Output("}");
         NL();

         Output("/// <inheritdoc />");
         Output($"public {modelRoot.EntityContainerName}(string connectionString, System.Data.Entity.Infrastructure.DbCompiledModel model) : base(connectionString, model)");
         Output("{");
         Output($"Configuration.LazyLoadingEnabled = {modelRoot.LazyLoadingEnabled.ToString().ToLower()};");
         Output($"Configuration.ProxyCreationEnabled = {modelRoot.ProxyGenerationEnabled.ToString().ToLower()};");

         Output(modelRoot.DatabaseInitializerType == DatabaseInitializerKind.None
                   ? $"System.Data.Entity.Database.SetInitializer<{modelRoot.EntityContainerName}>(null);"
                   : $"System.Data.Entity.Database.SetInitializer<{modelRoot.EntityContainerName}>(new {modelRoot.EntityContainerName}DatabaseInitializer());");

         Output("CustomInit();");
         Output("}");
         NL();

         Output("/// <inheritdoc />");
         Output($"public {modelRoot.EntityContainerName}(System.Data.Common.DbConnection existingConnection, bool contextOwnsConnection) : base(existingConnection, contextOwnsConnection)");
         Output("{");
         Output($"Configuration.LazyLoadingEnabled = {modelRoot.LazyLoadingEnabled.ToString().ToLower()};");
         Output($"Configuration.ProxyCreationEnabled = {modelRoot.ProxyGenerationEnabled.ToString().ToLower()};");

         Output(modelRoot.DatabaseInitializerType == DatabaseInitializerKind.None
                   ? $"System.Data.Entity.Database.SetInitializer<{modelRoot.EntityContainerName}>(null);"
                   : $"System.Data.Entity.Database.SetInitializer<{modelRoot.EntityContainerName}>(new {modelRoot.EntityContainerName}DatabaseInitializer());");

         Output("CustomInit();");
         Output("}");
         NL();

         Output("/// <inheritdoc />");
         Output($"public {modelRoot.EntityContainerName}(System.Data.Common.DbConnection existingConnection, System.Data.Entity.Infrastructure.DbCompiledModel model, bool contextOwnsConnection) : base(existingConnection, model, contextOwnsConnection)");
         Output("{");
         Output($"Configuration.LazyLoadingEnabled = {modelRoot.LazyLoadingEnabled.ToString().ToLower()};");
         Output($"Configuration.ProxyCreationEnabled = {modelRoot.ProxyGenerationEnabled.ToString().ToLower()};");

         Output(modelRoot.DatabaseInitializerType == DatabaseInitializerKind.None
                   ? $"System.Data.Entity.Database.SetInitializer<{modelRoot.EntityContainerName}>(null);"
                   : $"System.Data.Entity.Database.SetInitializer<{modelRoot.EntityContainerName}>(new {modelRoot.EntityContainerName}DatabaseInitializer());");

         Output("CustomInit();");
         Output("}");
         NL();

         Output("/// <inheritdoc />");
         Output($"public {modelRoot.EntityContainerName}(System.Data.Entity.Infrastructure.DbCompiledModel model) : base(model)");
         Output("{");
         Output($"Configuration.LazyLoadingEnabled = {modelRoot.LazyLoadingEnabled.ToString().ToLower()};");
         Output($"Configuration.ProxyCreationEnabled = {modelRoot.ProxyGenerationEnabled.ToString().ToLower()};");

         Output(modelRoot.DatabaseInitializerType == DatabaseInitializerKind.None
                   ? $"System.Data.Entity.Database.SetInitializer<{modelRoot.EntityContainerName}>(null);"
                   : $"System.Data.Entity.Database.SetInitializer<{modelRoot.EntityContainerName}>(new {modelRoot.EntityContainerName}DatabaseInitializer());");

         Output("CustomInit();");
         Output("}");
         NL();

         Output("/// <inheritdoc />");
         Output($"public {modelRoot.EntityContainerName}(System.Data.Entity.Core.Objects.ObjectContext objectContext, bool dbContextOwnsObjectContext) : base(objectContext, dbContextOwnsObjectContext)");
         Output("{");
         Output($"Configuration.LazyLoadingEnabled = {modelRoot.LazyLoadingEnabled.ToString().ToLower()};");
         Output($"Configuration.ProxyCreationEnabled = {modelRoot.ProxyGenerationEnabled.ToString().ToLower()};");

         Output(modelRoot.DatabaseInitializerType == DatabaseInitializerKind.None
                   ? $"System.Data.Entity.Database.SetInitializer<{modelRoot.EntityContainerName}>(null);"
                   : $"System.Data.Entity.Database.SetInitializer<{modelRoot.EntityContainerName}>(new {modelRoot.EntityContainerName}DatabaseInitializer());");

         Output("CustomInit();");
         Output("}");
         NL();
         Output("#endregion Constructors");
         NL();

         /***********************************************************************/
         // OnModelCreating 
         /***********************************************************************/
         Output("partial void OnModelCreatingImpl(System.Data.Entity.DbModelBuilder modelBuilder);");
         Output("partial void OnModelCreatedImpl(System.Data.Entity.DbModelBuilder modelBuilder);");
         NL();

         Output("/// <inheritdoc />");
         Output("protected override void OnModelCreating(System.Data.Entity.DbModelBuilder modelBuilder)");
         Output("{");
         Output("base.OnModelCreating(modelBuilder);");
         Output("OnModelCreatingImpl(modelBuilder);");
         NL();

         Output($"modelBuilder.HasDefaultSchema(\"{modelRoot.DatabaseSchema}\");");

         List<string> segments = new List<string>();

         List<Association> visited = new List<Association>();
         List<string> foreignKeyColumns = new List<string>();

         foreach (ModelClass modelClass in modelRoot.Classes.OrderBy(x => x.Name))
         {
            segments.Clear();
            foreignKeyColumns.Clear();
            NL();

            // class level
            bool isDependent = modelClass.IsDependentType;
            segments.Add($"modelBuilder.{(isDependent ? "ComplexType" : "Entity")}<{modelClass.FullName}>()");

            // note: this must come before the 'ToTable' call or there's a runtime error
            if (modelRoot.InheritanceStrategy == CodeStrategy.TablePerConcreteType && modelClass.Superclass != null)
               segments.Add("Map(x => x.MapInheritedProperties())");

            if (classesWithTables.Contains(modelClass))
            {
               segments.Add(modelClass.DatabaseSchema == modelClass.ModelRoot.DatabaseSchema
                               ? $"ToTable(\"{modelClass.TableName}\")"
                               : $"ToTable(\"{modelClass.TableName}\", \"{modelClass.DatabaseSchema}\")");

               // primary key code segments must be output last, since HasKey returns a different type
               List<ModelAttribute> identityAttributes = modelClass.IdentityAttributes.ToList();

               if (identityAttributes.Count == 1)
                  segments.Add($"HasKey(t => t.{identityAttributes[0].Name})");
               else if (identityAttributes.Count > 1)
                  segments.Add($"HasKey(t => new {{ t.{string.Join(", t.", identityAttributes.Select(ia => ia.Name))} }})");
            }

            foreach (ModelAttribute transient in modelClass.Attributes.Where(x => !x.Persistent))
               segments.Add($"Ignore(t => t.{transient.Name})");

            if (segments.Count > 1)
            {
               if (modelRoot.ChopMethodChains)
                  OutputChopped(segments);
               else
                  Output(string.Join(".", segments) + ";");
            }

            if (modelClass.IsDependentType)
               continue;

            // indexed properties
            foreach (ModelAttribute indexed in modelClass.Attributes.Where(x => x.Indexed && !x.IsIdentity))
            {
               segments.Clear();

               segments.Add(indexed.AutoProperty
                               ? $"modelBuilder.Entity<{modelClass.FullName}>().HasIndex(t => t.{indexed.Name})"
                               : $"modelBuilder.Entity<{modelClass.FullName}>().HasIndex(\"_{indexed.Name}\")");

               if (indexed.IndexedUnique)
                  segments.Add("IsUnique()");

               if (segments.Count > 1)
               {
                  if (modelRoot.ChopMethodChains)
                     OutputChopped(segments);
                  else
                     Output(string.Join(".", segments) + ";");
               }
            }

            // attribute level
            foreach (ModelAttribute modelAttribute in modelClass.Attributes.Where(x => x.Persistent && !SpatialTypes.Contains(x.Type)))
            {
               segments.Clear();

               if (modelAttribute.MaxLength > 0)
                  segments.Add($"HasMaxLength({modelAttribute.MaxLength})");

               if (modelAttribute.Required)
                  segments.Add("IsRequired()");

               if (modelAttribute.ColumnName != modelAttribute.Name && !string.IsNullOrEmpty(modelAttribute.ColumnName))
                  segments.Add($"HasColumnName(\"{modelAttribute.ColumnName}\")");

               if (!string.IsNullOrEmpty(modelAttribute.ColumnType) && modelAttribute.ColumnType.ToLowerInvariant() != "default")
                  segments.Add($"HasColumnType(\"{modelAttribute.ColumnType}\")");

               if (modelAttribute.Indexed && !modelAttribute.IsIdentity)
                  segments.Add("HasColumnAnnotation(\"Index\", new IndexAnnotation(new IndexAttribute()))");

               if (modelAttribute.IsConcurrencyToken)
                  segments.Add("IsRowVersion()");

               if (modelAttribute.IsIdentity)
               {
                  segments.Add(modelAttribute.IdentityType == IdentityType.AutoGenerated
                                  ? "HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)"
                                  : "HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)");
               }

               if (segments.Any())
               {
                  segments.Insert(0, $"modelBuilder.{(isDependent ? "ComplexType" : "Entity")}<{modelClass.FullName}>()");
                  segments.Insert(1, $"Property(t => t.{modelAttribute.Name})");

                  if (modelRoot.ChopMethodChains)
                     OutputChopped(segments);
                  else
                     Output(string.Join(".", segments) + ";");
               }
            }

            if (!isDependent)
            {
               // Navigation endpoints are distingished as Source and Target. They are also distinguished as Principal
               // and Dependent. How do these map?
               // In the case of one-to-one or zero-to-one-to-zero-to-one, it's model dependent and the user has to tell us
               // In all other cases, we can tell by the cardinalities of the associations
               // What matters is the Principal and Dependent classifications, so we look at those. 
               // Source and Target are accidents of where the user started drawing the association.

               // navigation properties
               // ReSharper disable once LoopCanBePartlyConvertedToQuery
               foreach (UnidirectionalAssociation association in Association.GetLinksToTargets(modelClass)
                                                                            .OfType<UnidirectionalAssociation>()
                                                                            .Where(x => x.Persistent && !x.Target.IsDependentType))
               {
                  if (visited.Contains(association))
                     continue;

                  visited.Add(association);

                  segments.Clear();
                  segments.Add($"modelBuilder.Entity<{modelClass.FullName}>()");

                  switch (association.TargetMultiplicity) // realized by property on source
                  {
                     case Multiplicity.ZeroMany:
                        segments.Add($"HasMany(x => x.{association.TargetPropertyName})");

                        break;

                     case Multiplicity.One:
                        segments.Add($"HasRequired(x => x.{association.TargetPropertyName})");

                        break;

                     case Multiplicity.ZeroOne:
                        segments.Add($"HasOptional(x => x.{association.TargetPropertyName})");

                        break;

                     //case Sawczyn.EFDesigner.EFModel.Multiplicity.OneMany:
                     //   segments.Add($"HasMany(x => x.{association.TargetPropertyName})");
                     //   break;
                  }

                  switch (association.SourceMultiplicity) // realized by property on target, but no property on target
                  {
                     case Multiplicity.ZeroMany:
                        segments.Add("WithMany()");

                        if (association.TargetMultiplicity == Multiplicity.ZeroMany)
                        {
                           if (modelClass == association.Source)
                           {
                              segments.Add("Map(x => { "
                                         + $@"x.ToTable(""{association.Source.Name}_x_{association.TargetPropertyName}""); "
                                         + $@"x.MapLeftKey(""{association.Source.Name}_{association.Source.AllAttributes.FirstOrDefault(a => a.IsIdentity)?.Name}""); "
                                         + $@"x.MapRightKey(""{association.Target.Name}_{association.Target.AllAttributes.FirstOrDefault(a => a.IsIdentity)?.Name}""); "
                                         + "})");
                           }
                           else
                           {
                              segments.Add("Map(x => { "
                                         + $@"x.ToTable(""{association.Source.Name}_x_{association.TargetPropertyName}""); "
                                         + $@"x.MapRightKey(""{association.Source.Name}_{association.Source.AllAttributes.FirstOrDefault(a => a.IsIdentity)?.Name}""); "
                                         + $@"x.MapLeftKey(""{association.Target.Name}_{association.Target.AllAttributes.FirstOrDefault(a => a.IsIdentity)?.Name}""); "
                                         + "})");
                           }
                        }

                        break;

                     case Multiplicity.One:
                        if (association.TargetMultiplicity == Multiplicity.One)
                        {
                           segments.Add(association.TargetRole == EndpointRole.Dependent
                                           ? "WithRequiredDependent()"
                                           : "WithRequiredPrincipal()");
                        }
                        else
                           segments.Add("WithRequired()");

                        break;

                     case Multiplicity.ZeroOne:
                        if (association.TargetMultiplicity == Multiplicity.ZeroOne)
                        {
                           segments.Add(association.TargetRole == EndpointRole.Dependent
                                           ? "WithOptionalDependent()"
                                           : "WithOptionalPrincipal()");
                        }
                        else
                           segments.Add("WithOptional()");

                        break;

                     //case Sawczyn.EFDesigner.EFModel.Multiplicity.OneMany:
                     //   segments.Add("HasMany()");
                     //   break;
                  }

                  string foreignKeySegment = CreateForeignKeyColumnSegmentEF6(association, foreignKeyColumns);

                  if (foreignKeySegment != null)
                     segments.Add(foreignKeySegment);

                  // Certain associations cascade delete automatically. Also, the user may ask for it.
                  // We only generate a cascade delete call if the user asks for it. 
                  if ((association.TargetDeleteAction != DeleteAction.Default && association.TargetRole == EndpointRole.Principal)
                   || (association.SourceDeleteAction != DeleteAction.Default && association.SourceRole == EndpointRole.Principal))
                  {
                     string willCascadeOnDelete = association.TargetDeleteAction != DeleteAction.Default && association.TargetRole == EndpointRole.Principal
                                                     ? (association.TargetDeleteAction == DeleteAction.Cascade).ToString().ToLowerInvariant()
                                                     : (association.SourceDeleteAction == DeleteAction.Cascade).ToString().ToLowerInvariant();

                     segments.Add($"WillCascadeOnDelete({willCascadeOnDelete})");
                  }

                  if (modelRoot.ChopMethodChains)
                     OutputChopped(segments);
                  else
                     Output(string.Join(".", segments) + ";");
               }

               // ReSharper disable once LoopCanBePartlyConvertedToQuery
               foreach (BidirectionalAssociation association in Association.GetLinksToSources(modelClass)
                                                                           .OfType<BidirectionalAssociation>()
                                                                           .Where(x => x.Persistent))
               {
                  if (visited.Contains(association))
                     continue;

                  visited.Add(association);

                  segments.Clear();
                  segments.Add($"modelBuilder.Entity<{modelClass.FullName}>()");

                  switch (association.SourceMultiplicity) // realized by property on target
                  {
                     case Multiplicity.ZeroMany:
                        segments.Add($"HasMany(x => x.{association.SourcePropertyName})");

                        break;

                     case Multiplicity.One:
                        segments.Add($"HasRequired(x => x.{association.SourcePropertyName})");

                        break;

                     case Multiplicity.ZeroOne:
                        segments.Add($"HasOptional(x => x.{association.SourcePropertyName})");

                        break;

                     //one or more constraint not supported in EF. TODO: make this possible ... later
                     //case Sawczyn.EFDesigner.EFModel.Multiplicity.OneMany:
                     //   segments.Add($"HasMany(x => x.{association.SourcePropertyName})");
                     //   break;
                  }

                  switch (association.TargetMultiplicity) // realized by property on source
                  {
                     case Multiplicity.ZeroMany:
                        segments.Add($"WithMany(x => x.{association.TargetPropertyName})");

                        if (association.SourceMultiplicity == Multiplicity.ZeroMany)
                        {
                           if (modelClass == association.Source)
                           {
                              segments.Add("Map(x => { "
                                         + $@"x.ToTable(""{association.SourcePropertyName}_x_{association.TargetPropertyName}""); "
                                         + $@"x.MapLeftKey(""{association.Source.Name}_{association.Source.AllAttributes.FirstOrDefault(a => a.IsIdentity)?.Name}""); "
                                         + $@"x.MapRightKey(""{association.Target.Name}_{association.Target.AllAttributes.FirstOrDefault(a => a.IsIdentity)?.Name}""); "
                                         + "})");
                           }
                           else
                           {
                              segments.Add("Map(x => { "
                                         + $@"x.ToTable(""{association.SourcePropertyName}_x_{association.TargetPropertyName}""); "
                                         + $@"x.MapRightKey(""{association.Source.Name}_{association.Source.AllAttributes.FirstOrDefault(a => a.IsIdentity)?.Name}""); "
                                         + $@"x.MapLeftKey(""{association.Target.Name}_{association.Target.AllAttributes.FirstOrDefault(a => a.IsIdentity)?.Name}""); "
                                         + "})");
                           }
                        }

                        break;

                     case Multiplicity.One:
                        if (association.SourceMultiplicity == Multiplicity.One)
                        {
                           segments.Add(association.SourceRole == EndpointRole.Dependent
                                           ? $"WithRequiredDependent(x => x.{association.TargetPropertyName})"
                                           : $"WithRequiredPrincipal(x => x.{association.TargetPropertyName})");
                        }
                        else
                           segments.Add($"WithRequired(x => x.{association.TargetPropertyName})");

                        break;

                     case Multiplicity.ZeroOne:
                        if (association.SourceMultiplicity == Multiplicity.ZeroOne)
                        {
                           segments.Add(association.SourceRole == EndpointRole.Dependent
                                           ? $"WithOptionalDependent(x => x.{association.TargetPropertyName})"
                                           : $"WithOptionalPrincipal(x => x.{association.TargetPropertyName})");
                        }
                        else
                           segments.Add($"WithOptional(x => x.{association.TargetPropertyName})");

                        break;

                     //one or more constraint not supported in EF. TODO: make this possible ... later
                     //case Sawczyn.EFDesigner.EFModel.Multiplicity.OneMany:
                     //   segments.Add($"HasMany(x => x.{association.TargetPropertyName})");
                     //   break;
                  }

                  string foreignKeySegment = CreateForeignKeyColumnSegmentEF6(association, foreignKeyColumns);

                  if (foreignKeySegment != null)
                     segments.Add(foreignKeySegment);

                  if ((association.TargetDeleteAction != DeleteAction.Default && association.TargetRole == EndpointRole.Principal)
                   || (association.SourceDeleteAction != DeleteAction.Default && association.SourceRole == EndpointRole.Principal))
                  {
                     string willCascadeOnDelete = association.TargetDeleteAction != DeleteAction.Default && association.TargetRole == EndpointRole.Principal
                                                     ? (association.TargetDeleteAction == DeleteAction.Cascade).ToString().ToLowerInvariant()
                                                     : (association.SourceDeleteAction == DeleteAction.Cascade).ToString().ToLowerInvariant();

                     segments.Add($"WillCascadeOnDelete({willCascadeOnDelete})");
                  }

                  if (modelRoot.ChopMethodChains)
                     OutputChopped(segments);
                  else
                     Output(string.Join(".", segments) + ";");
               }
            }
         }

         NL();

         Output("OnModelCreatedImpl(modelBuilder);");
         Output("}");

         Output("}");

         EndNamespace(modelRoot.Namespace);
      }

      void WriteMigrationConfigurationEF6(ModelRoot modelRoot)
      {
         Output("using System.Data.Entity.Migrations;");
         NL();

         BeginNamespace(modelRoot.Namespace);
         Output("/// <inheritdoc/>");
         Output($"public sealed partial class {modelRoot.EntityContainerName}DbMigrationConfiguration : DbMigrationsConfiguration<{modelRoot.EntityContainerName}>");

         Output("{");
         Output("partial void Init();");
         NL();

         Output("/// <inheritdoc/>");
         Output($"public {modelRoot.EntityContainerName}DbMigrationConfiguration()");
         Output("{");
         Output($"AutomaticMigrationsEnabled = {modelRoot.AutomaticMigrationsEnabled.ToString().ToLower()};");
         Output("AutomaticMigrationDataLossAllowed = false;");
         Output("Init();");
         Output("}");

         Output("}");
         EndNamespace(modelRoot.Namespace);
      }
   }
}