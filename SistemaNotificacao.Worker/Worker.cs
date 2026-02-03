using Confluent.Kafka;
using Polly;
using Polly.Retry;
using SistemaBase.Shared;
using SistemaNotificacao.Worker.DTOs;
using SistemaNotificacao.Worker.Interfaces;
using System.Text;
using System.Text.Json;

namespace SistemaNotificacao.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConsumer<string, string> _consumer;
    private static readonly string[] Topics = ["pedidos-realizados", "pedidos-estoque-confirmado", "pedidos-estoque-insuficiente"];
    private readonly AsyncRetryPolicy _retryPolicy;
    private readonly IServiceScopeFactory _scopeFactory;


    public Worker(ILogger<Worker> logger, IConfiguration configuration, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;

        _retryPolicy = Policy
        .Handle<Exception>()
        .WaitAndRetryAsync(3, retryAttempt =>
            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
            (exception, timeSpan, retryCount, context) =>
            {
                _logger.LogWarning($"Falha temporária: Tentativa {retryCount}. Re-tentando em {timeSpan.Seconds}s. Erro: {exception.Message}");
            });

        var config = new ConsumerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"],
            GroupId = "notificacao-group",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        _consumer = new ConsumerBuilder<string, string>(config).Build();
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _consumer.Subscribe(Topics);

        while (!stoppingToken.IsCancellationRequested)
        {
            var consumeResult = _consumer.Consume(stoppingToken);

            var correlationBytes = consumeResult.Message.Headers.FirstOrDefault(h => h.Key == "CorrelationId")?.GetValueBytes();
            var correlationId = correlationBytes != null ? Encoding.UTF8.GetString(correlationBytes) : "N/A";

            if (consumeResult != null) continue;

            using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
            {
                try
                {
                    await _retryPolicy.ExecuteAsync(async () =>
                    {
                        var pedido = JsonSerializer.Deserialize<PedidoEvent>(
                            consumeResult!.Message.Value)!;

                        var context = new PedidoNotificacaoContext
                        {
                            Topic = consumeResult.Topic,
                            CorrelationId = correlationId,
                            Pedido = pedido
                        };

                        _logger.LogInformation(
                            "Processando evento {Topic} para o pedido {PedidoId}",
                            context.Topic,
                            pedido.PedidoId
                        );

                        using var scope = _scopeFactory.CreateScope();

                        var handlers = scope.ServiceProvider
                            .GetRequiredService<IEnumerable<IPedidoNotificacaoHandler>>();

                        var handler = handlers.FirstOrDefault(h => h.CanHandle(context.Topic));

                        if (handler is null)
                        {
                            _logger.LogWarning(
                                "Nenhum handler registrado para o tópico {Topic}",
                                context.Topic);
                            return;
                        }

                        await handler.HandleAsync(context);

                        _consumer.Commit(consumeResult);

                        _logger.LogInformation(
                            "Evento do pedido {PedidoId} processado com sucesso.",
                            pedido.PedidoId
                        );
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Erro ao processar mensagem: {ex.Message}");
                }
            }
        }
    }

    public static Task EnviarEmailFake(PedidoEvent pedido)
    {
        // Simula o envio de e-mail
        Console.WriteLine($"[E-MAIL] Pedido {pedido.PedidoId} para o cliente {pedido.ClienteId} no valor de {pedido.ValorTotal:C} foi recebido.");
        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _consumer.Close();
        _consumer.Dispose();
        base.Dispose();
    }
}
