using ClinicaMedica.Application.Common;
using MediatR;

namespace ClinicaMedica.Application.Commands;

/// <summary>Baja lógica de un profesional (no borra el registro).</summary>
public sealed record EliminarProfesionalCommand(Guid Id) : IRequest<Result>;
