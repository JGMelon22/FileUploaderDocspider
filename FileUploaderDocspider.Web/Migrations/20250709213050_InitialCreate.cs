using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using System;

namespace FileUploaderDocspider.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "documents",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(maxLength: 100, nullable: false),
                    description = table.Column<string>(maxLength: 2000, nullable: true),
                    file_name = table.Column<string>(nullable: false),
                    file_path = table.Column<string>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: false),
                    file_size = table.Column<long>(nullable: false),
                    content_type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_documents", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ux_documents_title",
                table: "documents",
                column: "title",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "documents");
        }
    }
}
