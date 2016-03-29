using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;
using SabotageSms.Models.DbModels;

namespace SabotageSms.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20160329095920_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-rc1-16348")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("SabotageSms.Models.DbModels.DbGame", b =>
                {
                    b.Property<long>("GameId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTimeOffset>("CreatedTime");

                    b.Property<string>("CurrentState");

                    b.Property<string>("JoinCode")
                        .HasAnnotation("MaxLength", 120);

                    b.Property<DateTimeOffset>("LastActiveTime");

                    b.Property<int>("LeaderCount");

                    b.Property<int>("MissionCount");

                    b.HasKey("GameId");

                    b.HasIndex("JoinCode")
                        .IsUnique();

                    b.HasAnnotation("Relational:TableName", "Game");
                });

            modelBuilder.Entity("SabotageSms.Models.DbModels.DbGamePlayer", b =>
                {
                    b.Property<long>("PlayerId");

                    b.Property<long>("GameId");

                    b.Property<bool>("IsBad");

                    b.Property<int>("TurnOrder");

                    b.HasKey("PlayerId", "GameId");

                    b.HasAnnotation("Relational:TableName", "GamePlayer");
                });

            modelBuilder.Entity("SabotageSms.Models.DbModels.DbMessage", b =>
                {
                    b.Property<long>("MessageId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Body");

                    b.Property<long?>("GameId");

                    b.Property<long>("PlayerId");

                    b.Property<DateTimeOffset>("ReceivedTime");

                    b.Property<int>("Result");

                    b.HasKey("MessageId");

                    b.HasAnnotation("Relational:TableName", "Message");
                });

            modelBuilder.Entity("SabotageSms.Models.DbModels.DbPlayer", b =>
                {
                    b.Property<long>("PlayerId")
                        .ValueGeneratedOnAdd();

                    b.Property<long?>("CurrentGameId");

                    b.Property<long?>("DbRoundRoundId");

                    b.Property<long?>("DbRoundRoundId1");

                    b.Property<long?>("DbRoundRoundId2");

                    b.Property<long?>("DbRoundRoundId3");

                    b.Property<long?>("DbRoundRoundId4");

                    b.Property<string>("Name");

                    b.Property<string>("PhoneNumber")
                        .HasAnnotation("MaxLength", 50);

                    b.HasKey("PlayerId");

                    b.HasIndex("PhoneNumber")
                        .IsUnique();

                    b.HasAnnotation("Relational:TableName", "Player");
                });

            modelBuilder.Entity("SabotageSms.Models.DbModels.DbRound", b =>
                {
                    b.Property<long>("RoundId")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("BadWins");

                    b.Property<long>("GameId");

                    b.Property<int>("RejectedCount");

                    b.Property<int>("RoundNumber");

                    b.HasKey("RoundId");

                    b.HasAnnotation("Relational:TableName", "Round");
                });

            modelBuilder.Entity("SabotageSms.Models.DbModels.DbGamePlayer", b =>
                {
                    b.HasOne("SabotageSms.Models.DbModels.DbGame")
                        .WithMany()
                        .HasForeignKey("GameId");

                    b.HasOne("SabotageSms.Models.DbModels.DbPlayer")
                        .WithMany()
                        .HasForeignKey("PlayerId");
                });

            modelBuilder.Entity("SabotageSms.Models.DbModels.DbMessage", b =>
                {
                    b.HasOne("SabotageSms.Models.DbModels.DbGame")
                        .WithMany()
                        .HasForeignKey("GameId");

                    b.HasOne("SabotageSms.Models.DbModels.DbPlayer")
                        .WithMany()
                        .HasForeignKey("PlayerId");
                });

            modelBuilder.Entity("SabotageSms.Models.DbModels.DbPlayer", b =>
                {
                    b.HasOne("SabotageSms.Models.DbModels.DbGame")
                        .WithMany()
                        .HasForeignKey("CurrentGameId");

                    b.HasOne("SabotageSms.Models.DbModels.DbRound")
                        .WithMany()
                        .HasForeignKey("DbRoundRoundId");

                    b.HasOne("SabotageSms.Models.DbModels.DbRound")
                        .WithMany()
                        .HasForeignKey("DbRoundRoundId1");

                    b.HasOne("SabotageSms.Models.DbModels.DbRound")
                        .WithMany()
                        .HasForeignKey("DbRoundRoundId2");

                    b.HasOne("SabotageSms.Models.DbModels.DbRound")
                        .WithMany()
                        .HasForeignKey("DbRoundRoundId3");

                    b.HasOne("SabotageSms.Models.DbModels.DbRound")
                        .WithMany()
                        .HasForeignKey("DbRoundRoundId4");
                });

            modelBuilder.Entity("SabotageSms.Models.DbModels.DbRound", b =>
                {
                    b.HasOne("SabotageSms.Models.DbModels.DbGame")
                        .WithMany()
                        .HasForeignKey("GameId");
                });
        }
    }
}
