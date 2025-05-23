﻿// <auto-generated />
using System;
using EVotingSystem_SBMM.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace EVotingSystem_SBMM.Migrations
{
    [DbContext(typeof(EVotingSystemDB))]
    [Migration("20240223163014_VoterIdRelationship")]
    partial class VoterIdRelationship
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("EVotingSystem_SBMM.Models.CandidateModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsElected")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Party")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Profile")
                        .HasColumnType("int");

                    b.Property<string>("ProfileImage")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Candidates");
                });

            modelBuilder.Entity("EVotingSystem_SBMM.Models.EventModel", b =>
                {
                    b.Property<int>("EventId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("EventId"), 1L, 1);

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("EventType")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.HasKey("EventId");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("EVotingSystem_SBMM.Models.UserModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Profile")
                        .HasColumnType("int");

                    b.Property<DateTime>("RegisterDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("EVotingSystem_SBMM.Models.VoteAuditModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Action")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ActionTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("CandidateId")
                        .HasColumnType("int");

                    b.Property<int>("EventId")
                        .HasColumnType("int");

                    b.Property<int>("VoteId")
                        .HasColumnType("int");

                    b.Property<int>("VoterId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("VoteAudits");
                });

            modelBuilder.Entity("EVotingSystem_SBMM.Models.VoteModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("CandidateId")
                        .HasColumnType("int");

                    b.Property<int>("EventId")
                        .HasColumnType("int");

                    b.Property<DateTime>("VotedAtTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("VoterId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CandidateId");

                    b.HasIndex("EventId");

                    b.HasIndex("VoterId");

                    b.ToTable("Votes");
                });

            modelBuilder.Entity("EVotingSystem_SBMM.Models.VotePreferenceModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("CandidateId")
                        .HasColumnType("int");

                    b.Property<int?>("EventId")
                        .IsRequired()
                        .HasColumnType("int");

                    b.Property<int>("Rank")
                        .HasColumnType("int");

                    b.Property<int>("VoteId")
                        .HasColumnType("int");

                    b.Property<int>("VoterId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CandidateId");

                    b.HasIndex("EventId");

                    b.HasIndex("VoteId");

                    b.HasIndex("VoterId");

                    b.ToTable("VotePreferences");
                });

            modelBuilder.Entity("EVotingSystem_SBMM.Models.VoterModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("BirthDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Mobile")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Profile")
                        .HasColumnType("int");

                    b.Property<string>("State")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Voters");
                });

            modelBuilder.Entity("EVotingSystem_SBMM.Repository.VotePreferenceAuditModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Action")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ActionTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("CandidateId")
                        .HasColumnType("int");

                    b.Property<int?>("EventId")
                        .HasColumnType("int");

                    b.Property<int>("VotePreferenceId")
                        .HasColumnType("int");

                    b.Property<int>("VoterId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("VotePreferenceAudits");
                });

            modelBuilder.Entity("EVotingSystem_SBMM.Models.VoteModel", b =>
                {
                    b.HasOne("EVotingSystem_SBMM.Models.CandidateModel", "Candidates")
                        .WithMany("Votes")
                        .HasForeignKey("CandidateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EVotingSystem_SBMM.Models.EventModel", "Event")
                        .WithMany("Votes")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EVotingSystem_SBMM.Models.VoterModel", "Voter")
                        .WithMany("Votes")
                        .HasForeignKey("VoterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Candidates");

                    b.Navigation("Event");

                    b.Navigation("Voter");
                });

            modelBuilder.Entity("EVotingSystem_SBMM.Models.VotePreferenceModel", b =>
                {
                    b.HasOne("EVotingSystem_SBMM.Models.CandidateModel", "Candidates")
                        .WithMany()
                        .HasForeignKey("CandidateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EVotingSystem_SBMM.Models.EventModel", "Event")
                        .WithMany()
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EVotingSystem_SBMM.Models.VoteModel", "Votes")
                        .WithMany("Preferences")
                        .HasForeignKey("VoteId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EVotingSystem_SBMM.Models.VoterModel", "Voters")
                        .WithMany()
                        .HasForeignKey("VoterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Candidates");

                    b.Navigation("Event");

                    b.Navigation("Voters");

                    b.Navigation("Votes");
                });

            modelBuilder.Entity("EVotingSystem_SBMM.Models.CandidateModel", b =>
                {
                    b.Navigation("Votes");
                });

            modelBuilder.Entity("EVotingSystem_SBMM.Models.EventModel", b =>
                {
                    b.Navigation("Votes");
                });

            modelBuilder.Entity("EVotingSystem_SBMM.Models.VoteModel", b =>
                {
                    b.Navigation("Preferences");
                });

            modelBuilder.Entity("EVotingSystem_SBMM.Models.VoterModel", b =>
                {
                    b.Navigation("Votes");
                });
#pragma warning restore 612, 618
        }
    }
}
