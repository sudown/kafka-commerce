using SistemaBase.Shared;
using SistemaPedidos.API.HttpModels;
using SistemaPedidos.API.HttpModels.Pedido;
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

        public async Task<ResultPattern<ObterPedidoEventDTO>> ExecutarAsync(Guid pedidoId)
        {
            try
            {
                var pedido = await _pedidoRepository.BuscarPorIdAsync(pedidoId);

                if (pedido == null)
                    return ResultPattern<ObterPedidoEventDTO>.NotFound("Pedido não encontrado.");

                var dto = new ObterPedidoEventDTO(pedido);

                return ResultPattern<ObterPedidoEventDTO>.SuccessResult(dto);
            }
            catch (Exception ex)
            {
                return ResultPattern<ObterPedidoEventDTO>.InternalError("Erro interno.");
            }
        }
    }
}
