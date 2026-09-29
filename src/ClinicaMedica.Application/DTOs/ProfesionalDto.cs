using ClinicaMedica.Domain.Entities;
using ClinicaMedica.Domain.Enums;

namespace ClinicaMedica.Application.DTOs;

public sealed record ProfesionalDto(
    Guid Id,
    string Nombre,
    string Apellido,
    string? NumeroMatricula,
    TipoProfesional TipoProfesional,
    string? Especialidad,
    string CorreoElectronico,
    string? Telefono,
    bool Activo,
    DateTime FechaAlta,
    DateTime? FechaBaja)
{
    public static ProfesionalDto DesdeEntidad(Profesional p) => new(
        p.Id,
        p.Nombre,
        p.Apellido,
        p.NumeroMatricula,
        p.TipoProfesional,
        p.Especialidad,
        p.CorreoElectronico,
        p.Telefono,
        p.Activo,
        p.FechaAlta,
        p.FechaBaja);
}
