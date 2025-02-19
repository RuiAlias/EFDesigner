//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
//
//     Produced by Entity Framework Visual Editor
//     https://github.com/msawczyn/EFDesigner
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;

namespace Sandbox_EF6
{
   /// <inheritdoc/>
   public partial class EFModel : System.Data.Entity.DbContext
   {
      #region DbSets
      public virtual System.Data.Entity.DbSet<global::Sandbox_EF6.BaseClass> BaseClasses { get; set; }
      public virtual System.Data.Entity.DbSet<global::Sandbox_EF6.Detail> Details { get; set; }
      public virtual System.Data.Entity.DbSet<global::Sandbox_EF6.Master> Masters { get; set; }
      #endregion DbSets

      #region Constructors

      partial void CustomInit();

      /// <summary>
      /// Default connection string
      /// </summary>
      public static string ConnectionString { get; set; } = @"Data Source=.;Initial Catalog=Sandbox;Integrated Security=True";
      /// <inheritdoc />
      public EFModel() : base(ConnectionString)
      {
         Configuration.LazyLoadingEnabled = true;
         Configuration.ProxyCreationEnabled = true;
         System.Data.Entity.Database.SetInitializer<EFModel>(new EFModelDatabaseInitializer());
         CustomInit();
      }

      /// <inheritdoc />
      public EFModel(string connectionString) : base(connectionString)
      {
         Configuration.LazyLoadingEnabled = true;
         Configuration.ProxyCreationEnabled = true;
         System.Data.Entity.Database.SetInitializer<EFModel>(new EFModelDatabaseInitializer());
         CustomInit();
      }

      /// <inheritdoc />
      public EFModel(string connectionString, System.Data.Entity.Infrastructure.DbCompiledModel model) : base(connectionString, model)
      {
         Configuration.LazyLoadingEnabled = true;
         Configuration.ProxyCreationEnabled = true;
         System.Data.Entity.Database.SetInitializer<EFModel>(new EFModelDatabaseInitializer());
         CustomInit();
      }

      /// <inheritdoc />
      public EFModel(System.Data.Common.DbConnection existingConnection, bool contextOwnsConnection) : base(existingConnection, contextOwnsConnection)
      {
         Configuration.LazyLoadingEnabled = true;
         Configuration.ProxyCreationEnabled = true;
         System.Data.Entity.Database.SetInitializer<EFModel>(new EFModelDatabaseInitializer());
         CustomInit();
      }

      /// <inheritdoc />
      public EFModel(System.Data.Common.DbConnection existingConnection, System.Data.Entity.Infrastructure.DbCompiledModel model, bool contextOwnsConnection) : base(existingConnection, model, contextOwnsConnection)
      {
         Configuration.LazyLoadingEnabled = true;
         Configuration.ProxyCreationEnabled = true;
         System.Data.Entity.Database.SetInitializer<EFModel>(new EFModelDatabaseInitializer());
         CustomInit();
      }

      /// <inheritdoc />
      public EFModel(System.Data.Entity.Infrastructure.DbCompiledModel model) : base(model)
      {
         Configuration.LazyLoadingEnabled = true;
         Configuration.ProxyCreationEnabled = true;
         System.Data.Entity.Database.SetInitializer<EFModel>(new EFModelDatabaseInitializer());
         CustomInit();
      }

      /// <inheritdoc />
      public EFModel(System.Data.Entity.Core.Objects.ObjectContext objectContext, bool dbContextOwnsObjectContext) : base(objectContext, dbContextOwnsObjectContext)
      {
         Configuration.LazyLoadingEnabled = true;
         Configuration.ProxyCreationEnabled = true;
         System.Data.Entity.Database.SetInitializer<EFModel>(new EFModelDatabaseInitializer());
         CustomInit();
      }

      #endregion Constructors

      partial void OnModelCreatingImpl(System.Data.Entity.DbModelBuilder modelBuilder);
      partial void OnModelCreatedImpl(System.Data.Entity.DbModelBuilder modelBuilder);

      /// <inheritdoc />
      protected override void OnModelCreating(System.Data.Entity.DbModelBuilder modelBuilder)
      {
         base.OnModelCreating(modelBuilder);
         OnModelCreatingImpl(modelBuilder);

         modelBuilder.HasDefaultSchema("dbo");

         modelBuilder.Entity<global::Sandbox_EF6.BaseClass>()
                     .ToTable("BaseClasses")
                     .HasKey(t => t.Id);
         modelBuilder.Entity<global::Sandbox_EF6.BaseClass>()
                     .Property(t => t.Id)
                     .IsRequired()
                     .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

         modelBuilder.Entity<global::Sandbox_EF6.Detail>()
                     .ToTable("Details");
         modelBuilder.Entity<global::Sandbox_EF6.Detail>()
                     .HasMany(x => x.BaseClasses)
                     .WithRequired()
                     .Map(x => x.MapKey("Detail.BaseClasses_Id"));

         modelBuilder.Entity<global::Sandbox_EF6.Master>()
                     .ToTable("Masters");
         modelBuilder.Entity<global::Sandbox_EF6.Master>()
                     .HasMany(x => x.Details)
                     .WithOptional()
                     .Map(x => x.MapKey("Master.Details_Id"))
                     .WillCascadeOnDelete(true);

         OnModelCreatedImpl(modelBuilder);
      }
   }
}
