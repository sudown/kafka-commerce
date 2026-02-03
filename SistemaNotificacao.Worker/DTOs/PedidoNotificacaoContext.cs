using SistemaBase.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaNotificacao.Worker.DTOs
{
    public sealed record PedidoNotificacaoContext
    {
        public string Topic { get; init; } = default!;
        public string CorrelationId { get; init; } = default!;
        public PedidoEvent Pedido { get; init; } = default!;
    }
}
