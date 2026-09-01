using GestionComplejo2C.Presentation.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddScoped<ICanchaService, CanchaService>();

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
