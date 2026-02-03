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
    public sealed class PedidoAceitoHandler : IPedidoNotificacaoHandler
    {
        private readonly ILogger<PedidoAceitoHandler> _logger;
        private readonly IEmailService _emailService;

        public PedidoAceitoHandler(
            ILogger<PedidoAceitoHandler> logger,
            IEmailService emailService)
        {
            _logger = logger;
            _emailService = emailService;
        }

        public bool CanHandle(string topic)
            => topic == "pedidos-realizados";

        public async Task HandleAsync(PedidoNotificacaoContext context)
        {
            var pedido = context.Pedido;

            _logger.LogInformation(
                "Pedido {PedidoId} aceito e em processamento.",
                pedido.PedidoId);

            await _emailService.EnviarAsync(
                EmailTemplate.PedidoEmProcessamento,
                pedido
            );
        }
    }

}
