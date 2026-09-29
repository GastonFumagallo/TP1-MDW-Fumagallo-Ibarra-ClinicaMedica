using ClinicaMedica.Domain.Enums;

namespace ClinicaMedica.Application.DTOs;

/// <summary>Cuerpo de la solicitud PUT. El Id viaja en la ruta.</summary>
public sealed record ActualizarProfesionalDto(
    string Nombre,
    string Apellido,
    TipoProfesional TipoProfesional,
    string CorreoElectronico,
    string? NumeroMatricula = null,
    string? Especialidad = null,
    string? Telefono = null);
