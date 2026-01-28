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
    public class EstoqueRepository(EstoqueDbContext context) : IEstoqueRepository
    {
        private readonly EstoqueDbContext _context = context;

        public void Atualizar(Produto produto)
        {
            _context.Produtos.Update(produto);
        }

        public async Task<Produto?> BuscarPorIdAsync(Guid id)
        {
            return await _context.Produtos.FindAsync(id);
        }

        public void CriarAsync(Produto produto)
        {
            _context.Produtos.Add(produto);
        }

        public Task SalvaAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
