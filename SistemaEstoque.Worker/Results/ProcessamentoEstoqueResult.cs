using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaEstoque.Worker.Results
{
    public record ProcessamentoEstoqueResult(bool Sucesso, string? MensagemErro = null, bool ErroNegocio = false);
}
