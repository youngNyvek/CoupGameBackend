using Application.Services;
using Domain.Interfaces;
using Infra.Repositories;
using WebApi.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSignalR();

// Registra o repositório e o serviço
builder.Services.AddSingleton<ISessionRepository, InMemorySessionRepository>();
builder.Services.AddTransient<SessionService>();
builder.Services.AddTransient<GameActionsService>();
builder.Services.AddTransient<GameManagerService>();

// Configuração do CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:4173", "https://orange-banks-sip.loca.lt") // URL do frontend
           .AllowAnyHeader() // Permite qualquer cabeçalho
           .AllowAnyMethod() // Permite qualquer método HTTP (GET, POST, etc.)
           .AllowCredentials(); // Permite cookies e autenticação
    });
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors(); // Ativa o CORS antes dos endpoints

app.UseAuthorization();

app.MapHub<SessionHub>("/sessionHub");

app.Run();
