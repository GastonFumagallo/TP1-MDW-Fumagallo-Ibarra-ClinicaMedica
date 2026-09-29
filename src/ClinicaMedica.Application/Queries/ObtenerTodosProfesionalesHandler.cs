using ClinicaMedica.Application.Common;
using ClinicaMedica.Application.DTOs;
using ClinicaMedica.Domain.Interfaces;
using MediatR;

namespace ClinicaMedica.Application.Queries;

public sealed class ObtenerTodosProfesionalesHandler(IProfesionalRepository repositorio)
    : IRequestHandler<ObtenerTodosProfesionalesQuery, Result<IReadOnlyList<ProfesionalDto>>>
{
    public async Task<Result<IReadOnlyList<ProfesionalDto>>> Handle(
        ObtenerTodosProfesionalesQuery request,
        CancellationToken cancellationToken)
    {
        var profesionales = await repositorio.ObtenerTodosActivosAsync(cancellationToken);

        IReadOnlyList<ProfesionalDto> dtos = profesionales
            .Select(ProfesionalDto.DesdeEntidad)
            .ToList();

        return Result.Exito(dtos);
    }
}
