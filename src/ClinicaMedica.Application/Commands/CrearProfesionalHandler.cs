using ClinicaMedica.Application.Common;
using ClinicaMedica.Application.DTOs;
using ClinicaMedica.Domain.Entities;
using ClinicaMedica.Domain.Interfaces;
using MediatR;

namespace ClinicaMedica.Application.Commands;

public sealed class CrearProfesionalHandler(IProfesionalRepository repositorio)
    : IRequestHandler<CrearProfesionalCommand, Result<ProfesionalDto>>
{
    public async Task<Result<ProfesionalDto>> Handle(
        CrearProfesionalCommand request,
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

        if (matricula is not null
            && await repositorio.ExisteMatriculaAsync(matricula, cancellationToken))
            return Result.Fallo<ProfesionalDto>(
                Error.Conflicto("Profesional.MatriculaDuplicada",
                    $"Ya existe un profesional con la matrícula '{matricula}'."));

        if (await repositorio.ExisteCorreoAsync(correo, cancellationToken))
            return Result.Fallo<ProfesionalDto>(
                Error.Conflicto("Profesional.CorreoDuplicado",
                    $"Ya existe un profesional con el correo '{correo}'."));

        var profesional = Profesional.Crear(
            request.Nombre,
            request.Apellido,
            request.TipoProfesional,
            correo,
            matricula,
            request.Especialidad,
            request.Telefono);

        await repositorio.AgregarAsync(profesional, cancellationToken);

        return Result.Exito(ProfesionalDto.DesdeEntidad(profesional));
    }
}
