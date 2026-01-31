using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SistemaEstoque.Worker.Migrations
{
    /// <inheritdoc />
    public partial class SeedProdutos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Produtos",
                columns: new[] { "Id", "Nome", "QuantidadeEstoque" },
                values: new object[,]
                {
                    { new Guid("a1b2c3d4-e5f6-4a5b-8c9d-0e1f2a3b4c5d"), "Mouse Gamer 12000 DPI", 100 },
                    { new Guid("d290f1ee-6c54-4b01-90e6-d701748f0851"), "Teclado Mecânico RGB", 50 },
                    { new Guid("f1234567-89ab-4cde-0123-456789abcdef"), "Monitor 144Hz", 20 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Produtos",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-4a5b-8c9d-0e1f2a3b4c5d"));

            migrationBuilder.DeleteData(
                table: "Produtos",
                keyColumn: "Id",
                keyValue: new Guid("d290f1ee-6c54-4b01-90e6-d701748f0851"));

            migrationBuilder.DeleteData(
                table: "Produtos",
                keyColumn: "Id",
                keyValue: new Guid("f1234567-89ab-4cde-0123-456789abcdef"));
        }
    }
}
