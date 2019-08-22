using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PetSharing.Data.Migrations
{
    public partial class UpdateEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "Posts",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfBirth",
                table: "PetProfiles",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "Comments",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "Comments");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "Posts",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfBirth",
                table: "PetProfiles",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);
        }
    }
}
