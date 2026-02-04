using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using static Confluent.Kafka.ConfigPropertyNames;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;

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

        public async Task PublicarAsync<T>(string topico, T mensagem, Dictionary<string, string>? headers = null)
        {
            var kafkaMessage = new Message<string, string>
            {
                Key = Guid.NewGuid().ToString(),
                Value = JsonSerializer.Serialize(mensagem),
                Headers = new Headers()
            };

            // 1. Injetar o Contexto do OpenTelemetry nos Headers do Kafka
            var activityContext = Activity.Current?.Context ?? default;
            Propagators.DefaultTextMapPropagator.Inject(new PropagationContext(activityContext, Baggage.Current),
                kafkaMessage.Headers,
                (headers, key, value) => headers.Add(key, Encoding.UTF8.GetBytes(value)));

            // 2. Adicionar seus headers manuais (como o CorrelationId)
            if (headers != null)
            {
                foreach (var h in headers) kafkaMessage.Headers.Add(h.Key, Encoding.UTF8.GetBytes(h.Value));
            }

            await _producer.ProduceAsync(topico, kafkaMessage);
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
