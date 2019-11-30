using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

namespace Api.Migrations
{
    public partial class AddCheckpointNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Altitude",
                table: "Point");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Point");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Point");

            migrationBuilder.AddColumn<Point>(
                name: "Coordinates",
                table: "Point",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Number",
                table: "Point",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Coordinates",
                table: "Point");

            migrationBuilder.DropColumn(
                name: "Number",
                table: "Point");

            migrationBuilder.AddColumn<double>(
                name: "Altitude",
                table: "Point",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Point",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Point",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
