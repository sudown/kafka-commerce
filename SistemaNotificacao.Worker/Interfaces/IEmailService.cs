using SistemaBase.Shared;
using SistemaNotificacao.Worker.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaNotificacao.Worker.Interfaces
{
    public interface IEmailService
    {
        Task EnviarAsync(EmailTemplate template, PedidoEvent pedido);
    }
}
