using System.Net.Mail;
using ClinicaMedica.Domain.Enums;

namespace ClinicaMedica.Application.Common;

public sealed record DatosProfesionalNormalizados(string CorreoElectronico, string? NumeroMatricula);

/// <summary>Reglas de validación compartidas por los comandos de alta y modificación.</summary>
internal static class ValidadorProfesional
{
    public static Result<DatosProfesionalNormalizados> Validar(
        string? nombre,
        string? apellido,
        TipoProfesional tipoProfesional,
        string? correoElectronico,
        string? numeroMatricula,
        string? especialidad)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            return Result.Fallo<DatosProfesionalNormalizados>(
                Error.Validacion("Profesional.NombreRequerido", "El nombre es obligatorio."));

        if (string.IsNullOrWhiteSpace(apellido))
            return Result.Fallo<DatosProfesionalNormalizados>(
                Error.Validacion("Profesional.ApellidoRequerido", "El apellido es obligatorio."));

        if (!Enum.IsDefined(tipoProfesional))
            return Result.Fallo<DatosProfesionalNormalizados>(
                Error.Validacion("Profesional.TipoInvalido", "El tipo de profesional no es válido."));

        var correo = correoElectronico?.Trim().ToLowerInvariant() ?? string.Empty;
        if (!MailAddress.TryCreate(correo, out _))
            return Result.Fallo<DatosProfesionalNormalizados>(
                Error.Validacion("Profesional.CorreoInvalido", "El correo electrónico no tiene un formato válido."));

        var matricula = string.IsNullOrWhiteSpace(numeroMatricula)
            ? null
            : numeroMatricula.Trim().ToUpperInvariant();

        var requiereMatricula = tipoProfesional is TipoProfesional.Medico or TipoProfesional.Enfermero;
        if (requiereMatricula && matricula is null)
            return Result.Fallo<DatosProfesionalNormalizados>(
                Error.Validacion("Profesional.MatriculaRequerida",
                    "Médicos y enfermeros deben informar el número de matrícula."));

        if (tipoProfesional == TipoProfesional.Medico && string.IsNullOrWhiteSpace(especialidad))
            return Result.Fallo<DatosProfesionalNormalizados>(
                Error.Validacion("Profesional.EspecialidadRequerida",
                    "Los médicos deben informar su especialidad."));

        return Result.Exito(new DatosProfesionalNormalizados(correo, matricula));
    }
}
