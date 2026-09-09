using GestionComplejo2C.Application.Interfaces;
using GestionComplejo2C.Application.Services;
using GestionComplejo2C.Domain.Interfaces;
using GestionComplejo2C.Infrastructure.Persistence;
using GestionComplejo2C.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("GestionComplejoDb")
    ?? throw new InvalidOperationException("Falta la connection string 'GestionComplejoDb' en appsettings.json.");

builder.Services.AddDbContext<GestionComplejoDbContext>(options =>
    options.UseSqlServer(connectionString));

// Infrastructure: la implementacion concreta se elige aca, en el arranque.
builder.Services.AddScoped<IRepositorioCanchas, RepositorioCanchas>();

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
