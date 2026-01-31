using Confluent.Kafka;
using Polly;
using Polly.Retry;
using SistemaBase.Shared;
using SistemaBase.Shared.Services;
using SistemaEstoque.Worker.Interfaces;
using SistemaEstoque.Worker.UseCases;
using System.Text.Json;

namespace SistemaEstoque.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceScopeFactory _scopeFactory; // Essencial para Singletons
    private readonly IConsumer<string, string> _consumer;
    private readonly AsyncRetryPolicy _retryPolicy;

    public Worker(ILogger<Worker> logger, IConfiguration configuration, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _retryPolicy = Policy
        .Handle<Exception>()
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        var config = new ConsumerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"],
            GroupId = "estoque-group",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };
        _consumer = new ConsumerBuilder<string, string>(config).Build();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _consumer.Subscribe("pedidos-realizados");

        while (!stoppingToken.IsCancellationRequested)
        {
            var result = _consumer.Consume(stoppingToken);
            if (result == null) continue;

            // Dentro do ExecuteAsync, no loop do Consumer...
            using var scope = _scopeFactory.CreateScope();
            var useCase = scope.ServiceProvider.GetRequiredService<BaixarEstoqueUseCase>();
            var kafkaProducer = scope.ServiceProvider.GetRequiredService<IKafkaProducerService>();
            var pedido = JsonSerializer.Deserialize<PedidoEvent>(result.Message.Value);

            try
            {
                await _retryPolicy.ExecuteAsync(async () =>
                {
                    var resultado = await useCase.ExecutarAsync(pedido);

                    if (resultado.Sucesso)
                    {
                        _logger.LogInformation($"[SUCESSO] Pedido {pedido.PedidoId} processado.");
                    }
                    else if (resultado.ErroNegocio)
                    {
                        // CENTRALIZADO: Erro de negócio vai para o tópico de compensação
                        _logger.LogWarning($"[NEGÓCIO] Desviando pedido {pedido.PedidoId} para estorno: {resultado.MensagemErro}");
                        await kafkaProducer.PublicarAsync("pedidos-estoque-insuficiente", pedido);
                    }

                    _consumer.Commit(result);
                });
            }
            catch (Exception ex)
            {
                // CENTRALIZADO: Após 3 tentativas de infra, vai para a DLQ Técnica
                _logger.LogCritical($"[DLQ] Falha técnica definitiva no pedido {pedido.PedidoId}");
                await kafkaProducer.PublicarAsync("pedidos-erro-tecnico", pedido);
                _consumer.Commit(result);
            }
        }
    }
}
