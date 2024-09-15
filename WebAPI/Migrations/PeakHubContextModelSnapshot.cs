﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebAPI.Data;

#nullable disable

namespace WebAPI.Migrations
{
    [DbContext(typeof(PeakHubContext))]
    partial class PeakHubContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("WebAPI.Models.Award", b =>
                {
                    b.Property<int>("AwardID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("AwardID"));

                    b.Property<string>("Condition")
                        .HasColumnType("longtext");

                    b.Property<string>("Img")
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .HasColumnType("longtext");

                    b.Property<int?>("UserID")
                        .HasColumnType("int");

                    b.HasKey("AwardID");

                    b.HasIndex("UserID");

                    b.ToTable("Awards");
                });

            modelBuilder.Entity("WebAPI.Models.Board", b =>
                {
                    b.Property<int>("BoardID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("BoardID"));

                    b.Property<string>("Name")
                        .HasColumnType("longtext");

                    b.HasKey("BoardID");

                    b.ToTable("Boards");
                });

            modelBuilder.Entity("WebAPI.Models.Like", b =>
                {
                    b.Property<int>("LikeID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("LikeID"));

                    b.Property<int>("PostID")
                        .HasColumnType("int");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.HasKey("LikeID");

                    b.HasIndex("PostID");

                    b.HasIndex("UserID");

                    b.ToTable("Likes");
                });

            modelBuilder.Entity("WebAPI.Models.Post", b =>
                {
                    b.Property<int>("PostID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("PostID"));

                    b.Property<int>("BoardID")
                        .HasColumnType("int");

                    b.Property<string>("Content")
                        .HasColumnType("longtext");

                    b.Property<int>("Likes")
                        .HasColumnType("int");

                    b.Property<string>("MediaLink")
                        .HasColumnType("longtext");

                    b.Property<string>("MediaType")
                        .HasColumnType("longtext");

                    b.Property<DateTime>("TransactionTimeUtc")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.HasKey("PostID");

                    b.HasIndex("BoardID");

                    b.HasIndex("UserID");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("WebAPI.Models.Task", b =>
                {
                    b.Property<int>("TaskID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("TaskID"));

                    b.Property<string>("AccountType")
                        .IsRequired()
                        .HasMaxLength(1)
                        .HasColumnType("char");

                    b.Property<string>("Coords")
                        .HasColumnType("longtext");

                    b.Property<string>("Details")
                        .HasColumnType("longtext");

                    b.Property<string>("IMG")
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .HasColumnType("longtext");

                    b.Property<int?>("UserID")
                        .HasColumnType("int");

                    b.HasKey("TaskID");

                    b.HasIndex("UserID");

                    b.ToTable("Tasks");
                });

            modelBuilder.Entity("WebAPI.Models.User", b =>
                {
                    b.Property<int>("UserID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("UserID"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(94)
                        .HasColumnType("char");

                    b.Property<string>("ProfileIMG")
                        .HasColumnType("longtext");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("UserID");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("WebAPI.Models.Award", b =>
                {
                    b.HasOne("WebAPI.Models.User", null)
                        .WithMany("Awards")
                        .HasForeignKey("UserID");
                });

            modelBuilder.Entity("WebAPI.Models.Like", b =>
                {
                    b.HasOne("WebAPI.Models.Post", "Post")
                        .WithMany()
                        .HasForeignKey("PostID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WebAPI.Models.User", "User")
                        .WithMany("Likes")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Post");

                    b.Navigation("User");
                });

            modelBuilder.Entity("WebAPI.Models.Post", b =>
                {
                    b.HasOne("WebAPI.Models.Board", "Board")
                        .WithMany("Posts")
                        .HasForeignKey("BoardID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WebAPI.Models.User", "User")
                        .WithMany("Posts")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Board");

                    b.Navigation("User");
                });

            modelBuilder.Entity("WebAPI.Models.Task", b =>
                {
                    b.HasOne("WebAPI.Models.User", null)
                        .WithMany("Tasks")
                        .HasForeignKey("UserID");
                });

            modelBuilder.Entity("WebAPI.Models.Board", b =>
                {
                    b.Navigation("Posts");
                });

            modelBuilder.Entity("WebAPI.Models.User", b =>
                {
                    b.Navigation("Awards");

                    b.Navigation("Likes");

                    b.Navigation("Posts");

                    b.Navigation("Tasks");
                });
#pragma warning restore 612, 618
        }
    }
}
