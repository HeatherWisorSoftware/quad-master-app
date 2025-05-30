using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuadMasterApp.Migrations
{
    /// <inheritdoc />
    public partial class AddQuadMatchAndTournamentStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Round1Opponent",
                table: "TournamentPlayers");

            migrationBuilder.DropColumn(
                name: "Round1Score",
                table: "TournamentPlayers");

            migrationBuilder.DropColumn(
                name: "Round1Table",
                table: "TournamentPlayers");

            migrationBuilder.DropColumn(
                name: "Round2Opponent",
                table: "TournamentPlayers");

            migrationBuilder.DropColumn(
                name: "Round2Score",
                table: "TournamentPlayers");

            migrationBuilder.DropColumn(
                name: "Round2Table",
                table: "TournamentPlayers");

            migrationBuilder.DropColumn(
                name: "Round3Opponent",
                table: "TournamentPlayers");

            migrationBuilder.DropColumn(
                name: "Round3Score",
                table: "TournamentPlayers");

            migrationBuilder.DropColumn(
                name: "Round3Table",
                table: "TournamentPlayers");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "Tournaments",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Quads",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Players",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "Quads");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Players");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "Tournaments",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Round1Opponent",
                table: "TournamentPlayers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Round1Score",
                table: "TournamentPlayers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Round1Table",
                table: "TournamentPlayers",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Round2Opponent",
                table: "TournamentPlayers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Round2Score",
                table: "TournamentPlayers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Round2Table",
                table: "TournamentPlayers",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Round3Opponent",
                table: "TournamentPlayers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Round3Score",
                table: "TournamentPlayers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Round3Table",
                table: "TournamentPlayers",
                type: "INTEGER",
                nullable: true);
        }
    }
}
