using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecommenduWeb.Data.Migrations
{
    /// <inheritdoc />
    public partial class EntidadeRegistroLike : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RegistroCurtida",
                columns: table => new
                {
                    CurtidaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostagemId = table.Column<int>(type: "int", nullable: false),
                    DtCurtida = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistroCurtida", x => x.CurtidaId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegistroCurtida");

            migrationBuilder.DropColumn(
                name: "Cidade",
                table: "PostagemServico");

            migrationBuilder.DropColumn(
                name: "Titulo",
                table: "PostagemServico");

            migrationBuilder.DropColumn(
                name: "Titulo",
                table: "PostagemProduto");

            migrationBuilder.RenameColumn(
                name: "Estado",
                table: "PostagemServico",
                newName: "NomeServico");

            migrationBuilder.AddColumn<string>(
                name: "Modelo",
                table: "PostagemProduto",
                type: "nvarchar(max)",
                nullable: true);

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
        }
    }
}
