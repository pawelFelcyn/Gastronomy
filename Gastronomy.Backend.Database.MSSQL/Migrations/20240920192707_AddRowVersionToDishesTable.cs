﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gastronomy.Backend.Database.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class AddRowVersionToDishesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Dishes",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Dishes");
        }
    }
}
