using ClinicaMedica.Application.Common;
using ClinicaMedica.Application.DTOs;
using MediatR;

namespace ClinicaMedica.Application.Queries;

public sealed record ObtenerProfesionalPorIdQuery(Guid Id) : IRequest<Result<ProfesionalDto>>;
