using SistemaNotificacao.Worker;
using SistemaNotificacao.Worker.Handlers;
using SistemaNotificacao.Worker.Interfaces;
using SistemaNotificacao.Worker.Services;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddScoped<IPedidoNotificacaoHandler, EstoqueInsuficienteHandler>();
builder.Services.AddScoped<IPedidoNotificacaoHandler, PedidoAceitoHandler>();
builder.Services.AddScoped<IPedidoNotificacaoHandler, PedidoConfirmadoHandler>();
builder.Services.AddScoped<IEmailService, EmailServiceFake>();

var host = builder.Build();
host.Run();
