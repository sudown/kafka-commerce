using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaEstoque.Worker.Entities
{
    public class Produto
    {
        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public int QuantidadeEstoque { get; private set; }
        public Produto(Guid id, string nome, int quantidadeEstoque)
        {
            if (quantidadeEstoque < 0)
                throw new ArgumentException("A quantidade em estoque não pode ser negativa.");
            Id = id;
            Nome = nome;
            QuantidadeEstoque = quantidadeEstoque;
        }

        public void BaixarEstoque(int quantidade)
        {
            if (quantidade <= 0)
                throw new InvalidOperationException("A quantidade a ser baixada deve ser maior que zero.");
            if (QuantidadeEstoque < quantidade)
                throw new InvalidOperationException("Estoque insuficiente para a operação.");
            QuantidadeEstoque -= quantidade;
        }
    }
}
