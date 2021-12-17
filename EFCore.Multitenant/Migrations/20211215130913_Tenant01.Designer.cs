﻿// <auto-generated />
using EFCore.Multitenant.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace EFCore.Multitenant.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20211215130913_Tenant01")]
    partial class Tenant01
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("EFCore.Multitenant.Domain.Person", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TenantId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("People");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Person 1",
                            TenantId = "tenant-1"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Person 2",
                            TenantId = "tenant-2"
                        },
                        new
                        {
                            Id = 3,
                            Name = "Person 3",
                            TenantId = "tenant-2"
                        });
                });

            modelBuilder.Entity("EFCore.Multitenant.Domain.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TenantId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Products");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Description = "Description 1",
                            TenantId = "tenant-1"
                        },
                        new
                        {
                            Id = 2,
                            Description = "Description 2",
                            TenantId = "tenant-2"
                        },
                        new
                        {
                            Id = 3,
                            Description = "Description 3",
                            TenantId = "tenant-2"
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
