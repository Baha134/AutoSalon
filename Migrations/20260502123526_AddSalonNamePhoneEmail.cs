using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoSalon.Migrations
{
    /// <inheritdoc />
    public partial class AddSalonNamePhoneEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "SalonSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "SalonSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SalonName",
                table: "SalonSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "SalonSettings");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "SalonSettings");

            migrationBuilder.DropColumn(
                name: "SalonName",
                table: "SalonSettings");
        }
    }
}
