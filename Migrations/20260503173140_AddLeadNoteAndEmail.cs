using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoSalon.Migrations
{
    /// <inheritdoc />
    public partial class AddLeadNoteAndEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdminNote",
                table: "Leads",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Leads",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminNote",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Leads");
        }
    }
}
