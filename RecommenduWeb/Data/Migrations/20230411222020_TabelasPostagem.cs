using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecommenduWeb.Data.Migrations
{
    /// <inheritdoc />
    public partial class TabelasPostagem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "PostagemSequence");

            migrationBuilder.AddColumn<string>(
                name: "NomeCompleto",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Cidade",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImagemPerfil",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Reputacao",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "comentarioPostagem",
                columns: table => new
                {
                    ComentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubComentId = table.Column<int>(type: "int", nullable: false),
                    Comentario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostagemId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comentarioPostagem", x => x.ComentId);
                });

            migrationBuilder.CreateTable(
                name: "PostagemProduto",
                columns: table => new
                {
                    PostagemId = table.Column<int>(type: "int", nullable: false, defaultValueSql: "NEXT VALUE FOR [PostagemSequence]"),
                    Categoria = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PublicoAlvo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImgPostagem = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DtPostagem = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Curtidas = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Modelo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fabricante = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LinkProduto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TempoUso = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostagemProduto", x => x.PostagemId);
                    table.ForeignKey(
                        name: "FK_PostagemProduto_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PostagemServico",
                columns: table => new
                {
                    PostagemId = table.Column<int>(type: "int", nullable: false, defaultValueSql: "NEXT VALUE FOR [PostagemSequence]"),
                    Categoria = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PublicoAlvo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImgPostagem = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DtPostagem = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Curtidas = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NomeServico = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Endereco = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Contato = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostagemServico", x => x.PostagemId);
                    table.ForeignKey(
                        name: "FK_PostagemServico_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_comentarioPostagem_PostagemId",
                table: "comentarioPostagem",
                column: "PostagemId");

            migrationBuilder.CreateIndex(
                name: "IX_PostagemProduto_UsuarioId",
                table: "PostagemProduto",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_PostagemServico_UsuarioId",
                table: "PostagemServico",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "comentarioPostagem");

            migrationBuilder.DropTable(
                name: "PostagemProduto");

            migrationBuilder.DropTable(
                name: "PostagemServico");

            migrationBuilder.DropColumn(
                name: "Cidade",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ImagemPerfil",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "NomeCompleto",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Reputacao",
                table: "AspNetUsers");

            migrationBuilder.DropSequence(
                name: "PostagemSequence");
        }
    }
}
