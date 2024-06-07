using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rocky_DataAccess.Migrations
{
    /// <inheritdoc />
    
        public partial class AgeAdded : Migration
        {
            protected override void Up(MigrationBuilder migrationBuilder)
            {
                migrationBuilder.AddColumn<int>(
                    name: "AgeRestriction",
                    table: "Products",
                    type: "int",
                    nullable: true);
            }

            protected override void Down(MigrationBuilder migrationBuilder)
            {
                migrationBuilder.DropColumn(
                    name: "AgeRestriction",
                    table: "Products");
            }
        }
    }

