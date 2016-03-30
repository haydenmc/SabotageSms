using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace SabotageSms.Migrations
{
    public partial class FixedGamePlayerRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_DbGamePlayer_DbGame_GameId", table: "GamePlayer");
            migrationBuilder.DropForeignKey(name: "FK_DbGamePlayer_DbPlayer_PlayerId", table: "GamePlayer");
            migrationBuilder.DropForeignKey(name: "FK_DbMessage_DbPlayer_PlayerId", table: "Message");
            migrationBuilder.DropForeignKey(name: "FK_DbRound_DbGame_GameId", table: "Round");
            migrationBuilder.AddForeignKey(
                name: "FK_DbGamePlayer_DbGame_GameId",
                table: "GamePlayer",
                column: "GameId",
                principalTable: "Game",
                principalColumn: "GameId",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_DbGamePlayer_DbPlayer_PlayerId",
                table: "GamePlayer",
                column: "PlayerId",
                principalTable: "Player",
                principalColumn: "PlayerId",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_DbMessage_DbPlayer_PlayerId",
                table: "Message",
                column: "PlayerId",
                principalTable: "Player",
                principalColumn: "PlayerId",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_DbRound_DbGame_GameId",
                table: "Round",
                column: "GameId",
                principalTable: "Game",
                principalColumn: "GameId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_DbGamePlayer_DbGame_GameId", table: "GamePlayer");
            migrationBuilder.DropForeignKey(name: "FK_DbGamePlayer_DbPlayer_PlayerId", table: "GamePlayer");
            migrationBuilder.DropForeignKey(name: "FK_DbMessage_DbPlayer_PlayerId", table: "Message");
            migrationBuilder.DropForeignKey(name: "FK_DbRound_DbGame_GameId", table: "Round");
            migrationBuilder.AddForeignKey(
                name: "FK_DbGamePlayer_DbGame_GameId",
                table: "GamePlayer",
                column: "GameId",
                principalTable: "Game",
                principalColumn: "GameId",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_DbGamePlayer_DbPlayer_PlayerId",
                table: "GamePlayer",
                column: "PlayerId",
                principalTable: "Player",
                principalColumn: "PlayerId",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_DbMessage_DbPlayer_PlayerId",
                table: "Message",
                column: "PlayerId",
                principalTable: "Player",
                principalColumn: "PlayerId",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_DbRound_DbGame_GameId",
                table: "Round",
                column: "GameId",
                principalTable: "Game",
                principalColumn: "GameId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
