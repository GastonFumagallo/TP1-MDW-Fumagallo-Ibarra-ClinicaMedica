using ClinicaMedica.Application.Common;
using ClinicaMedica.Application.DTOs;
using ClinicaMedica.Domain.Interfaces;
using MediatR;

namespace ClinicaMedica.Application.Commands;

public sealed class ActualizarProfesionalHandler(IProfesionalRepository repositorio)
    : IRequestHandler<ActualizarProfesionalCommand, Result<ProfesionalDto>>
{
    public async Task<Result<ProfesionalDto>> Handle(
        ActualizarProfesionalCommand request,
        CancellationToken cancellationToken)
    {
        var validacion = ValidadorProfesional.Validar(
            request.Nombre,
            request.Apellido,
            request.TipoProfesional,
            request.CorreoElectronico,
            request.NumeroMatricula,
            request.Especialidad);

        if (validacion.EsFallido)
            return Result.Fallo<ProfesionalDto>(validacion.Error);

        var (correo, matricula) = validacion.Valor;

        var profesional = await repositorio.ObtenerPorIdAsync(request.Id, cancellationToken);
        if (profesional is null)
            return Result.Fallo<ProfesionalDto>(
                Error.NoEncontrado("Profesional.NoEncontrado",
                    $"No existe un profesional con Id '{request.Id}'."));

        if (!profesional.Activo)
            return Result.Fallo<ProfesionalDto>(
                Error.Conflicto("Profesional.Inactivo",
                    "No se puede modificar un profesional dado de baja."));

        if (matricula is not null
            && await repositorio.ExisteMatriculaEnOtroProfesionalAsync(matricula, request.Id, cancellationToken))
            return Result.Fallo<ProfesionalDto>(
                Error.Conflicto("Profesional.MatriculaDuplicada",
                    $"Ya existe otro profesional con la matrícula '{matricula}'."));

        if (await repositorio.ExisteCorreoEnOtroProfesionalAsync(correo, request.Id, cancellationToken))
            return Result.Fallo<ProfesionalDto>(
                Error.Conflicto("Profesional.CorreoDuplicado",
                    $"Ya existe otro profesional con el correo '{correo}'."));

        profesional.Actualizar(
            request.Nombre,
            request.Apellido,
            request.TipoProfesional,
            correo,
            matricula,
            request.Especialidad,
            request.Telefono);

        await repositorio.ActualizarAsync(profesional, cancellationToken);

        return Result.Exito(ProfesionalDto.DesdeEntidad(profesional));
    }
}
