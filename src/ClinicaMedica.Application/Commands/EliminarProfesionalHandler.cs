using ClinicaMedica.Application.Common;
using ClinicaMedica.Domain.Interfaces;
using MediatR;

namespace ClinicaMedica.Application.Commands;

public sealed class EliminarProfesionalHandler(IProfesionalRepository repositorio)
    : IRequestHandler<EliminarProfesionalCommand, Result>
{
    public async Task<Result> Handle(
        EliminarProfesionalCommand request,
        CancellationToken cancellationToken)
    {
        var profesional = await repositorio.ObtenerPorIdAsync(request.Id, cancellationToken);
        if (profesional is null)
            return Result.Fallo(
                Error.NoEncontrado("Profesional.NoEncontrado",
                    $"No existe un profesional con Id '{request.Id}'."));

        // Idempotente: si ya estaba dado de baja no se vuelve a modificar.
        if (profesional.Activo)
        {
            profesional.DarDeBaja();
            await repositorio.ActualizarAsync(profesional, cancellationToken);
        }

        return Result.Exito();
    }
}
