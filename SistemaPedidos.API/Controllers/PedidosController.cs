using Microsoft.AspNetCore.Mvc;
using SistemaBase.Shared;
using SistemaBase.Shared.Services;
using SistemaPedidos.API.HttpModels.Pedido;
using SistemaPedidos.API.Services;

namespace SistemaPedidos.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PedidosController : ControllerBase
    {
        private readonly IKafkaProducerService _kafkaService;
        private readonly ILogger<PedidosController> _logger;

        public PedidosController(IKafkaProducerService kafkaProducerService, ILogger<PedidosController> logger)
        {
            _kafkaService = kafkaProducerService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CriarPedido([FromBody] CriarPedidoRequest request, [FromServices] IPedidoRepository pedidoRepository)
        {
            var correlationId = Guid.NewGuid().ToString();
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

            await pedidoRepository.CriarAsync(pedido);

            var headers = new Dictionary<string, string>
            {
                { "CorrelationId", correlationId }
            };

            _logger.LogInformation("[{CorrelationId}] Recebendo novo pedido para o cliente {ClienteId}", correlationId, pedido.ClienteId);

            await _kafkaService.PublicarAsync("pedidos-realizados", pedido, headers);

            return Accepted(new { CorrelationId = correlationId });
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> ObterPorId(Guid id, [FromServices] IPedidoRepository pedidoRepository)
        {
            var pedido = await pedidoRepository.BuscarPorIdAsync(id);

            if (pedido == null)
            {
                return NotFound(new { Message = $"Pedido {id} não encontrado no MongoDB." });
            }

            return Ok(pedido);
        }
    }
}
