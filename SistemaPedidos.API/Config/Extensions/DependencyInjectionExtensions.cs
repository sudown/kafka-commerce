using SistemaPedidos.API.Services;

namespace SistemaPedidos.API.Config.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddSingleton<IPedidoRepository, PedidoRepository>();
            services.AddSingleton<IKafkaProducerService, KafkaProducerService>();

            return services;
        }
    }

}
