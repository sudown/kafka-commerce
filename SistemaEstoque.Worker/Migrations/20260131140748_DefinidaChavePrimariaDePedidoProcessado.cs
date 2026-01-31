using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaEstoque.Worker.Migrations
{
    /// <inheritdoc />
    public partial class DefinidaChavePrimariaDePedidoProcessado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PedidosProcessados",
                columns: table => new
                {
                    PedidoId = table.Column<Guid>(type: "uuid", nullable: false),
                    DataProcessamento = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PedidosProcessados", x => x.PedidoId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PedidosProcessados");
        }
    }
}
