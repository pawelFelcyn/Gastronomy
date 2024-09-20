using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gastronomy.Backend.Database.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class AddIsActiveColumnToDishesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Dishes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Dishes");
        }
    }
}
