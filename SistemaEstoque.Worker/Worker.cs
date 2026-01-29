using Confluent.Kafka;
using Polly;
using Polly.Retry;
using SistemaBase.Shared;
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

            // Criando escopo para usar o Repository Scoped
            using (var scope = _scopeFactory.CreateScope())
            {
                var baixarEstoqueUseCase = scope.ServiceProvider.GetRequiredService<BaixarEstoqueUseCase>();
                var pedido = JsonSerializer.Deserialize<PedidoEvent>(result.Message.Value);

                await _retryPolicy.ExecuteAsync(async () =>
                {
                    var sucesso = await baixarEstoqueUseCase.ExecutarAsync(pedido);
                    if (sucesso)
                    {
                        _consumer.Commit(result);
                    }
                    else
                    {
                        // Se o UseCase retornou falso (ex: produto não existe), 
                        // talvez não adiante tentar de novo (erro de negócio).
                        // Aqui você decidiria se manda para a DLQ.
                        _logger.LogWarning($"Pedido {pedido.PedidoId} não pôde ser processado por regra de negócio.");
                        _consumer.Commit(result);
                    }
                });
            }
        }
    }
}