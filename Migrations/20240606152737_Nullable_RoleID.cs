using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ForestChurches.Migrations
{
    /// <inheritdoc />
    public partial class Nullable_RoleID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissions_AspNetRoles_churchRolesId",
                table: "RolePermissions");

            migrationBuilder.AlterColumn<string>(
                name: "churchRolesId",
                table: "RolePermissions",
                type: "varchar(95)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(95)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissions_AspNetRoles_churchRolesId",
                table: "RolePermissions",
                column: "churchRolesId",
                principalTable: "AspNetRoles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissions_AspNetRoles_churchRolesId",
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
                type: "varchar(95)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(95)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissions_AspNetRoles_churchRolesId",
                table: "RolePermissions",
                column: "churchRolesId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
