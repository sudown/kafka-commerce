namespace SistemaBase.Shared.Services
{
    public interface IKafkaProducerService
    {
        Task SendPedidoEventAsync(PedidoEvent pedido);
        Task PublicarAsync<T>(string topic, T message);
    }
}
