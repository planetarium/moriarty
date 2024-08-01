﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Moriarty.Web.Data;

#nullable disable

namespace Moriarty.Web.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20240730054511_Add Campaign.Clues")]
    partial class AddCampaignClues
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.6");

            modelBuilder.Entity("CampaignSuspect", b =>
                {
                    b.Property<Guid>("CampaignId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("CharacterId")
                        .HasColumnType("TEXT");

                    b.HasKey("CampaignId", "CharacterId");

                    b.HasIndex("CharacterId");

                    b.ToTable("CampaignSuspect");
                });

            modelBuilder.Entity("Moriarty.Web.Data.Models.Campaign", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Method")
                        .HasColumnType("TEXT");

                    b.Property<string>("Motive")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("OffenderId")
                        .HasColumnType("TEXT");

                    b.Property<string>("OpenAIFileId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Plot")
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("VictimId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("OffenderId");

                    b.HasIndex("VictimId");

                    b.ToTable("Campaigns");
                });

            modelBuilder.Entity("Moriarty.Web.Data.Models.Character", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<int>("Age")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<byte[]>("ProfilePicture")
                        .IsRequired()
                        .HasColumnType("BLOB");

                    b.HasKey("Id");

                    b.ToTable("Characters");
                });

            modelBuilder.Entity("Moriarty.Web.Data.Models.Clue", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("CampaignId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("CampaignId");

                    b.ToTable("Clue");
                });

            modelBuilder.Entity("CampaignSuspect", b =>
                {
                    b.HasOne("Moriarty.Web.Data.Models.Campaign", null)
                        .WithMany()
                        .HasForeignKey("CampaignId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_CampaignSuspect_Campaign_CampaignId");

                    b.HasOne("Moriarty.Web.Data.Models.Character", null)
                        .WithMany()
                        .HasForeignKey("CharacterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_CampaignSuspect_Character_CharacterId");
                });

            modelBuilder.Entity("Moriarty.Web.Data.Models.Campaign", b =>
                {
                    b.HasOne("Moriarty.Web.Data.Models.Character", "Offender")
                        .WithMany()
                        .HasForeignKey("OffenderId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Moriarty.Web.Data.Models.Character", "Victim")
                        .WithMany()
                        .HasForeignKey("VictimId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Offender");

                    b.Navigation("Victim");
                });

            modelBuilder.Entity("Moriarty.Web.Data.Models.Clue", b =>
                {
                    b.HasOne("Moriarty.Web.Data.Models.Campaign", "Campaign")
                        .WithMany("Clues")
                        .HasForeignKey("CampaignId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Campaign");
                });

            modelBuilder.Entity("Moriarty.Web.Data.Models.Campaign", b =>
                {
                    b.Navigation("Clues");
                });
#pragma warning restore 612, 618
        }
    }
}
