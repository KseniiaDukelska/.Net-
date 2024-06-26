﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rocky_DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserInteraction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserInteractions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserInteractions",
                columns: table => new
                {
                    InteractionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InteractionTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InteractionType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    InteractionValue = table.Column<float>(type: "real", nullable: true),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInteractions", x => x.InteractionId);
                });
        }
    }
}
