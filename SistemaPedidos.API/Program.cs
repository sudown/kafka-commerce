using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using SistemaPedidos.API.BackgroundServices;
using SistemaPedidos.API.Config;
using SistemaPedidos.API.Config.Extensions;
using SistemaPedidos.API.Services;
using SistemaPedidos.API.UseCases;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<SagaPedidoWorker>();
builder.Services.AddControllers();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddScoped<CriarPedidoUseCase>();
builder.Services.AddScoped<ObterPedidoPorIdUseCase>();


var app = builder.Build();

app.UseSwaggerDocumentation(app.Environment);
app.UseHttpsRedirection();
app.MapControllers();

app.Run();