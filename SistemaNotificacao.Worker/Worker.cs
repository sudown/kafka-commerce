using Confluent.Kafka;
using SistemaBase.Shared;
using System.Text.Json;

namespace SistemaNotificacao.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConsumer<string, string> _consumer;
    private const string Topic = "pedidos-realizados";

    public Worker(ILogger<Worker> logger, IConfiguration configuration)
    {
        _logger = logger;
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
                    var pedido = JsonSerializer.Deserialize<PedidoEvent>(consumeResult.Message.Value);

                    _logger.LogInformation("--- NOTIFICAÇÃO ---");
                    _logger.LogInformation($"Enviando e-mail para o cliente do pedido: {pedido?.PedidoId}");

                    // Simula um processamento (envio de e-mail)
                    await Task.Delay(1000, stoppingToken);

                    // Confirma que a mensagem foi processada com sucesso
                    _consumer.Commit(consumeResult);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao processar mensagem: {ex.Message}");
            }
        }
    }

    public override void Dispose()
    {
        _consumer.Close();
        _consumer.Dispose();
        base.Dispose();
    }
}
