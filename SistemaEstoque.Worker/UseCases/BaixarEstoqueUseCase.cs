using SistemaBase.Shared;
using SistemaEstoque.Worker.Entities;
using SistemaEstoque.Worker.Interfaces;
using SistemaEstoque.Worker.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SistemaEstoque.Worker.UseCases
{
    internal class BaixarEstoqueUseCase(ILogger<BaixarEstoqueUseCase> logger, IEstoqueRepository EstoqueRepository, IPedidoProcessadoRepository pedidoProcessadoRepository)
    {
        private readonly ILogger<BaixarEstoqueUseCase> _logger = logger;
        private readonly IEstoqueRepository _repository = EstoqueRepository;
        private readonly IPedidoProcessadoRepository _pedidoProcessadoRepository = pedidoProcessadoRepository;

        public async Task<ProcessamentoEstoqueResult> ExecutarAsync(PedidoEvent pedido)
        {
            var pedidoJaProcessado = await _pedidoProcessadoRepository.PedidoJaProcessadoAsync(pedido.PedidoId);
            if(pedidoJaProcessado)
            {
                _logger.LogWarning("[IDEMPOTENCIA] Pedido {PedidoId} já foi processado anteriormente. Ignorando.", pedido.PedidoId);
                return new ProcessamentoEstoqueResult(true);
            }

            try
            {
                foreach (var item in pedido.Itens)
                {
                    var produto = await _repository.BuscarPorIdAsync(item.ProdutoId);

                    if (produto == null)
                    {
                        return new ProcessamentoEstoqueResult(false, $"Produto {item.ProdutoId} não encontrado", true);
                    }

                    try
                    {
                        produto.BaixarEstoque(item.Quantidade);
                    }
                    catch (ArgumentException)
                    {
                        return new ProcessamentoEstoqueResult(false, "Estoque insuficiente", true);
                    }

                    await _pedidoProcessadoRepository.RegistrarProcessamentoAsync(new PedidoProcessado(pedido.PedidoId));
                    _repository.Atualizar(produto);
                }

                await _repository.SalvaAsync();
                await MostrarPainelEstoque();
                return new ProcessamentoEstoqueResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[TÉCNICO] Falha de infraestrutura no pedido {PedidoId}", pedido.PedidoId);
                throw; // Lança para o Worker capturar no Polly
            }
        }

        private async Task MostrarPainelEstoque()
        {
            var estoqueAtualizado = await _repository.ObterTodosAsync();

            var tabela = "\n" +
                "==================================================\n" +
                "       PAINEL DE CONTROLE DE ESTOQUE (POSTGRES)   \n" +
                "==================================================\n" +
                " PRODUTO              | QTD ATUAL \n" +
                "--------------------------------------------------\n";

            foreach (var p in estoqueAtualizado)
            {
                tabela += $" {p.Nome.PadRight(20)} | {p.QuantidadeEstoque.ToString().PadLeft(8)} \n";
            }

            tabela += "==================================================";

            _logger.LogInformation(tabela);
        }
    }
}
