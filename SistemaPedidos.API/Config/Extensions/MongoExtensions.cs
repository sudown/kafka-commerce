using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;

namespace SistemaPedidos.API.Config.Extensions
{
    public static class MongoExtensions
    {
        public static IServiceCollection AddMongoDb(this IServiceCollection services, IConfiguration configuration)
        {
            // Serializer para Guid
            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

            // Convenção para ignorar campos extras no MongoDB
            var conventionPack = new ConventionPack
            {
                new IgnoreExtraElementsConvention(true)
            };

            ConventionRegistry.Register(
                "IgnoreExtraElements",
                conventionPack,
                type => true
            );

            // Bind das configurações do MongoDB
            services.Configure<MongoDbSettings>(
                configuration.GetSection("MongoDbSettings"));

            return services;
        }
    }

}
