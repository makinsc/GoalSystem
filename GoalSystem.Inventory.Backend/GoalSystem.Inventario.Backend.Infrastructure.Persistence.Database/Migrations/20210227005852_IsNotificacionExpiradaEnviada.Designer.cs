﻿// <auto-generated />
using GoalSystem.Inventario.Backend.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace GoalSystem.Inventario.Backend.Infrastructure.Persistence.Database.Migrations
{
    [DbContext(typeof(InventarioContext))]
    [Migration("20210227005852_IsNotificacionExpiradaEnviada")]
    partial class IsNotificacionExpiradaEnviada
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("GoalSystem.Inventario.Backend.Persistence.Models.InventarioItem.InventarioItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("FechaCaducidad")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("FrechaCreacion")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("FrechaModificacion")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsNotificacionExpiradaEnviada")
                        .HasColumnType("bit");

                    b.Property<string>("Nombre")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Unidades")
                        .HasColumnType("int");

                    b.Property<string>("UsusarioCreacion")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UsusarioModificacion")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("Nombre")
                        .HasName("Ind_InventarioItem_Nombre_1793F16CC77D03694AF5E1610A43F634");

                    b.ToTable("InventarioItems");
                });
#pragma warning restore 612, 618
        }
    }
}
