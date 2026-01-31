using SistemaEstoque.Worker.Database.Context;
using SistemaEstoque.Worker.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaEstoque.Worker.Database.Repositories
{
    internal class UnitOfWork(EstoqueDbContext context) : IUnitOfWork
    {
        private readonly EstoqueDbContext _context = context;

        public Task SalvaAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
