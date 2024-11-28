using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ForestChurches.Migrations
{
    /// <inheritdoc />
    public partial class Added_Lists : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Facilities",
                table: "ChurchInformation",
                newName: "Activities");

            migrationBuilder.AddColumn<bool>(
                name: "Refreshments",
                table: "ChurchInformation",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Restrooms",
                table: "ChurchInformation",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "WheelchairAccess",
                table: "ChurchInformation",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Wifi",
                table: "ChurchInformation",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Refreshments",
                table: "ChurchInformation");

            migrationBuilder.DropColumn(
                name: "Restrooms",
                table: "ChurchInformation");

            migrationBuilder.DropColumn(
                name: "WheelchairAccess",
                table: "ChurchInformation");

            migrationBuilder.DropColumn(
                name: "Wifi",
                table: "ChurchInformation");

            migrationBuilder.RenameColumn(
                name: "Activities",
                table: "ChurchInformation",
                newName: "Facilities");
        }
    }
}
