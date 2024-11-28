using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ForestChurches.Migrations
{
    /// <inheritdoc />
    public partial class Servicetime_table_migrated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServiceTimes",
                table: "ChurchInformation");

            migrationBuilder.CreateTable(
                name: "ServiceTimes",
                columns: table => new
                {
                    ServiceID = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Time = table.Column<TimeOnly>(type: "time(0)", nullable: false),
                    Note = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ChurchInformationId = table.Column<string>(type: "varchar(95)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceTimes", x => x.ServiceID);
                    table.ForeignKey(
                        name: "FK_ServiceTimes_ChurchInformation_ChurchInformationId",
                        column: x => x.ChurchInformationId,
                        principalTable: "ChurchInformation",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceTimes_ChurchInformationId",
                table: "ServiceTimes",
                column: "ChurchInformationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceTimes");

            migrationBuilder.AddColumn<string>(
                name: "ServiceTimes",
                table: "ChurchInformation",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
