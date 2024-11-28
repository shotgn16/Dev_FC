using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ForestChurches.Migrations
{
    /// <inheritdoc />
    public partial class RemoveChurchRolesID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_ChurchRolesId",
                table: "AspNetRoleClaims");

            migrationBuilder.DropIndex(
                name: "IX_AspNetRoleClaims_ChurchRolesId",
                table: "AspNetRoleClaims");

            migrationBuilder.DropColumn(
                name: "ChurchRolesId",
                table: "AspNetRoleClaims");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChurchRolesId",
                table: "AspNetRoleClaims",
                type: "varchar(95)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_ChurchRolesId",
                table: "AspNetRoleClaims",
                column: "ChurchRolesId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_ChurchRolesId",
                table: "AspNetRoleClaims",
                column: "ChurchRolesId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
