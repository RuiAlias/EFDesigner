﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sandbox_EFCore;

namespace Sandbox_EFCore.Migrations
{
    [DbContext(typeof(EFModel))]
    partial class EFModelModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("dbo")
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Sandbox_EFCore.Source", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Sources");
                });

            modelBuilder.Entity("Sandbox_EFCore.Source_TargetsBi_x_Target_Sources", b =>
                {
                    b.Property<int>("LHSId");

                    b.Property<int>("RHSId");

                    b.HasKey("LHSId", "RHSId");

                    b.HasIndex("RHSId");

                    b.ToTable("Source_TargetsBi_x_Target_Sources");
                });

            modelBuilder.Entity("Sandbox_EFCore.Source_TargetsUni_x_Target", b =>
                {
                    b.Property<int>("LHSId");

                    b.Property<int>("RHSId");

                    b.HasKey("LHSId", "RHSId");

                    b.HasIndex("RHSId");

                    b.ToTable("Source_TargetsUni_x_Target");
                });

            modelBuilder.Entity("Sandbox_EFCore.Target", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Targets");
                });

            modelBuilder.Entity("Sandbox_EFCore.Source_TargetsBi_x_Target_Sources", b =>
                {
                    b.HasOne("Sandbox_EFCore.Source", "LHS")
                        .WithMany("Source_TargetsBi_x_Target_Sources_List")
                        .HasForeignKey("LHSId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Sandbox_EFCore.Target", "RHS")
                        .WithMany("Source_TargetsBi_x_Target_Sources_List")
                        .HasForeignKey("RHSId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Sandbox_EFCore.Source_TargetsUni_x_Target", b =>
                {
                    b.HasOne("Sandbox_EFCore.Source", "LHS")
                        .WithMany("Source_TargetsUni_x_Target_List")
                        .HasForeignKey("LHSId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Sandbox_EFCore.Target", "RHS")
                        .WithMany()
                        .HasForeignKey("RHSId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
