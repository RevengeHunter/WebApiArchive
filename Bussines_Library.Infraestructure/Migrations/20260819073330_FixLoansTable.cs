using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bussines_Library.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class FixLoansTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Loans_Books_BookId1",
                table: "Loans");

            migrationBuilder.DropIndex(
                name: "IX_Loans_BookId1",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "BookId1",
                table: "Loans");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BookId1",
                table: "Loans",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Loans_BookId1",
                table: "Loans",
                column: "BookId1",
                unique: true,
                filter: "[BookId1] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_Books_BookId1",
                table: "Loans",
                column: "BookId1",
                principalTable: "Books",
                principalColumn: "Id");
        }
    }
}
