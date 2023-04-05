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
                name: "ImgPerfil",
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
                name: "Postagem",
                columns: table => new
                {
                    PostagemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Categoria = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PublicoAlvo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DtPostagem = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Curtidas = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Modelo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Fabricante = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LinkProduto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TempoUso = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Endereco = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Contato = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Postagem", x => x.PostagemId);
                    table.ForeignKey(
                        name: "FK_Postagem_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                    table.ForeignKey(
                        name: "FK_comentarioPostagem_Postagem_PostagemId",
                        column: x => x.PostagemId,
                        principalTable: "Postagem",
                        principalColumn: "PostagemId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "imagemPostagem",
                columns: table => new
                {
                    ImgId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Imagem = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostagemId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_imagemPostagem", x => x.ImgId);
                    table.ForeignKey(
                        name: "FK_imagemPostagem_Postagem_PostagemId",
                        column: x => x.PostagemId,
                        principalTable: "Postagem",
                        principalColumn: "PostagemId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_comentarioPostagem_PostagemId",
                table: "comentarioPostagem",
                column: "PostagemId");

            migrationBuilder.CreateIndex(
                name: "IX_imagemPostagem_PostagemId",
                table: "imagemPostagem",
                column: "PostagemId");

            migrationBuilder.CreateIndex(
                name: "IX_Postagem_UsuarioId",
                table: "Postagem",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "comentarioPostagem");

            migrationBuilder.DropTable(
                name: "imagemPostagem");

            migrationBuilder.DropTable(
                name: "Postagem");

            migrationBuilder.DropColumn(
                name: "Cidade",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ImgPerfil",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "NomeCompleto",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Reputacao",
                table: "AspNetUsers");
        }
    }
}
