using Confluent.Kafka;
using SistemaBase.Shared;
using SistemaPedidos.API.Services;
using System.Text;
using System.Text.Json;

namespace SistemaPedidos.API.BackgroundServices
{
    public class SagaPedidoWorker : BackgroundService
    {
        private readonly IConsumer<string, string> _consumer;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SagaPedidoWorker> _logger;

        public SagaPedidoWorker(IConfiguration config, IServiceProvider serviceProvider, ILogger<SagaPedidoWorker> logger)
        {
            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = config["Kafka:BootstrapServers"],
                GroupId = "saga-pedido-group",
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
            _consumer.Subscribe(new[] {
                "pedidos-estoque-confirmado",
                "pedidos-estoque-insuficiente"
            });

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = _consumer.Consume(stoppingToken);
                    if (result == null) continue;

                    var correlationBytes = result.Message.Headers.FirstOrDefault(h => h.Key == "CorrelationId")?.GetValueBytes();
                    var correlationId = correlationBytes != null ? Encoding.UTF8.GetString(correlationBytes) : "N/A";

                    using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
                    {
                        var evento = JsonSerializer.Deserialize<PedidoEvent>(result.Message.Value);

                        var novoStatus = result.Topic switch
                        {
                            "pedidos-estoque-confirmado" => PedidoStatusEnum.APROVADO,
                            "pedidos-estoque-insuficiente" => PedidoStatusEnum.CANCELADO_SEM_ESTOQUE,
                            _ => PedidoStatusEnum.PROCESSANDO
                        };

                        await AtualizarStatusNoBanco(evento.PedidoId, novoStatus);

                        _consumer.Commit(result);
                        _logger.LogInformation("Saga: Pedido {PedidoId} atualizado para {Status}", evento.PedidoId, novoStatus);
                    }
                }
                catch (OperationCanceledException) { break; }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro no worker de Saga.");
                }
            }
        }
        private async Task AtualizarStatusNoBanco(Guid pedidoId, PedidoStatusEnum status)
        {
            using var scope = _serviceProvider.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IPedidoRepository>();

            await repository.AtualizarStatusAsync(pedidoId, status);
        }

        public override void Dispose()
        {
            _consumer.Close();
            _consumer.Dispose();
            base.Dispose();
        }
    }
}
