using Confluent.Kafka;
using Polly;
using Polly.Retry;
using SistemaBase.Shared;
using System.Text;
using System.Text.Json;

namespace SistemaNotificacao.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConsumer<string, string> _consumer;
    private const string Topic = "pedidos-realizados";
    private readonly AsyncRetryPolicy _retryPolicy;
    private readonly IProducer<string, string> _dlqProducer;

    public Worker(ILogger<Worker> logger, IConfiguration configuration)
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
            GroupId = "notificacao-group", // Identifica este microsserviço
            AutoOffsetReset = AutoOffsetReset.Earliest, // Lê desde o início se for novo
            EnableAutoCommit = false // Vamos controlar o "OK" manualmente (mais seguro)
        };

        _consumer = new ConsumerBuilder<string, string>(config).Build();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _consumer.Subscribe(Topic);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Tenta ler uma mensagem por 1 segundo
                var consumeResult = _consumer.Consume(stoppingToken);

                if (consumeResult != null)
                {
                    await _retryPolicy.ExecuteAsync(async () =>
                    {
                        var pedido = JsonSerializer.Deserialize<PedidoEvent>(consumeResult.Message.Value);

                        _logger.LogInformation($"Processando notificação do pedido {pedido.PedidoId}...");

                        // SIMULAÇÃO DE ERRO: 
                        // Se você quiser testar o retry, descomente a linha abaixo:
                        //throw new Exception("Serviço de e-mail fora do ar!");

                        await EnviarEmailFake(pedido);

                        // Só fazemos o Commit se o Polly chegar até aqui com sucesso
                        _consumer.Commit(consumeResult);
                        _logger.LogInformation($"Pedido {pedido.PedidoId} processado e confirmado.");
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao processar mensagem: {ex.Message}");
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
