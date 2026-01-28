using Confluent.Kafka;
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

    public Worker(ILogger<Worker> logger, IConfiguration configuration, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;

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

                _logger.LogInformation($"Processando estoque do pedido {pedido.PedidoId}");

                await baixarEstoqueUseCase.ExecutarAsync(pedido);
                _consumer.Commit(result);
            }
        }
    }
}