using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaEstoque.Worker.Entities
{
    internal class Produto
    {
        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public int QuantidadeEstoque { get; private set; }
        public int PrecoUnitario { get; private set; }
        public Produto(Guid id, string nome, int quantidadeEstoque, int precoUnitario)
        {
            if (quantidadeEstoque < 0)
                throw new ArgumentException("A quantidade em estoque não pode ser negativa.");
            if (precoUnitario < 0)
                throw new ArgumentException("A quantidade em estoque não pode ser negativa.");
            Id = id;
            Nome = nome;
            QuantidadeEstoque = quantidadeEstoque;
            PrecoUnitario = precoUnitario;

        }
    }
}
