using Microsoft.EntityFrameworkCore;
using SistemaEstoque.Worker.Database.Context;
using SistemaEstoque.Worker.Entities;
using SistemaEstoque.Worker.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaEstoque.Worker.Database.Repositories
{
    internal class PedidoProcessadoRepository(EstoqueDbContext context) : IPedidoProcessadoRepository
    {
        private readonly EstoqueDbContext _context = context;
        public async Task<bool> PedidoJaProcessadoAsync(Guid pedidoId)
        {
            return await _context.PedidosProcessados
                .AnyAsync(p => p.PedidoId == pedidoId);
        }

        public async Task RegistrarProcessamentoAsync(PedidoProcessado registro)
        {
            await _context.PedidosProcessados.AddAsync(registro);
        }
    }
}
