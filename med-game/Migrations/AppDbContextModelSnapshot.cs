﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using med_game.src.Data;

#nullable disable

namespace med_game.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("AchievementUser", b =>
                {
                    b.Property<int>("AchievementsId")
                        .HasColumnType("integer");

                    b.Property<long>("UsersId")
                        .HasColumnType("bigint");

                    b.HasKey("AchievementsId", "UsersId");

                    b.HasIndex("UsersId");

                    b.ToTable("AchievementUser");
                });

            modelBuilder.Entity("AnswerQuestion", b =>
                {
                    b.Property<long>("AnswersId")
                        .HasColumnType("bigint");

                    b.Property<long>("QuestionsId")
                        .HasColumnType("bigint");

                    b.HasKey("AnswersId", "QuestionsId");

                    b.HasIndex("QuestionsId");

                    b.ToTable("AnswerQuestion");
                });

            modelBuilder.Entity("med_game.Models.Achievement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("CountPoints")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Image")
                        .HasColumnType("text");

                    b.Property<int>("MaxCountPoints")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("character varying(150)");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Achievements");
                });

            modelBuilder.Entity("med_game.src.Models.Answer", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Image")
                        .HasColumnType("text");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Type");

                    b.ToTable("Answers");
                });

            modelBuilder.Entity("med_game.src.Models.FriendRequest", b =>
                {
                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.Property<long>("SubscriberId")
                        .HasColumnType("bigint");

                    b.HasKey("UserId", "SubscriberId");

                    b.HasIndex("SubscriberId");

                    b.ToTable("FriendRequest");
                });

            modelBuilder.Entity("med_game.src.Models.Lectern", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Lecterns");
                });

            modelBuilder.Entity("med_game.src.Models.Module", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<int>("LecternId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("LecternId");

                    b.ToTable("Modules");
                });

            modelBuilder.Entity("med_game.src.Models.Question", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long>("CorrectAnswerIndex")
                        .HasColumnType("bigint");

                    b.Property<int>("CountPointsPerAnswer")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Image")
                        .HasColumnType("text");

                    b.Property<int>("ModuleId")
                        .HasColumnType("integer");

                    b.Property<string>("Text")
                        .HasColumnType("text");

                    b.Property<int>("TimeSeconds")
                        .HasColumnType("integer");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ModuleId");

                    b.HasIndex("Type");

                    b.ToTable("Questions");
                });

            modelBuilder.Entity("med_game.src.Models.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Image")
                        .HasColumnType("text");

                    b.Property<string>("Nickname")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Rating")
                        .HasColumnType("integer");

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("TokenHash")
                        .HasColumnType("text");

                    b.Property<DateTime?>("TokenValidBefore")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("med_game.src.models.Friends", b =>
                {
                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.Property<long>("FriendId")
                        .HasColumnType("bigint");

                    b.HasKey("UserId", "FriendId");

                    b.HasIndex("FriendId");

                    b.ToTable("Friends");
                });

            modelBuilder.Entity("AchievementUser", b =>
                {
                    b.HasOne("med_game.Models.Achievement", null)
                        .WithMany()
                        .HasForeignKey("AchievementsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("med_game.src.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UsersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AnswerQuestion", b =>
                {
                    b.HasOne("med_game.src.Models.Answer", null)
                        .WithMany()
                        .HasForeignKey("AnswersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("med_game.src.Models.Question", null)
                        .WithMany()
                        .HasForeignKey("QuestionsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("med_game.src.Models.FriendRequest", b =>
                {
                    b.HasOne("med_game.src.Models.User", "Subscriber")
                        .WithMany("FriendRequestFromMe")
                        .HasForeignKey("SubscriberId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("med_game.src.Models.User", "User")
                        .WithMany("FriendRequestToMe")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Subscriber");

                    b.Navigation("User");
                });

            modelBuilder.Entity("med_game.src.Models.Module", b =>
                {
                    b.HasOne("med_game.src.Models.Lectern", "Lectern")
                        .WithMany("Modules")
                        .HasForeignKey("LecternId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Lectern");
                });

            modelBuilder.Entity("med_game.src.Models.Question", b =>
                {
                    b.HasOne("med_game.src.Models.Module", "Module")
                        .WithMany("Questions")
                        .HasForeignKey("ModuleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Module");
                });

            modelBuilder.Entity("med_game.src.models.Friends", b =>
                {
                    b.HasOne("med_game.src.Models.User", "Friend")
                        .WithMany("FriendsAcceptedByMe")
                        .HasForeignKey("FriendId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("med_game.src.Models.User", "User")
                        .WithMany("FriendsAcceptedMe")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Friend");

                    b.Navigation("User");
                });

            modelBuilder.Entity("med_game.src.Models.Lectern", b =>
                {
                    b.Navigation("Modules");
                });

            modelBuilder.Entity("med_game.src.Models.Module", b =>
                {
                    b.Navigation("Questions");
                });

            modelBuilder.Entity("med_game.src.Models.User", b =>
                {
                    b.Navigation("FriendRequestFromMe");

                    b.Navigation("FriendRequestToMe");

                    b.Navigation("FriendsAcceptedByMe");

                    b.Navigation("FriendsAcceptedMe");
                });
#pragma warning restore 612, 618
        }
    }
}
