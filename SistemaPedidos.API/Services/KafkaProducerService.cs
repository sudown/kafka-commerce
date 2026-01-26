using Confluent.Kafka;
using SistemaBase.Shared;
using System.Text.Json;
using static Confluent.Kafka.ConfigPropertyNames;

namespace SistemaPedidos.API.Services
{
    public class KafkaProducerService : IKafkaProducerService
    {
        private readonly IConfiguration _configuration;
        private readonly IProducer<string, string> _producer;

        public KafkaProducerService(IConfiguration configuration)
        {
            _configuration = configuration;

            var config = new ProducerConfig
            {
                BootstrapServers = _configuration["Kafka:BootstrapServers"],
                // Garante que a mensagem chegue ao broker (Ack All)
                Acks = Acks.All,
                // Tenta reenviar em caso de falha de rede temporária
                MessageSendMaxRetries = 3
            };

            _producer = new ProducerBuilder<string, string>(config).Build();
        }

        public async Task SendPedidoEventAsync(PedidoEvent pedido)
        {
            var topic = "pedidos-realizados";
            var payload = JsonSerializer.Serialize<PedidoEvent>(pedido);

            var message = new Message<string, string>
            {
                // Usar o PedidoId como Key garante ordem na partição
                Key = pedido.PedidoId.ToString(),
                Value = payload
            };

            await _producer.ProduceAsync(topic, message);
        }
    }
}
