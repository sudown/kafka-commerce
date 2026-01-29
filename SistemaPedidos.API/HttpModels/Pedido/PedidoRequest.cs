namespace SistemaPedidos.API.HttpModels.Pedido
{
    public record CriarPedidoRequest(
         Guid ClienteId,
         List<PedidoItemRequest> Itens
    );

    public record PedidoItemRequest(
        Guid ProdutoId,
        int Quantidade,
        decimal PrecoUnitario
    );
}
