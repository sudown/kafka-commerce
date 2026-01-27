using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaEstoque.Worker.Entities
{
    internal class Pedido
    {
        public Guid PedidoId { get; set; }
        private readonly List<Produto> _itens = [];
        public IReadOnlyList<Produto> Items => _itens.AsReadOnly();

        public void AddProduto(Produto produto)
        {
            _itens.Add(produto);
        }

    }
}
