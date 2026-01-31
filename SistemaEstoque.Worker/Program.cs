using Microsoft.EntityFrameworkCore;
using SistemaBase.Shared.Services;
using SistemaEstoque.Worker;
using SistemaEstoque.Worker.Database.Context;
using SistemaEstoque.Worker.Database.Repositories;
using SistemaEstoque.Worker.Interfaces;
using SistemaEstoque.Worker.UseCases;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.AddDbContext<EstoqueDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IEstoqueRepository, EstoqueRepository>();
builder.Services.AddScoped<IPedidoProcessadoRepository, PedidoProcessadoRepository>();
builder.Services.AddSingleton<IKafkaProducerService, KafkaProducerService>();
builder.Services.AddScoped<BaixarEstoqueUseCase>();
builder.Services.AddLogging();

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<EstoqueDbContext>();
    await db.Database.MigrateAsync();
}

host.Run();
