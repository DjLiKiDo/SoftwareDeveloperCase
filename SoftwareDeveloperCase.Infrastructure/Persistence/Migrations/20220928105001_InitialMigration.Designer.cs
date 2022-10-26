﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SoftwareDeveloperCase.Infrastructure.Persistence;

#nullable disable

namespace SoftwareDeveloperCase.Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(SoftwareDeveloperCaseDbContext))]
    [Migration("20220928105001_InitialMigration")]
    partial class InitialMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("SoftwareDeveloperCase.Domain.Entities.Department", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("LastModifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Department");

                    b.HasData(
                        new
                        {
                            Id = new Guid("7e1ecedd-d9a5-4c81-8d2d-0ffd332f29c0"),
                            CreatedBy = "InitialSeed",
                            CreatedOn = new DateTime(2022, 9, 28, 10, 50, 1, 490, DateTimeKind.Utc).AddTicks(9189),
                            Name = "HR"
                        },
                        new
                        {
                            Id = new Guid("0eded24e-f07e-434c-af1d-b97d638564c9"),
                            CreatedBy = "InitialSeed",
                            CreatedOn = new DateTime(2022, 9, 28, 10, 50, 1, 490, DateTimeKind.Utc).AddTicks(9193),
                            Name = "IT"
                        });
                });

            modelBuilder.Entity("SoftwareDeveloperCase.Domain.Entities.Permission", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("LastModifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Permission");

                    b.HasData(
                        new
                        {
                            Id = new Guid("9a6ae1d8-0688-43d4-b1ce-2a13608fa68c"),
                            CreatedBy = "InitialSeed",
                            CreatedOn = new DateTime(2022, 9, 28, 10, 50, 1, 490, DateTimeKind.Utc).AddTicks(9302),
                            Name = "Read"
                        },
                        new
                        {
                            Id = new Guid("d2e69e18-e1a5-48c2-b5b5-eb888c13d46b"),
                            CreatedBy = "InitialSeed",
                            CreatedOn = new DateTime(2022, 9, 28, 10, 50, 1, 490, DateTimeKind.Utc).AddTicks(9306),
                            Name = "Add"
                        },
                        new
                        {
                            Id = new Guid("5162e7da-6b87-424a-a08d-fdd6e3c6b4b2"),
                            CreatedBy = "InitialSeed",
                            CreatedOn = new DateTime(2022, 9, 28, 10, 50, 1, 490, DateTimeKind.Utc).AddTicks(9307),
                            Name = "Update"
                        },
                        new
                        {
                            Id = new Guid("4a03f568-69e4-4548-85b6-8100cad15631"),
                            CreatedBy = "InitialSeed",
                            CreatedOn = new DateTime(2022, 9, 28, 10, 50, 1, 490, DateTimeKind.Utc).AddTicks(9309),
                            Name = "Delete"
                        });
                });

            modelBuilder.Entity("SoftwareDeveloperCase.Domain.Entities.Role", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("LastModifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("ParentRoleId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ParentRoleId");

                    b.ToTable("Role");

                    b.HasData(
                        new
                        {
                            Id = new Guid("2d7aa3b0-f221-4753-b77f-ff261858a13a"),
                            CreatedBy = "InitialSeed",
                            CreatedOn = new DateTime(2022, 9, 28, 10, 50, 1, 490, DateTimeKind.Utc).AddTicks(9332),
                            Name = "Employee"
                        },
                        new
                        {
                            Id = new Guid("9eca8d57-f7ca-4f8d-9c83-73b659225ae4"),
                            CreatedBy = "InitialSeed",
                            CreatedOn = new DateTime(2022, 9, 28, 10, 50, 1, 490, DateTimeKind.Utc).AddTicks(9335),
                            Name = "Manager",
                            ParentRoleId = new Guid("2d7aa3b0-f221-4753-b77f-ff261858a13a")
                        });
                });

            modelBuilder.Entity("SoftwareDeveloperCase.Domain.Entities.RolePermission", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("LastModifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("PermissionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("PermissionId");

                    b.HasIndex("RoleId");

                    b.ToTable("RolePermission");

                    b.HasData(
                        new
                        {
                            Id = new Guid("9e89d8f2-c8dd-474c-b7fa-267a9570488a"),
                            CreatedBy = "InitialSeed",
                            CreatedOn = new DateTime(2022, 9, 28, 10, 50, 1, 490, DateTimeKind.Utc).AddTicks(9351),
                            PermissionId = new Guid("9a6ae1d8-0688-43d4-b1ce-2a13608fa68c"),
                            RoleId = new Guid("2d7aa3b0-f221-4753-b77f-ff261858a13a")
                        },
                        new
                        {
                            Id = new Guid("00aeb653-9bfd-46bd-98e8-631c080a7cd3"),
                            CreatedBy = "InitialSeed",
                            CreatedOn = new DateTime(2022, 9, 28, 10, 50, 1, 490, DateTimeKind.Utc).AddTicks(9354),
                            PermissionId = new Guid("9a6ae1d8-0688-43d4-b1ce-2a13608fa68c"),
                            RoleId = new Guid("9eca8d57-f7ca-4f8d-9c83-73b659225ae4")
                        },
                        new
                        {
                            Id = new Guid("265e22e5-2252-432e-b6d2-e994740f6f08"),
                            CreatedBy = "InitialSeed",
                            CreatedOn = new DateTime(2022, 9, 28, 10, 50, 1, 490, DateTimeKind.Utc).AddTicks(9357),
                            PermissionId = new Guid("d2e69e18-e1a5-48c2-b5b5-eb888c13d46b"),
                            RoleId = new Guid("9eca8d57-f7ca-4f8d-9c83-73b659225ae4")
                        },
                        new
                        {
                            Id = new Guid("5b953aa8-6bde-4f47-b984-d3622a8e0550"),
                            CreatedBy = "InitialSeed",
                            CreatedOn = new DateTime(2022, 9, 28, 10, 50, 1, 490, DateTimeKind.Utc).AddTicks(9359),
                            PermissionId = new Guid("5162e7da-6b87-424a-a08d-fdd6e3c6b4b2"),
                            RoleId = new Guid("9eca8d57-f7ca-4f8d-9c83-73b659225ae4")
                        },
                        new
                        {
                            Id = new Guid("0754e6c4-4e12-42b1-845b-21ad1fc2f6b0"),
                            CreatedBy = "InitialSeed",
                            CreatedOn = new DateTime(2022, 9, 28, 10, 50, 1, 490, DateTimeKind.Utc).AddTicks(9361),
                            PermissionId = new Guid("4a03f568-69e4-4548-85b6-8100cad15631"),
                            RoleId = new Guid("9eca8d57-f7ca-4f8d-9c83-73b659225ae4")
                        });
                });

            modelBuilder.Entity("SoftwareDeveloperCase.Domain.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("DepartmentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("LastModifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("DepartmentId");

                    b.ToTable("User");

                    b.HasData(
                        new
                        {
                            Id = new Guid("2d7aa3b0-f221-4753-b77f-ff261858a13a"),
                            CreatedBy = "InitialSeed",
                            CreatedOn = new DateTime(2022, 9, 28, 10, 50, 1, 490, DateTimeKind.Utc).AddTicks(9380),
                            DepartmentId = new Guid("7e1ecedd-d9a5-4c81-8d2d-0ffd332f29c0"),
                            Email = "hremployee@sdc.com",
                            Name = "HR Employee",
                            Password = "sdc"
                        },
                        new
                        {
                            Id = new Guid("64a19c7d-a7a9-4481-a498-7df87f341da4"),
                            CreatedBy = "InitialSeed",
                            CreatedOn = new DateTime(2022, 9, 28, 10, 50, 1, 490, DateTimeKind.Utc).AddTicks(9383),
                            DepartmentId = new Guid("7e1ecedd-d9a5-4c81-8d2d-0ffd332f29c0"),
                            Email = "hrmanager@sdc.com",
                            Name = "HR Manager",
                            Password = "sdc"
                        },
                        new
                        {
                            Id = new Guid("29d6cf7d-2335-4329-91ad-4a7ec437d73c"),
                            CreatedBy = "InitialSeed",
                            CreatedOn = new DateTime(2022, 9, 28, 10, 50, 1, 490, DateTimeKind.Utc).AddTicks(9385),
                            DepartmentId = new Guid("0eded24e-f07e-434c-af1d-b97d638564c9"),
                            Email = "itemployee@sdc.com",
                            Name = "IT Employee",
                            Password = "sdc"
                        },
                        new
                        {
                            Id = new Guid("94651e82-5fc3-43d0-9c64-5a16ac517d43"),
                            CreatedBy = "InitialSeed",
                            CreatedOn = new DateTime(2022, 9, 28, 10, 50, 1, 490, DateTimeKind.Utc).AddTicks(9387),
                            DepartmentId = new Guid("0eded24e-f07e-434c-af1d-b97d638564c9"),
                            Email = "itmanager@sdc.com",
                            Name = "IT Manager",
                            Password = "sdc"
                        });
                });

            modelBuilder.Entity("SoftwareDeveloperCase.Domain.Entities.UserRole", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("LastModifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserId");

                    b.ToTable("UserRole");

                    b.HasData(
                        new
                        {
                            Id = new Guid("47175080-a8c0-4d4f-bf88-85cd5cbff45b"),
                            CreatedBy = "InitialSeed",
                            CreatedOn = new DateTime(2022, 9, 28, 10, 50, 1, 490, DateTimeKind.Utc).AddTicks(9404),
                            RoleId = new Guid("2d7aa3b0-f221-4753-b77f-ff261858a13a"),
                            UserId = new Guid("2d7aa3b0-f221-4753-b77f-ff261858a13a")
                        },
                        new
                        {
                            Id = new Guid("cf61c58c-e416-4b84-a195-dd7b77ef96dd"),
                            CreatedBy = "InitialSeed",
                            CreatedOn = new DateTime(2022, 9, 28, 10, 50, 1, 490, DateTimeKind.Utc).AddTicks(9407),
                            RoleId = new Guid("2d7aa3b0-f221-4753-b77f-ff261858a13a"),
                            UserId = new Guid("64a19c7d-a7a9-4481-a498-7df87f341da4")
                        },
                        new
                        {
                            Id = new Guid("8e49f3ba-11f1-4256-a195-91f9a8f3a41b"),
                            CreatedBy = "InitialSeed",
                            CreatedOn = new DateTime(2022, 9, 28, 10, 50, 1, 490, DateTimeKind.Utc).AddTicks(9410),
                            RoleId = new Guid("9eca8d57-f7ca-4f8d-9c83-73b659225ae4"),
                            UserId = new Guid("64a19c7d-a7a9-4481-a498-7df87f341da4")
                        },
                        new
                        {
                            Id = new Guid("5562714c-a186-439f-a0df-25321703cf7e"),
                            CreatedBy = "InitialSeed",
                            CreatedOn = new DateTime(2022, 9, 28, 10, 50, 1, 490, DateTimeKind.Utc).AddTicks(9412),
                            RoleId = new Guid("2d7aa3b0-f221-4753-b77f-ff261858a13a"),
                            UserId = new Guid("29d6cf7d-2335-4329-91ad-4a7ec437d73c")
                        },
                        new
                        {
                            Id = new Guid("4dc7a2ea-5e37-4038-8810-79fd9df7ac0a"),
                            CreatedBy = "InitialSeed",
                            CreatedOn = new DateTime(2022, 9, 28, 10, 50, 1, 490, DateTimeKind.Utc).AddTicks(9414),
                            RoleId = new Guid("2d7aa3b0-f221-4753-b77f-ff261858a13a"),
                            UserId = new Guid("94651e82-5fc3-43d0-9c64-5a16ac517d43")
                        },
                        new
                        {
                            Id = new Guid("07a59e4e-ead7-4597-bb36-96a06ee3a847"),
                            CreatedBy = "InitialSeed",
                            CreatedOn = new DateTime(2022, 9, 28, 10, 50, 1, 490, DateTimeKind.Utc).AddTicks(9417),
                            RoleId = new Guid("9eca8d57-f7ca-4f8d-9c83-73b659225ae4"),
                            UserId = new Guid("94651e82-5fc3-43d0-9c64-5a16ac517d43")
                        });
                });

            modelBuilder.Entity("SoftwareDeveloperCase.Domain.Entities.Role", b =>
                {
                    b.HasOne("SoftwareDeveloperCase.Domain.Entities.Role", "ParentRole")
                        .WithMany()
                        .HasForeignKey("ParentRoleId");

                    b.Navigation("ParentRole");
                });

            modelBuilder.Entity("SoftwareDeveloperCase.Domain.Entities.RolePermission", b =>
                {
                    b.HasOne("SoftwareDeveloperCase.Domain.Entities.Permission", "Permission")
                        .WithMany("RolePermissions")
                        .HasForeignKey("PermissionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SoftwareDeveloperCase.Domain.Entities.Role", "Role")
                        .WithMany("RolePermissions")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Permission");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("SoftwareDeveloperCase.Domain.Entities.User", b =>
                {
                    b.HasOne("SoftwareDeveloperCase.Domain.Entities.Department", "Department")
                        .WithMany("Users")
                        .HasForeignKey("DepartmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Department");
                });

            modelBuilder.Entity("SoftwareDeveloperCase.Domain.Entities.UserRole", b =>
                {
                    b.HasOne("SoftwareDeveloperCase.Domain.Entities.Role", "Role")
                        .WithMany("UserRoles")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SoftwareDeveloperCase.Domain.Entities.User", "User")
                        .WithMany("UserRoles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");

                    b.Navigation("User");
                });

            modelBuilder.Entity("SoftwareDeveloperCase.Domain.Entities.Department", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("SoftwareDeveloperCase.Domain.Entities.Permission", b =>
                {
                    b.Navigation("RolePermissions");
                });

            modelBuilder.Entity("SoftwareDeveloperCase.Domain.Entities.Role", b =>
                {
                    b.Navigation("RolePermissions");

                    b.Navigation("UserRoles");
                });

            modelBuilder.Entity("SoftwareDeveloperCase.Domain.Entities.User", b =>
                {
                    b.Navigation("UserRoles");
                });
#pragma warning restore 612, 618
        }
    }
}
