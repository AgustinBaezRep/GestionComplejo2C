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

builder.Services.AddScoped<IRepositorioCanchas, RepositorioCanchas>();
builder.Services.AddScoped<IRepositorioVestuarios, RepositorioVestuarios>();
builder.Services.AddScoped<IRepositorioServicios, RepositorioServicios>();

builder.Services.AddScoped<ICanchaService, CanchaService>();
builder.Services.AddScoped<IReservaService, ReservaService>();
builder.Services.AddScoped<IVestuarioService, VestuarioService>();
builder.Services.AddScoped<IServicioService, ServicioService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
