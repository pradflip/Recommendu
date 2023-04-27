using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecommenduWeb.Data.Migrations
{
    /// <inheritdoc />
    public partial class RolesSeedETreino : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TreinoML",
                columns: table => new
                {
                    Valor = table.Column<string>(type: "nvarchar(1)", nullable: false),
                    Texto = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreinoML", x => new { x.Valor, x.Texto });
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "1", null, "Admin", "ADMIN" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TreinoML");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1");

            migrationBuilder.DropColumn(
                name: "DtComentario",
                table: "ComentarioPostagem");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "ComentarioPostagem");
        }
    }
}
