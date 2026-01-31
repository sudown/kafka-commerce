using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SistemaBase.Shared;
using SistemaPedidos.API.Config;

namespace SistemaPedidos.API.Services
{
    public class PedidoRepository : IPedidoRepository
    {
        private readonly IMongoCollection<PedidoEvent> _pedidos;

        public PedidoRepository(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _pedidos = database.GetCollection<PedidoEvent>(settings.Value.CollectionName);
        }

        public async Task CriarAsync(PedidoEvent pedido)
        {
            await _pedidos.InsertOneAsync(pedido);
        }

        public async Task<PedidoEvent?> BuscarPorIdAsync(Guid pedidoId)
        {
            return await _pedidos.Find(p => p.PedidoId == pedidoId).FirstOrDefaultAsync();
        }

        public async Task AtualizarStatusAsync(Guid pedidoId, string novoStatus)
        {
            var filter = Builders<PedidoEvent>.Filter.Eq(p => p.PedidoId, pedidoId);
            var update = Builders<PedidoEvent>.Update.Set(p => p.Status, novoStatus);

            await _pedidos.UpdateOneAsync(filter, update);
        }
    }
}
