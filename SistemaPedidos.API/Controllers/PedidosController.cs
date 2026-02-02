using Microsoft.AspNetCore.Mvc;
using SistemaBase.Shared;
using SistemaBase.Shared.Services;
using SistemaPedidos.API.HttpModels.Pedido;
using SistemaPedidos.API.Services;
using SistemaPedidos.API.UseCases;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SistemaPedidos.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PedidosController : BaseController
    {
        private readonly IKafkaProducerService _kafkaService;
        private readonly ILogger<PedidosController> _logger;

        public PedidosController(IKafkaProducerService kafkaProducerService, ILogger<PedidosController> logger)
        {
            _kafkaService = kafkaProducerService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CriarPedido([FromBody] CriarPedidoRequest request, [FromServices] CriarPedidoUseCase criarPedidoUseCase)
        {
            var result = await criarPedidoUseCase.ExecutarAsync(request);
            return ProcessResult(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> ObterPorId(Guid id, [FromServices] ObterPedidoPorIdUseCase obterPedidoPorIdUseCase)
        {
            var result = await obterPedidoPorIdUseCase.ExecutarAsync(id);
            return ProcessResult(result);
        }
    }
}
