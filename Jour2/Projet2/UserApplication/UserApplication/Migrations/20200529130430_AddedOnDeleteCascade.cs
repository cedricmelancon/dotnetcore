using Microsoft.EntityFrameworkCore.Migrations;

namespace UserApplication.Migrations
{
    public partial class AddedOnDeleteCascade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Users_UserModelId",
                table: "Addresses");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Users_UserModelId",
                table: "Addresses",
                column: "UserModelId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Users_UserModelId",
                table: "Addresses");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Users_UserModelId",
                table: "Addresses",
                column: "UserModelId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
