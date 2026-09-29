using System.Text.Json.Serialization;
using ClinicaMedica.Application;
using ClinicaMedica.Infrastructure;
using ClinicaMedica.Infrastructure.Data;
using ClinicaMedica.Presentation.Middlewares;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Capas
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Presentación
builder.Services
    .AddControllers()
    .AddJsonOptions(opciones =>
        opciones.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opciones =>
{
    opciones.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Clínica Médica - API de Profesionales",
        Version = "v1",
        Description = "Gestión de profesionales de una clínica médica (Clean Architecture, CQRS, Result)."
    });
});

// Manejo global de errores
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Crea la base de datos si no existe (desarrollo). Con migraciones, reemplazar por Database.Migrate().
using (var scope = app.Services.CreateScope())
{
    var contexto = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    contexto.Database.EnsureCreated();
}

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
