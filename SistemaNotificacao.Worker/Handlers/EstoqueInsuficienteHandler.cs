using SistemaNotificacao.Worker.DTOs;
using SistemaNotificacao.Worker.Enums;
using SistemaNotificacao.Worker.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaNotificacao.Worker.Handlers
{
    public sealed class EstoqueInsuficienteHandler : IPedidoNotificacaoHandler
    {
        private readonly ILogger<EstoqueInsuficienteHandler> _logger;
        private readonly IEmailService _emailService;

        public EstoqueInsuficienteHandler(
            ILogger<EstoqueInsuficienteHandler> logger,
            IEmailService emailService)
        {
            _logger = logger;
            _emailService = emailService;
        }

        public bool CanHandle(string topic)
            => topic == "pedidos-estoque-insuficiente";

        public async Task HandleAsync(PedidoNotificacaoContext context)
        {
            var pedido = context.Pedido;

            _logger.LogWarning(
                "Pedido {PedidoId} com estoque insuficiente.",
                pedido.PedidoId);

            await _emailService.EnviarAsync(
                EmailTemplate.EstoqueInsuficiente,
                pedido
            );
        }
    }

}
