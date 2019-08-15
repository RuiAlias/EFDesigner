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

            modelBuilder.Entity("Sandbox_EFCore.PressRelease", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.HasKey("Id");

                    b.ToTable("PressReleases");
                });

            modelBuilder.Entity("Sandbox_EFCore.PressReleaseDetail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.HasKey("Id");

                    b.ToTable("PressReleaseDetails");
                });

            modelBuilder.Entity("Sandbox_EFCore.PressRelease_PressReleaseDetailHistory_x_PressReleaseDetail_PressReleases", b =>
                {
                    b.Property<int>("LHS_Id");

                    b.Property<int>("RHS_Id");

                    b.HasKey("LHS_Id", "RHS_Id");

                    b.HasIndex("RHS_Id");

                    b.ToTable("PressRelease_PressReleaseDetailHistory_x_PressReleaseDetail_PressReleases");
                });

            modelBuilder.Entity("Sandbox_EFCore.PressRelease_PressReleaseDetails_x_PressRelease", b =>
                {
                    b.Property<int>("LHS_Id");

                    b.Property<int>("RHS_Id");

                    b.HasKey("LHS_Id", "RHS_Id");

                    b.HasIndex("RHS_Id");

                    b.ToTable("PressRelease_PressReleaseDetails_x_PressRelease");
                });

            modelBuilder.Entity("Sandbox_EFCore.PressRelease_PressReleaseDetailHistory_x_PressReleaseDetail_PressReleases", b =>
                {
                    b.HasOne("Sandbox_EFCore.PressRelease", "LHS")
                        .WithMany()
                        .HasForeignKey("LHS_Id")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Sandbox_EFCore.PressReleaseDetail", "RHS")
                        .WithMany()
                        .HasForeignKey("RHS_Id")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Sandbox_EFCore.PressRelease_PressReleaseDetails_x_PressRelease", b =>
                {
                    b.HasOne("Sandbox_EFCore.PressRelease", "LHS")
                        .WithMany()
                        .HasForeignKey("LHS_Id")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Sandbox_EFCore.PressReleaseDetail", "RHS")
                        .WithMany()
                        .HasForeignKey("RHS_Id")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
