using ClinicaMedica.Application.Common;
using ClinicaMedica.Application.DTOs;
using ClinicaMedica.Domain.Enums;
using MediatR;

namespace ClinicaMedica.Application.Commands;

public sealed record CrearProfesionalCommand(
    string Nombre,
    string Apellido,
    TipoProfesional TipoProfesional,
    string CorreoElectronico,
    string? NumeroMatricula = null,
    string? Especialidad = null,
    string? Telefono = null) : IRequest<Result<ProfesionalDto>>;
