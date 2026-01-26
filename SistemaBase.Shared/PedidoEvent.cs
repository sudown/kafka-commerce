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
        public Guid EventId { get; private set; } = Guid.NewGuid();

        // Dados do negócio
        public Guid PedidoId { get; private set; }
        public int ClienteId { get; private set; }
        public decimal ValorTotal { get; private set; }
        public DateTime DataCriacao { get; private set; }

        // Lista simples de itens (apenas o essencial)
        public List<PedidoItemEvent> Itens { get; private set; } = new();
        public string VersaoSchema { get; private set; } = "v1";
    }

    public class PedidoItemEvent
    {
        public int ProdutoId { get; private set; }
        public int Quantidade { get; private set; }
        public decimal PrecoUnitario { get; private set; }
    }
}
