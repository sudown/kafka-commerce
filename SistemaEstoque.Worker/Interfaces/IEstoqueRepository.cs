using SistemaEstoque.Worker.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaEstoque.Worker.Interfaces
{
    internal interface IEstoqueRepository
    {
        void CriarAsync(Produto produto);
        void Atualizar(Produto produto);
        Task<Produto?> BuscarPorIdAsync(Guid id);
        Task SalvaAsync();
        Task<List<Produto>> ObterTodosAsync();
    }
}
