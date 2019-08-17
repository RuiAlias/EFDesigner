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
using Microsoft.EntityFrameworkCore;

namespace Sandbox_EFCore
{
   public partial class EFModel : Microsoft.EntityFrameworkCore.DbContext
   {
      #region DbSets
      public virtual Microsoft.EntityFrameworkCore.DbSet<global::Sandbox_EFCore.Source> Sources { get; set; }
      public virtual Microsoft.EntityFrameworkCore.DbSet<global::Sandbox_EFCore.Target> Targets { get; set; }
      #endregion DbSets

      /// <summary>
      /// Default connection string
      /// </summary>
      public static string ConnectionString { get; set; } = @"Data Source=.\sqlexpress;Initial Catalog=N2N;Integrated Security=True";

      /// <inheritdoc />
      public EFModel(DbContextOptions<EFModel> options) : base(options)
      {
      }

      partial void CustomInit(DbContextOptionsBuilder optionsBuilder);

      /// <inheritdoc />
      protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
      {
         CustomInit(optionsBuilder);
      }

      partial void OnModelCreatingImpl(ModelBuilder modelBuilder);
      partial void OnModelCreatedImpl(ModelBuilder modelBuilder);

      /// <inheritdoc />
      protected override void OnModelCreating(ModelBuilder modelBuilder)
      {
         base.OnModelCreating(modelBuilder);
         OnModelCreatingImpl(modelBuilder);

         modelBuilder.HasDefaultSchema("dbo");

         modelBuilder.Entity<global::Sandbox_EFCore.Source>().ToTable("Sources").HasKey(t => t.Id);
         modelBuilder.Entity<global::Sandbox_EFCore.Source>().Property(t => t.Id).IsRequired().ValueGeneratedOnAdd();

         #region Many-to-Many associations containing global::Sandbox_EFCore.Source

         modelBuilder.Entity<global::Sandbox_EFCore.Source>().Ignore(x => x.TargetsUni);
         modelBuilder.Entity<global::Sandbox_EFCore.Source>().HasMany(x => x.Source_TargetsUni_x_Target_List).WithOne(x => x.LHS).HasForeignKey(x => x.LHSId).IsRequired();

         modelBuilder.Entity<global::Sandbox_EFCore.Source_TargetsUni_x_Target>().ToTable("Source_TargetsUni_x_Target").HasKey(t => new { t.LHSId, t.RHSId });

         modelBuilder.Entity<global::Sandbox_EFCore.Source>().Ignore(x => x.TargetsBi);
         modelBuilder.Entity<global::Sandbox_EFCore.Source>().HasMany(x => x.Source_TargetsBi_x_Target_Sources_List).WithOne(x => x.LHS).HasForeignKey(x => x.LHSId).IsRequired();
         modelBuilder.Entity<global::Sandbox_EFCore.Target>().Ignore(x => x.Sources);
         modelBuilder.Entity<global::Sandbox_EFCore.Target>().HasMany(x => x.Source_TargetsBi_x_Target_Sources_List).WithOne(x => x.RHS).HasForeignKey(x => x.RHSId).IsRequired();

         modelBuilder.Entity<global::Sandbox_EFCore.Source_TargetsBi_x_Target_Sources>().ToTable("Source_TargetsBi_x_Target_Sources").HasKey(t => new { t.LHSId, t.RHSId });

         #endregion Many-to-Many associations

         modelBuilder.Entity<global::Sandbox_EFCore.Target>().ToTable("Targets").HasKey(t => t.Id);
         modelBuilder.Entity<global::Sandbox_EFCore.Target>().Property(t => t.Id).IsRequired().ValueGeneratedOnAdd();

         OnModelCreatedImpl(modelBuilder);
      }
   }
}
