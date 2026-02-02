using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaBase.Shared
{
    public class PedidoEvent
    {
        public Guid EventId { get;  set; } = Guid.NewGuid();
        public Guid PedidoId { get;  set; }
        public Guid ClienteId { get;  set; }
        public decimal ValorTotal { get;  set; }
        public PedidoStatusValue Status { get; set; } = new(PedidoStatusEnum.PROCESSANDO);
        public DateTime DataCriacao { get; set; }

        public List<PedidoItemEvent> Itens { get;  set; } = new();
        public string VersaoSchema { get;  set; } = "v1";
    }

    public enum PedidoStatusEnum
    {
        PROCESSANDO = 1,
        APROVADO = 2,
        CANCELADO_SEM_ESTOQUE = 3
    }

    public record PedidoStatusValue
    {
        public PedidoStatusEnum Valor { get; init; }

        public PedidoStatusValue(PedidoStatusEnum valor) => Valor = valor;

        // Construtor vazio para o Serializador (pode ser privado em alguns casos, mas público é mais seguro aqui)
        public PedidoStatusValue() { }

        public override string ToString() => Valor.ToString();

        // Açúcar sintático: permite fazer: PedidoStatusValue status = PedidoStatusEnum.APROVADO;
        public static implicit operator PedidoStatusValue(PedidoStatusEnum e) => new(e);
    }

    public class PedidoItemEvent
    {
        public Guid ProdutoId { get;  set; }
        public int Quantidade { get;  set; }
        public decimal PrecoUnitario { get;  set; }
    }
}
