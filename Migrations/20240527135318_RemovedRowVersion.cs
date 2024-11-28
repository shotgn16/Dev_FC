using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ForestChurches.Migrations
{
    /// <inheritdoc />
    public partial class RemovedRowVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "AspNetUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "AspNetUsers",
                type: "longblob",
                nullable: false);
        }
    }
}
