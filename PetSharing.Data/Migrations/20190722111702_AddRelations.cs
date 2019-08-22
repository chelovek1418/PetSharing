using Microsoft.EntityFrameworkCore.Migrations;

namespace PetSharing.Data.Migrations
{
    public partial class AddRelations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PetProfiles_AspNetUsers_UserId",
                table: "PetProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_PetProfiles_PetProfileId",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_PetProfileId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "PetProfileId",
                table: "Posts");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "PetProfiles",
                newName: "OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_PetProfiles_UserId",
                table: "PetProfiles",
                newName: "IX_PetProfiles_OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_PetId",
                table: "Posts",
                column: "PetId");

            migrationBuilder.AddForeignKey(
                name: "FK_PetProfiles_AspNetUsers_OwnerId",
                table: "PetProfiles",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_PetProfiles_PetId",
                table: "Posts",
                column: "PetId",
                principalTable: "PetProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PetProfiles_AspNetUsers_OwnerId",
                table: "PetProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_PetProfiles_PetId",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_PetId",
                table: "Posts");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "PetProfiles",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_PetProfiles_OwnerId",
                table: "PetProfiles",
                newName: "IX_PetProfiles_UserId");

            migrationBuilder.AddColumn<int>(
                name: "PetProfileId",
                table: "Posts",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_PetProfileId",
                table: "Posts",
                column: "PetProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_PetProfiles_AspNetUsers_UserId",
                table: "PetProfiles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_PetProfiles_PetProfileId",
                table: "Posts",
                column: "PetProfileId",
                principalTable: "PetProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
