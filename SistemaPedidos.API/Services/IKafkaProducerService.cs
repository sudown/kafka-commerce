using SistemaBase.Shared;

namespace SistemaPedidos.API.Services
{
    public interface IKafkaProducerService
    {
        Task SendPedidoEventAsync(PedidoEvent pedido);
    }
}
