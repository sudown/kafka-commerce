using SistemaBase.Shared;
using SistemaNotificacao.Worker.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaNotificacao.Worker.Interfaces
{
    public interface IPedidoNotificacaoHandler
    {
        bool CanHandle(string topic);
        Task HandleAsync(PedidoNotificacaoContext context);
    }
}
