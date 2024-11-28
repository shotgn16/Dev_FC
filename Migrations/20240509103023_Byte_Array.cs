using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ForestChurches.Migrations
{
    /// <inheritdoc />
    public partial class Byte_Array : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Logo",
                table: "Events",
                newName: "ImageArray"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageArray",
                table: "Events",
                newName: "Logo");
        }
    }
}
