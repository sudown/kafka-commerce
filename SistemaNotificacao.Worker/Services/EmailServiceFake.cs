using SistemaBase.Shared;
using SistemaNotificacao.Worker.Enums;
using SistemaNotificacao.Worker.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaNotificacao.Worker.Services
{
    public sealed class EmailServiceFake : IEmailService
    {
        public Task EnviarAsync(EmailTemplate template, PedidoEvent pedido)
        {
            var message = template switch
            {
                EmailTemplate.EstoqueInsuficiente =>
                $"Pedido {pedido.PedidoId} sem estoque disponível.",

                EmailTemplate.PedidoEmProcessamento =>
                    $"Pedido {pedido.PedidoId} foi aceito e está em processamento.",

                EmailTemplate.PedidoConfirmado =>
                    $"Pedido {pedido.PedidoId} confirmado no valor de {pedido.ValorTotal:C}.",

                _ => "Notificação desconhecida"
            };

            Console.WriteLine($"[E-MAIL] {message}");
            return Task.CompletedTask;
        }
    }
}
