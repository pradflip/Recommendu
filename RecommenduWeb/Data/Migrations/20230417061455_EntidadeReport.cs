using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecommenduWeb.Data.Migrations
{
    /// <inheritdoc />
    public partial class EntidadeReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_comentarioPostagem",
                table: "comentarioPostagem");

            migrationBuilder.RenameTable(
                name: "comentarioPostagem",
                newName: "ComentarioPostagem");

            migrationBuilder.RenameIndex(
                name: "IX_comentarioPostagem_PostagemId",
                table: "ComentarioPostagem",
                newName: "IX_ComentarioPostagem_PostagemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ComentarioPostagem",
                table: "ComentarioPostagem",
                column: "ComentId");

            migrationBuilder.CreateTable(
                name: "ReportPostagemNegativas",
                columns: table => new
                {
                    ReportId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DtReport = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostagemId = table.Column<int>(type: "int", nullable: false),
                    Categoria = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DtPostagem = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportPostagemNegativas", x => x.ReportId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReportPostagemNegativas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ComentarioPostagem",
                table: "ComentarioPostagem");

            migrationBuilder.RenameTable(
                name: "ComentarioPostagem",
                newName: "comentarioPostagem");

            migrationBuilder.RenameIndex(
                name: "IX_ComentarioPostagem_PostagemId",
                table: "comentarioPostagem",
                newName: "IX_comentarioPostagem_PostagemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_comentarioPostagem",
                table: "comentarioPostagem",
                column: "ComentId");
        }
    }
}
