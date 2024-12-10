﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Persistence;

#nullable disable

namespace Persistence.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20241209213030_TableDeleteActionsRefactored")]
    partial class TableDeleteActionsRefactored
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.20")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Domain.Tables.Disease", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<List<string>>("Symptoms")
                        .IsRequired()
                        .HasColumnType("text[]");

                    b.Property<List<string>>("Treatments")
                        .IsRequired()
                        .HasColumnType("text[]");

                    b.HasKey("Id");

                    b.ToTable("tblDiseases", (string)null);
                });

            modelBuilder.Entity("Domain.Tables.Plant", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("DiseaseId")
                        .HasColumnType("integer");

                    b.Property<int>("HumidityLevel")
                        .HasColumnType("integer");

                    b.Property<int>("IrrigationAmount")
                        .HasColumnType("integer");

                    b.Property<int>("LightNeed")
                        .HasColumnType("integer");

                    b.Property<int>("ModTemp")
                        .HasColumnType("integer");

                    b.Property<string>("ScientificName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("SoilType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("WateringFrequency")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("DiseaseId");

                    b.ToTable("tblPlants", (string)null);
                });

            modelBuilder.Entity("Domain.Tables.SensorData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("Moisture")
                        .HasColumnType("integer");

                    b.Property<int?>("PlantId")
                        .HasColumnType("integer");

                    b.Property<int>("Temperature")
                        .HasColumnType("integer");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int?>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("PlantId");

                    b.HasIndex("UserId");

                    b.ToTable("tblSensorData", (string)null);
                });

            modelBuilder.Entity("Domain.Tables.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("tblUsers", (string)null);
                });

            modelBuilder.Entity("Domain.Tables.Plant", b =>
                {
                    b.HasOne("Domain.Tables.Disease", "Diseases")
                        .WithMany()
                        .HasForeignKey("DiseaseId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.Navigation("Diseases");
                });

            modelBuilder.Entity("Domain.Tables.SensorData", b =>
                {
                    b.HasOne("Domain.Tables.Plant", "Plant")
                        .WithMany()
                        .HasForeignKey("PlantId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("Domain.Tables.User", null)
                        .WithMany("SensorData")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.Navigation("Plant");
                });

            modelBuilder.Entity("Domain.Tables.User", b =>
                {
                    b.Navigation("SensorData");
                });
#pragma warning restore 612, 618
        }
    }
}