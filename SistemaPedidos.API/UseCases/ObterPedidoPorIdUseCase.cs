using SistemaBase.Shared;
using SistemaPedidos.API.HttpModels;
using SistemaPedidos.API.Services;

namespace SistemaPedidos.API.UseCases
{
    public class ObterPedidoPorIdUseCase
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly ILogger<ObterPedidoPorIdUseCase> _logger;

        public ObterPedidoPorIdUseCase(IPedidoRepository pedidoRepository, ILogger<ObterPedidoPorIdUseCase> logger)
        {
            _pedidoRepository = pedidoRepository;
            _logger = logger;
        }

        public async Task<ResultPattern<PedidoEvent>> ExecutarAsync(Guid pedidoId)
        {
            try
            {
                var pedido = await _pedidoRepository.BuscarPorIdAsync(pedidoId);
                if (pedido == null)
                    return ResultPattern<PedidoEvent>.NotFound("Pedido não encontrado.");

                return ResultPattern<PedidoEvent>.SuccessResult(pedido);
            } catch (Exception ex)
            {
                return ResultPattern<PedidoEvent>.InternalError("Erro ao obter o pedido: " + ex.Message);
            }
        } 
    }
}
