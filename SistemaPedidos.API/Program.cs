using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using SistemaPedidos.API.Config;
using SistemaPedidos.API.Config.Extensions;
using SistemaPedidos.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseSwaggerDocumentation(app.Environment);
app.UseHttpsRedirection();
app.MapControllers();

app.Run();