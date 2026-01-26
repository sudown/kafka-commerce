using Microsoft.AspNetCore.Mvc;
using SistemaBase.Shared;
using SistemaPedidos.API.Services;

namespace SistemaPedidos.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PedidosController : ControllerBase
    {
        private readonly IKafkaProducerService _kafkaService;

        public PedidosController(IKafkaProducerService kafkaProducerService)
        {
            _kafkaService = kafkaProducerService;
        }

        [HttpPost]
        public async Task<IActionResult> CriarPedido([FromBody] PedidoEvent pedido)
        {
            // Aqui você salvaria no Banco de Dados primeiro...

            // Dispara o evento para o Kafka
            await _kafkaService.SendPedidoEventAsync(pedido);

            return Ok(new { message = "Pedido enviado para processamento!" });
        }
    }
}
