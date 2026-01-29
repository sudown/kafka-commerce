using Microsoft.AspNetCore.Mvc;
using SistemaBase.Shared;
using SistemaPedidos.API.HttpModels.Pedido;
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
        public async Task<IActionResult> CriarPedido([FromBody] CriarPedidoRequest request, [FromServices] IPedidoRepository pedidoRepository)
        {
            var pedidoEvent = new PedidoEvent
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

            //Aqui você salvaria no MongoDB (Próximo passo!)
            await pedidoRepository.CriarAsync(pedidoEvent);

            await _kafkaService.SendPedidoEventAsync(pedidoEvent);

            return Ok(new
            {
                Id = pedidoEvent.PedidoId,
                Message = "Pedido recebido com sucesso!"
            });
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
