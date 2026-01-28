using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaBase.Shared
{
    public class PedidoEvent
    {
        // Identificador único do evento (útil para rastreamento/logs)
        public Guid EventId { get;  set; } = Guid.NewGuid();

        // Dados do negócio
        public Guid PedidoId { get;  set; }
        public Guid ClienteId { get;  set; }
        public decimal ValorTotal { get;  set; }
        public DateTime DataCriacao { get;  set; }

        // Lista simples de itens (apenas o essencial)
        public List<PedidoItemEvent> Itens { get;  set; } = new();
        public string VersaoSchema { get;  set; } = "v1";
    }

    public class PedidoItemEvent
    {
        public Guid ProdutoId { get;  set; }
        public int Quantidade { get;  set; }
        public decimal PrecoUnitario { get;  set; }
    }
}
