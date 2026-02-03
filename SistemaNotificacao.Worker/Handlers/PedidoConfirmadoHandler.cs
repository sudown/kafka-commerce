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
    public sealed class PedidoConfirmadoHandler : IPedidoNotificacaoHandler
    {
        private readonly ILogger<PedidoConfirmadoHandler> _logger;
        private readonly IEmailService _emailService;

        public PedidoConfirmadoHandler(
            ILogger<PedidoConfirmadoHandler> logger,
            IEmailService emailService)
        {
            _logger = logger;
            _emailService = emailService;
        }

        public bool CanHandle(string topic)
            => topic == "pedidos-estoque-confirmado";

        public async Task HandleAsync(PedidoNotificacaoContext context)
        {
            var pedido = context.Pedido;

            _logger.LogInformation(
                "Pedido {PedidoId} confirmado com sucesso.",
                pedido.PedidoId);

            await _emailService.EnviarAsync(
                EmailTemplate.PedidoConfirmado,
                pedido
            );
        }
    }

}
