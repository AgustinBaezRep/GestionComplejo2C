using System.Text;
using GestionComplejo2C.Application.Interfaces;
using GestionComplejo2C.Application.Services;
using GestionComplejo2C.Domain.Interfaces;
using GestionComplejo2C.Infrastructure.ExternalServices;
using GestionComplejo2C.Infrastructure.Persistence;
using GestionComplejo2C.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("GestionComplejoDb")
    ?? throw new InvalidOperationException("Falta la connection string 'GestionComplejoDb' en appsettings.json.");

builder.Services.AddDbContext<GestionComplejoDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<IRepositorioCanchas, RepositorioCanchas>();
builder.Services.AddScoped<IRepositorioVestuarios, RepositorioVestuarios>();
builder.Services.AddScoped<IRepositorioServicios, RepositorioServicios>();
builder.Services.AddScoped<IRepositorioUsuarios, RepositorioUsuarios>();

builder.Services.AddScoped<ICanchaService, CanchaService>();
builder.Services.AddScoped<IReservaService, ReservaService>();
builder.Services.AddScoped<IVestuarioService, VestuarioService>();
builder.Services.AddScoped<IServicioService, ServicioService>();
builder.Services.AddScoped<IAutenticacionService, AutenticacionService>();
builder.Services.AddScoped<IServicioToken, ServicioTokenJwt>();
builder.Services.AddScoped<IServicioHashPassword, ServicioHashPasswordBCrypt>();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SeccionConfiguracion));

var jwtSettings = builder.Configuration.GetSection(JwtSettings.SeccionConfiguracion).Get<JwtSettings>()
    ?? throw new InvalidOperationException("Falta la seccion 'Jwt' en appsettings.json.");

if (string.IsNullOrWhiteSpace(jwtSettings.Key))
{
    throw new InvalidOperationException("Falta la clave 'Jwt:Key' en appsettings.json.");
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,

            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Pegue aqui el token devuelto por POST /api/auth/login (sin la palabra Bearer)."
    });

    options.AddSecurityRequirement(documento => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference(JwtBearerDefaults.AuthenticationScheme, documento)] = new List<string>()
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
