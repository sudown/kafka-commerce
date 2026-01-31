using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaEstoque.Worker.Entities
{
    public class PedidoProcessado
    {
        public Guid PedidoId { get; private set; }
        public DateTimeOffset DataProcessamento { get; private set; }

        public PedidoProcessado(Guid pedidoId)
        {
            PedidoId = pedidoId;
            DataProcessamento = DateTimeOffset.UtcNow;
        }
    }
}
