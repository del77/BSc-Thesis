using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Api.Migrations
{
    public partial class MoveRoutePropertiesToRouteTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RouteProperties");

            migrationBuilder.AddColumn<double>(
                name: "Properties_Distance",
                table: "Routes",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Properties_HeightAboveSeaLevel",
                table: "Routes",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Properties_Name",
                table: "Routes",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Properties_PavedPercentage",
                table: "Routes",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Properties_Distance",
                table: "Routes");

            migrationBuilder.DropColumn(
                name: "Properties_HeightAboveSeaLevel",
                table: "Routes");

            migrationBuilder.DropColumn(
                name: "Properties_Name",
                table: "Routes");

            migrationBuilder.DropColumn(
                name: "Properties_PavedPercentage",
                table: "Routes");

            migrationBuilder.CreateTable(
                name: "RouteProperties",
                columns: table => new
                {
                    RouteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Distance = table.Column<double>(type: "float", nullable: false),
                    HeightAboveSeaLevel = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PavedPercentage = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteProperties", x => x.RouteId);
                    table.ForeignKey(
                        name: "FK_RouteProperties_Routes_RouteId",
                        column: x => x.RouteId,
                        principalTable: "Routes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
