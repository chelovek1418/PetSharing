using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PetSharing.Data.Migrations
{
    public partial class SoftDelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SenderId",
                table: "Messages");

            migrationBuilder.AlterColumn<string>(
                name: "ReceiverId",
                table: "Messages",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "Messages",
                nullable: true,
                oldClrType: typeof(DateTime));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ReceiverId",
                table: "Messages",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "Messages",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SenderId",
                table: "Messages",
                nullable: false,
                defaultValue: 0);
        }
    }
}
