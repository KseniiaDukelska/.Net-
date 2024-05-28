using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rocky_DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductEventTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EventTime",
                table: "Products",
                newName: "StartTime");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndTime",
                table: "Products",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "Products",
                newName: "EventTime");
        }
    }
}
