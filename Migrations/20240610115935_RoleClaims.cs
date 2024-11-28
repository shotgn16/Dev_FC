using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ForestChurches.Migrations
{
    /// <inheritdoc />
    public partial class RoleClaims : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissions_AspNetRoles_churchRolesId",
                table: "RolePermissions");

            migrationBuilder.DropIndex(
                name: "IX_RolePermissions_churchRolesId",
                table: "RolePermissions");

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "churchRolesId",
                keyValue: null,
                column: "churchRolesId",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "churchRolesId",
                table: "RolePermissions",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(95)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ChurchRolesId",
                table: "AspNetRoleClaims",
                type: "varchar(95)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_ChurchRolesId",
                table: "AspNetRoleClaims",
                column: "ChurchRolesId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_ChurchRolesId",
                table: "AspNetRoleClaims",
                column: "ChurchRolesId",
                principalTable: "AspNetRoles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<string>(
                name: "churchRolesId",
                table: "RolePermissions",
                type: "varchar(95)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_churchRolesId",
                table: "RolePermissions",
                column: "churchRolesId");

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissions_AspNetRoles_churchRolesId",
                table: "RolePermissions",
                column: "churchRolesId",
                principalTable: "AspNetRoles",
                principalColumn: "Id");
        }
    }
}
