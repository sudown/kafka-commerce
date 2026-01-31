using SistemaEstoque.Worker.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaEstoque.Worker.Interfaces
{
    internal interface IPedidoProcessadoRepository
    {
        Task<bool> PedidoJaProcessadoAsync(Guid pedidoId);
        Task RegistrarProcessamentoAsync(PedidoProcessado registro);
    }
}
