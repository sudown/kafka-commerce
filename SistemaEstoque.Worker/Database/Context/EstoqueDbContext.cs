using Microsoft.EntityFrameworkCore;
using SistemaEstoque.Worker.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;


namespace SistemaEstoque.Worker.Database.Context
{
    public class EstoqueDbContext : DbContext
    {
        public EstoqueDbContext(DbContextOptions<EstoqueDbContext> options) : base(options) { }

        public DbSet<Produto> Produtos { get; set; }
        public DbSet<PedidoProcessado> PedidosProcessados { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<PedidoProcessado>()
                .HasKey(p => p.PedidoId);

            var tecladoId = Guid.Parse("d290f1ee-6c54-4b01-90e6-d701748f0851");
            var mouseId = Guid.Parse("a1b2c3d4-e5f6-4a5b-8c9d-0e1f2a3b4c5d");
            var monitorId = Guid.Parse("f1234567-89ab-4cde-0123-456789abcdef");

            builder.Entity<Produto>().HasData(
                new Produto(tecladoId, "Teclado Mecânico RGB", 50),
                new Produto(mouseId, "Mouse Gamer 12000 DPI", 100),
                new Produto(monitorId, "Monitor 144Hz", 20)
            );
        }
    }
}
