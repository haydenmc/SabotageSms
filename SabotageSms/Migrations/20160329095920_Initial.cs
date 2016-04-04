using System;
using Microsoft.Data.Entity.Migrations;
using Microsoft.Data.Entity.Metadata;

namespace SabotageSms.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Game",
                columns: table => new
                {
                    GameId = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedTime = table.Column<DateTimeOffset>(nullable: false),
                    CurrentState = table.Column<string>(nullable: true),
                    JoinCode = table.Column<string>(nullable: true),
                    LastActiveTime = table.Column<DateTimeOffset>(nullable: false),
                    LeaderCount = table.Column<int>(nullable: false),
                    MissionCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbGame", x => x.GameId);
                });
            migrationBuilder.CreateTable(
                name: "Round",
                columns: table => new
                {
                    RoundId = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BadWins = table.Column<bool>(nullable: false),
                    GameId = table.Column<long>(nullable: false),
                    RejectedCount = table.Column<int>(nullable: false),
                    RoundNumber = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbRound", x => x.RoundId);
                    table.ForeignKey(
                        name: "FK_DbRound_DbGame_GameId",
                        column: x => x.GameId,
                        principalTable: "Game",
                        principalColumn: "GameId",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateTable(
                name: "Player",
                columns: table => new
                {
                    PlayerId = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CurrentGameId = table.Column<long>(nullable: true),
                    DbRoundRoundId = table.Column<long>(nullable: true),
                    DbRoundRoundId1 = table.Column<long>(nullable: true),
                    DbRoundRoundId2 = table.Column<long>(nullable: true),
                    DbRoundRoundId3 = table.Column<long>(nullable: true),
                    DbRoundRoundId4 = table.Column<long>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbPlayer", x => x.PlayerId);
                    table.ForeignKey(
                        name: "FK_DbPlayer_DbGame_CurrentGameId",
                        column: x => x.CurrentGameId,
                        principalTable: "Game",
                        principalColumn: "GameId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DbPlayer_DbRound_DbRoundRoundId",
                        column: x => x.DbRoundRoundId,
                        principalTable: "Round",
                        principalColumn: "RoundId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DbPlayer_DbRound_DbRoundRoundId1",
                        column: x => x.DbRoundRoundId1,
                        principalTable: "Round",
                        principalColumn: "RoundId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DbPlayer_DbRound_DbRoundRoundId2",
                        column: x => x.DbRoundRoundId2,
                        principalTable: "Round",
                        principalColumn: "RoundId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DbPlayer_DbRound_DbRoundRoundId3",
                        column: x => x.DbRoundRoundId3,
                        principalTable: "Round",
                        principalColumn: "RoundId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DbPlayer_DbRound_DbRoundRoundId4",
                        column: x => x.DbRoundRoundId4,
                        principalTable: "Round",
                        principalColumn: "RoundId",
                        onDelete: ReferentialAction.Restrict);
                });
            migrationBuilder.CreateTable(
                name: "GamePlayer",
                columns: table => new
                {
                    PlayerId = table.Column<long>(nullable: false),
                    GameId = table.Column<long>(nullable: false),
                    IsBad = table.Column<bool>(nullable: false),
                    TurnOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbGamePlayer", x => new { x.PlayerId, x.GameId });
                    table.ForeignKey(
                        name: "FK_DbGamePlayer_DbGame_GameId",
                        column: x => x.GameId,
                        principalTable: "Game",
                        principalColumn: "GameId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DbGamePlayer_DbPlayer_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Player",
                        principalColumn: "PlayerId",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateTable(
                name: "Message",
                columns: table => new
                {
                    MessageId = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Body = table.Column<string>(nullable: true),
                    GameId = table.Column<long>(nullable: true),
                    PlayerId = table.Column<long>(nullable: false),
                    ReceivedTime = table.Column<DateTimeOffset>(nullable: false),
                    Result = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbMessage", x => x.MessageId);
                    table.ForeignKey(
                        name: "FK_DbMessage_DbGame_GameId",
                        column: x => x.GameId,
                        principalTable: "Game",
                        principalColumn: "GameId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DbMessage_DbPlayer_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Player",
                        principalColumn: "PlayerId",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateIndex(
                name: "IX_DbGame_JoinCode",
                table: "Game",
                column: "JoinCode",
                unique: true);
            migrationBuilder.CreateIndex(
                name: "IX_DbPlayer_PhoneNumber",
                table: "Player",
                column: "PhoneNumber",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("GamePlayer");
            migrationBuilder.DropTable("Message");
            migrationBuilder.DropTable("Player");
            migrationBuilder.DropTable("Round");
            migrationBuilder.DropTable("Game");
        }
    }
}
