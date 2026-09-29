using ClinicaMedica.Domain.Entities;

namespace ClinicaMedica.Domain.Interfaces;

public interface IProfesionalRepository
{
    Task AgregarAsync(Profesional profesional, CancellationToken cancellationToken = default);
    Task ActualizarAsync(Profesional profesional, CancellationToken cancellationToken = default);

    Task<Profesional?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Profesional>> ObtenerTodosActivosAsync(CancellationToken cancellationToken = default);

    Task<bool> ExisteMatriculaAsync(string numeroMatricula, CancellationToken cancellationToken = default);
    Task<bool> ExisteCorreoAsync(string correoElectronico, CancellationToken cancellationToken = default);

    Task<bool> ExisteMatriculaEnOtroProfesionalAsync(string numeroMatricula, Guid idProfesional, CancellationToken cancellationToken = default);
    Task<bool> ExisteCorreoEnOtroProfesionalAsync(string correoElectronico, Guid idProfesional, CancellationToken cancellationToken = default);
}
