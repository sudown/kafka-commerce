using Confluent.Kafka;
using SistemaBase.Shared;
using SistemaPedidos.API.Services;
using System.Text;
using System.Text.Json;

namespace SistemaPedidos.API.BackgroundServices
{
    public class SagaEstornoWorker : BackgroundService
    {
        private readonly IConsumer<string, string> _consumer;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SagaEstornoWorker> _logger;

        public SagaEstornoWorker(IConfiguration config, IServiceProvider serviceProvider, ILogger<SagaEstornoWorker> logger)
        {
            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = config["Kafka:BootstrapServers"],
                GroupId = "saga-estorno-group",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            };
            _consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();
            _consumer.Subscribe("pedidos-estoque-insuficiente");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = _consumer.Consume(stoppingToken);
                    if (result == null) continue;

                    // Extrair CorrelationId para manter o rastro
                    var correlationBytes = result.Message.Headers.FirstOrDefault(h => h.Key == "CorrelationId")?.GetValueBytes();
                    var correlationId = correlationBytes != null ? Encoding.UTF8.GetString(correlationBytes) : "N/A";

                    using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
                    {
                        var pedidoErro = JsonSerializer.Deserialize<PedidoEvent>(result.Message.Value);

                        using (var scope = _serviceProvider.CreateScope())
                        {
                            var repository = scope.ServiceProvider.GetRequiredService<IPedidoRepository>();

                            _logger.LogWarning("Estornando pedido {PedidoId} por falta de estoque.", pedidoErro.PedidoId);

                            await repository.AtualizarStatusAsync(pedidoErro.PedidoId, PedidoStatusEnum.CANCELADO_SEM_ESTOQUE);
                            _consumer.Commit(result);
                        }
                    }
                }
                catch (OperationCanceledException) { break; }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro no worker de Saga.");
                }
            }
        }
    }
}
