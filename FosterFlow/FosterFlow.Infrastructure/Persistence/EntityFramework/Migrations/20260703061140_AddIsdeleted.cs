using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FosterFlow.Infrastructure.Persistence.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddIsdeleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "Cats");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Cats",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Cats",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Cats",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "Cats",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Cats");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Cats");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Cats");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "Cats");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                table: "Cats",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
