using GestionComplejo2C.Application.Interfaces;
using GestionComplejo2C.Application.Services;
using GestionComplejo2C.Domain.Interfaces;
using GestionComplejo2C.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Infrastructure: la implementacion concreta se elige aca, en el arranque.
// Singleton porque el repositorio en memoria guarda el estado del proceso.
builder.Services.AddSingleton<IRepositorioCanchas, RepositorioCanchas>();

// Application: casos de uso.
builder.Services.AddScoped<ICanchaService, CanchaService>();
builder.Services.AddScoped<IReservaService, ReservaService>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
