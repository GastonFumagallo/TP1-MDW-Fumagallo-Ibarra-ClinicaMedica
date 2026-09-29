using ClinicaMedica.Application.Common;
using ClinicaMedica.Application.DTOs;
using ClinicaMedica.Domain.Interfaces;
using MediatR;

namespace ClinicaMedica.Application.Queries;

public sealed class ObtenerProfesionalPorIdHandler(IProfesionalRepository repositorio)
    : IRequestHandler<ObtenerProfesionalPorIdQuery, Result<ProfesionalDto>>
{
    public async Task<Result<ProfesionalDto>> Handle(
        ObtenerProfesionalPorIdQuery request,
        CancellationToken cancellationToken)
    {
        var profesional = await repositorio.ObtenerPorIdAsync(request.Id, cancellationToken);

        return profesional is null
            ? Result.Fallo<ProfesionalDto>(
                Error.NoEncontrado("Profesional.NoEncontrado", $"No existe un profesional con Id '{request.Id}'."))
            : Result.Exito(ProfesionalDto.DesdeEntidad(profesional));
    }
}
