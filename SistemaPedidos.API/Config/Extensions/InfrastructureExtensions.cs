using System.Runtime.CompilerServices;

namespace SistemaPedidos.API.Config.Extensions
{
    public static class InfrastructureExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMongoDb(configuration);
            services.AddApplicationServices();
            services.AddSwaggerDocumentation();
            return services;
        }
    }
}
