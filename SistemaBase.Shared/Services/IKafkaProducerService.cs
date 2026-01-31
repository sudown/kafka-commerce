namespace SistemaBase.Shared.Services
{
    public interface IKafkaProducerService
    {
        Task SendPedidoEventAsync(PedidoEvent pedido, Dictionary<string, string>? headers = null);
        Task PublicarAsync<T>(string topic, T message, Dictionary<string, string>? headers = null);
    }
}
