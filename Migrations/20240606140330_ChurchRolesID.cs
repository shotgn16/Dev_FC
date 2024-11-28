using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ForestChurches.Migrations
{
    /// <inheritdoc />
    public partial class ChurchRolesID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissions_AspNetRoles_ChurchRolesId",
                table: "RolePermissions");

            migrationBuilder.RenameColumn(
                name: "ChurchRolesId",
                table: "RolePermissions",
                newName: "churchRolesId");

            migrationBuilder.RenameIndex(
                name: "IX_RolePermissions_ChurchRolesId",
                table: "RolePermissions",
                newName: "IX_RolePermissions_churchRolesId");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissions_AspNetRoles_churchRolesId",
                table: "RolePermissions");

            migrationBuilder.RenameColumn(
                name: "churchRolesId",
                table: "RolePermissions",
                newName: "ChurchRolesId");

            migrationBuilder.RenameIndex(
                name: "IX_RolePermissions_churchRolesId",
                table: "RolePermissions",
                newName: "IX_RolePermissions_ChurchRolesId");

            migrationBuilder.AlterColumn<string>(
                name: "ChurchRolesId",
                table: "RolePermissions",
                type: "varchar(95)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(95)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissions_AspNetRoles_ChurchRolesId",
                table: "RolePermissions",
                column: "ChurchRolesId",
                principalTable: "AspNetRoles",
                principalColumn: "Id");
        }
    }
}
