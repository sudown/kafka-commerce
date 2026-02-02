namespace SistemaPedidos.API.HttpModels.Pedido
{
    public sealed record CriarPedidoResult
    (
        Guid CorrelationId, 
        Guid PedidoId
    );
}
