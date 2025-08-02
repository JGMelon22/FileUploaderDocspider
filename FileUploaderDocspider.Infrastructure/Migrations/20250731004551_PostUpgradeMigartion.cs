using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileUploaderDocspider.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PostUpgradeMigartion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ux_documents_title",
                table: "documents");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "documents",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.CreateIndex(
                name: "ux_documents_title",
                table: "documents",
                column: "title");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ux_documents_title",
                table: "documents");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "documents",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.CreateIndex(
                name: "ux_documents_title",
                table: "documents",
                column: "title",
                unique: true);
        }
    }
}
