using ClinicaMedica.Application.Common;
using ClinicaMedica.Application.DTOs;
using MediatR;

namespace ClinicaMedica.Application.Queries;

/// <summary>Lista los profesionales activos (los dados de baja no se incluyen).</summary>
public sealed record ObtenerTodosProfesionalesQuery : IRequest<Result<IReadOnlyList<ProfesionalDto>>>;
