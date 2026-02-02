using SistemaBase.Shared;

namespace SistemaPedidos.API.Services
{
    public interface IPedidoRepository
    {
        Task CriarAsync(PedidoEvent pedido);
        Task<PedidoEvent?> BuscarPorIdAsync(Guid pedidoId);
        Task AtualizarStatusAsync(Guid pedidoId, PedidoStatusEnum status);
    }
}
