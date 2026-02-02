using SistemaBase.Shared;
using SistemaBase.Shared.Services;
using SistemaPedidos.API.HttpModels;
using SistemaPedidos.API.HttpModels.Pedido;
using SistemaPedidos.API.Services;

namespace SistemaPedidos.API.UseCases
{
    public class CriarPedidoUseCase
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IKafkaProducerService _kafkaService;
        private readonly ILogger<CriarPedidoUseCase> _logger;

        public CriarPedidoUseCase(
            IPedidoRepository pedidoRepository,
            IKafkaProducerService kafkaService,
            ILogger<CriarPedidoUseCase> logger)
        {
            _pedidoRepository = pedidoRepository;
            _kafkaService = kafkaService;
            _logger = logger;
        }

        public async Task<ResultPattern<CriarPedidoResult>> ExecutarAsync(CriarPedidoRequest request)
        {
            var correlationId = Guid.NewGuid();

            var pedido = new PedidoEvent
            {
                PedidoId = Guid.NewGuid(),
                ClienteId = request.ClienteId,
                DataCriacao = DateTime.UtcNow,
                Itens = request.Itens.Select(i => new PedidoItemEvent
                {
                    ProdutoId = i.ProdutoId,
                    Quantidade = i.Quantidade,
                    PrecoUnitario = i.PrecoUnitario
                }).ToList(),
                ValorTotal = request.Itens.Sum(i => i.Quantidade * i.PrecoUnitario)
            };

            await _pedidoRepository.CriarAsync(pedido);

            var headers = new Dictionary<string, string>
            {
                { "CorrelationId", correlationId.ToString() }
            };

            _logger.LogInformation(
                "[{CorrelationId}] Pedido criado para o cliente {ClienteId}",
                correlationId,
                pedido.ClienteId
            );

            await _kafkaService.PublicarAsync(
                "pedidos-realizados",
                pedido,
                headers
            );

            return ResultPattern<CriarPedidoResult>.SuccessResult(
                new CriarPedidoResult
                (
                    correlationId,
                    pedido.PedidoId
                )
            );
        }
    }

}
