using ClinicaMedica.Domain.Entities;
using ClinicaMedica.Domain.Interfaces;
using ClinicaMedica.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicaMedica.Infrastructure.Repositories;

public sealed class ProfesionalRepository(AppDbContext contexto) : IProfesionalRepository
{
    public async Task AgregarAsync(Profesional profesional, CancellationToken cancellationToken = default)
    {
        await contexto.Profesionales.AddAsync(profesional, cancellationToken);
        await contexto.SaveChangesAsync(cancellationToken);
    }

    public async Task ActualizarAsync(Profesional profesional, CancellationToken cancellationToken = default)
    {
        contexto.Profesionales.Update(profesional);
        await contexto.SaveChangesAsync(cancellationToken);
    }

    public Task<Profesional?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        contexto.Profesionales
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Profesional>> ObtenerTodosActivosAsync(CancellationToken cancellationToken = default)
    {
        return await contexto.Profesionales
            .AsNoTracking()
            .Where(p => p.Activo)
            .OrderBy(p => p.Apellido)
            .ThenBy(p => p.Nombre)
            .ToListAsync(cancellationToken);
    }

    public Task<bool> ExisteMatriculaAsync(string numeroMatricula, CancellationToken cancellationToken = default) =>
        contexto.Profesionales.AnyAsync(p => p.NumeroMatricula == numeroMatricula, cancellationToken);

    public Task<bool> ExisteCorreoAsync(string correoElectronico, CancellationToken cancellationToken = default) =>
        contexto.Profesionales.AnyAsync(p => p.CorreoElectronico == correoElectronico, cancellationToken);

    public Task<bool> ExisteMatriculaEnOtroProfesionalAsync(
        string numeroMatricula, Guid idProfesional, CancellationToken cancellationToken = default) =>
        contexto.Profesionales.AnyAsync(
            p => p.NumeroMatricula == numeroMatricula && p.Id != idProfesional, cancellationToken);

    public Task<bool> ExisteCorreoEnOtroProfesionalAsync(
        string correoElectronico, Guid idProfesional, CancellationToken cancellationToken = default) =>
        contexto.Profesionales.AnyAsync(
            p => p.CorreoElectronico == correoElectronico && p.Id != idProfesional, cancellationToken);
}
