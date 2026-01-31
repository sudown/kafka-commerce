using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaEstoque.Worker.Migrations
{
    /// <inheritdoc />
    public partial class RemovendoPropriedadePrecoUnitario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrecoUnitario",
                table: "Produtos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PrecoUnitario",
                table: "Produtos",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
