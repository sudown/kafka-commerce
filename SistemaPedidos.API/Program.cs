using SistemaPedidos.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Adiciona apenas o necessário para o Swagger funcionar
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IKafkaProducerService, KafkaProducerService>();

var app = builder.Build();

// Remove o bloco try-catch após testar, ou mantenha apenas o if
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers(); // Agora o Reflection vai carregar tudo sem conflitos

app.Run();