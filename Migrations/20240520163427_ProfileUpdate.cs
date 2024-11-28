using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ForestChurches.Migrations
{
    /// <inheritdoc />
    public partial class ProfileUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "ImageArray",
                table: "Events",
                type: "longblob",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
