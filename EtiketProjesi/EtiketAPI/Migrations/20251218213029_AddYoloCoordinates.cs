using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtiketAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddYoloCoordinates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "height",
                table: "EtiketlenenImages",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "width",
                table: "EtiketlenenImages",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "x_center",
                table: "EtiketlenenImages",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "y_center",
                table: "EtiketlenenImages",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "height",
                table: "EtiketlenenImages");

            migrationBuilder.DropColumn(
                name: "width",
                table: "EtiketlenenImages");

            migrationBuilder.DropColumn(
                name: "x_center",
                table: "EtiketlenenImages");

            migrationBuilder.DropColumn(
                name: "y_center",
                table: "EtiketlenenImages");
        }
    }
}
