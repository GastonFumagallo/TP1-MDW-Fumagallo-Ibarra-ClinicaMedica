using ClinicaMedica.Domain.Interfaces;
using ClinicaMedica.Infrastructure.Data;
using ClinicaMedica.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicaMedica.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var cadenaConexion = configuration.GetConnectionString("ClinicaDb")
            ?? throw new InvalidOperationException("Falta la cadena de conexión 'ClinicaDb'.");

        services.AddDbContext<AppDbContext>(opciones => opciones.UseSqlite(cadenaConexion));

        services.AddScoped<IProfesionalRepository, ProfesionalRepository>();

        return services;
    }
}
