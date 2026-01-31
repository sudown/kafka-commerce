using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;
using static Confluent.Kafka.ConfigPropertyNames;

namespace SistemaBase.Shared.Services
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
                Acks = Acks.All,
                MessageSendMaxRetries = 3
            };

            _producer = new ProducerBuilder<string, string>(config).Build();
        }

        public Task PublicarAsync<T>(string topic, T message, Dictionary<string, string>? headers = null)
        {
            var payload = JsonSerializer.Serialize<T>(message);
            var kafkaMessage = new Message<string, string>
            {
                Key = Guid.NewGuid().ToString(),
                Value = payload,
                Headers = []
            };

            if(headers != null)
            {
                foreach (var header in headers)
                {
                    kafkaMessage.Headers.Add(header.Key, Encoding.UTF8.GetBytes(header.Value));
                }
            }

            return _producer.ProduceAsync(topic, kafkaMessage);
        }

        public async Task SendPedidoEventAsync(PedidoEvent pedido, Dictionary<string, string>? headers = null)
        {
            var topic = "pedidos-realizados";
            var payload = JsonSerializer.Serialize<PedidoEvent>(pedido);

            var message = new Message<string, string>
            {
                Key = pedido.PedidoId.ToString(),
                Value = payload,
                Headers = []
            };

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    message.Headers.Add(header.Key, Encoding.UTF8.GetBytes(header.Value));
                }
            }

            await _producer.ProduceAsync(topic, message);
        }
    }
}
