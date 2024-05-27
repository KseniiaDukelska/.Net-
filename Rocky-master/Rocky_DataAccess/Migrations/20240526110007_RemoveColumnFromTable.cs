using Microsoft.EntityFrameworkCore.Migrations;

namespace Rocky_DataAccess.Migrations
{
    public partial class RemoveColumnFromTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the foreign key constraint
            migrationBuilder.DropForeignKey(
                name: "FK_Likes_Products_ProductId",
                table: "Likes");

            // Drop the index
            migrationBuilder.DropIndex(
                name: "IX_Likes_ProductId",
                table: "Likes");

            // Drop the column
            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Likes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Add the column back
            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "Likes",
                type: "int",
                nullable: true);

            // Recreate the index
            migrationBuilder.CreateIndex(
                name: "IX_Likes_ProductId",
                table: "Likes",
                column: "ProductId");

            // Recreate the foreign key constraint
            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Products_ProductId",
                table: "Likes",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
