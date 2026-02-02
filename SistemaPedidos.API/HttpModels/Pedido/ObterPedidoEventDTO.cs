using SistemaBase.Shared;

namespace SistemaPedidos.API.HttpModels.Pedido
{
    public sealed record ObterPedidoEventDTO
    {
        public Guid EventId { get; init; }
        public Guid PedidoId { get; init; }
        public Guid ClientId { get; init; }
        public decimal ValorTotal { get; init; }
        public string Status { get; init; } = default!;
        public DateTime DataCriacao { get; init; }
        public string VersaoSchema { get; init; } = default!;
        public IReadOnlyCollection<ObterPedidoItemEventDTO> Itens { get; init; } = [];

        public ObterPedidoEventDTO(PedidoEvent pedidoEvent)
        {
            EventId = pedidoEvent.EventId;
            PedidoId = pedidoEvent.PedidoId;
            ClientId = pedidoEvent.ClienteId;
            ValorTotal = pedidoEvent.ValorTotal;
            Status = pedidoEvent.Status.ToString();
            DataCriacao = pedidoEvent.DataCriacao;
            VersaoSchema = pedidoEvent.VersaoSchema;
            Itens = pedidoEvent.Itens
                .Select(i => new ObterPedidoItemEventDTO(i))
                .ToList();
        }
    }
    public sealed record ObterPedidoItemEventDTO
    {
        public Guid ProdutoId { get; init; }
        public int Quantidade { get; init; }
        public decimal PrecoUnitario { get; init; }

        public ObterPedidoItemEventDTO(PedidoItemEvent pedidoItem)
        {
            ProdutoId = pedidoItem.ProdutoId;
            Quantidade = pedidoItem.Quantidade;
            PrecoUnitario = pedidoItem.PrecoUnitario;
        }
    }
}
