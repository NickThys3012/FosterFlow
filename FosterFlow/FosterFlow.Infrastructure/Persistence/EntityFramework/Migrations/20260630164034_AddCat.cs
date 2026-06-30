using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FosterFlow.Infrastructure.Persistence.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddCat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BirthDate",
                table: "Cats");

            migrationBuilder.AddColumn<int>(
                name: "Age",
                table: "Cats",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "DogFriendly",
                table: "Cats",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "FosterDuration",
                table: "Cats",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsUrgent",
                table: "Cats",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "MedicalNeeds",
                table: "Cats",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl",
                table: "Cats",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Sex",
                table: "Cats",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ShelterId",
                table: "Cats",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TemperamentTags",
                table: "Cats",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.CreateIndex(
                name: "IX_Cats_ShelterId",
                table: "Cats",
                column: "ShelterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cats_AspNetUsers_ShelterId",
                table: "Cats",
                column: "ShelterId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cats_AspNetUsers_ShelterId",
                table: "Cats");

            migrationBuilder.DropIndex(
                name: "IX_Cats_ShelterId",
                table: "Cats");

            migrationBuilder.DropColumn(
                name: "Age",
                table: "Cats");

            migrationBuilder.DropColumn(
                name: "DogFriendly",
                table: "Cats");

            migrationBuilder.DropColumn(
                name: "FosterDuration",
                table: "Cats");

            migrationBuilder.DropColumn(
                name: "IsUrgent",
                table: "Cats");

            migrationBuilder.DropColumn(
                name: "MedicalNeeds",
                table: "Cats");

            migrationBuilder.DropColumn(
                name: "PhotoUrl",
                table: "Cats");

            migrationBuilder.DropColumn(
                name: "Sex",
                table: "Cats");

            migrationBuilder.DropColumn(
                name: "ShelterId",
                table: "Cats");

            migrationBuilder.DropColumn(
                name: "TemperamentTags",
                table: "Cats");

            migrationBuilder.AddColumn<DateTime>(
                name: "BirthDate",
                table: "Cats",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
