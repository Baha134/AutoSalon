using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoSalon.Migrations
{
    /// <inheritdoc />
    public partial class AddCarIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "Cars",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Brand",
                table: "Cars",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Cars_Brand",
                table: "Cars",
                column: "Brand");

            migrationBuilder.CreateIndex(
                name: "IX_Cars_CreatedAt",
                table: "Cars",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Cars_IsActive_Status",
                table: "Cars",
                columns: new[] { "IsActive", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Cars_Price",
                table: "Cars",
                column: "Price");

            migrationBuilder.CreateIndex(
                name: "IX_Cars_Slug_Unique",
                table: "Cars",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cars_Year",
                table: "Cars",
                column: "Year");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Cars_Brand",
                table: "Cars");

            migrationBuilder.DropIndex(
                name: "IX_Cars_CreatedAt",
                table: "Cars");

            migrationBuilder.DropIndex(
                name: "IX_Cars_IsActive_Status",
                table: "Cars");

            migrationBuilder.DropIndex(
                name: "IX_Cars_Price",
                table: "Cars");

            migrationBuilder.DropIndex(
                name: "IX_Cars_Slug_Unique",
                table: "Cars");

            migrationBuilder.DropIndex(
                name: "IX_Cars_Year",
                table: "Cars");

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "Cars",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Brand",
                table: "Cars",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
