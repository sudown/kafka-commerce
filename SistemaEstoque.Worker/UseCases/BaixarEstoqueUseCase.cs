using SistemaBase.Shared;
using SistemaEstoque.Worker.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaEstoque.Worker.UseCases
{
    public class BaixarEstoqueUseCase(ILogger<BaixarEstoqueUseCase> logger, IEstoqueRepository repository)
    {
        private readonly ILogger<BaixarEstoqueUseCase> _logger = logger;
        private readonly IEstoqueRepository _repository = repository;

        public async Task<bool> ExecutarAsync(PedidoEvent pedido)
        {
            _logger.LogInformation($"Processando estoque do pedido {pedido.PedidoId}");
            bool sucesso = true;
            foreach (var item in pedido.Itens)
            {
                var produtoEstoque = await _repository.BuscarPorIdAsync(item.ProdutoId);
                if (produtoEstoque == null)
                {
                    _logger.LogError($"Produto {item.ProdutoId} não encontrado no estoque.");
                    sucesso = false;
                    continue;
                }
                produtoEstoque.BaixarEstoque(item.Quantidade);
                _repository.Atualizar(produtoEstoque);
            }
            if (sucesso)
            {
                await _repository.SalvaAsync();
                _logger.LogInformation($"Estoque atualizado para o pedido {pedido.PedidoId}");
            }
            return sucesso;
        }
    }
}
