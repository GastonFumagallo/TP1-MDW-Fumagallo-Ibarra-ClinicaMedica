using ClinicaMedica.Domain.Enums;

namespace ClinicaMedica.Domain.Entities;

public class Profesional
{
    public Guid Id { get; private set; }
    public string Nombre { get; private set; }
    public string Apellido { get; private set; }
    public string? NumeroMatricula { get; private set; }
    public TipoProfesional TipoProfesional { get; private set; }
    public string? Especialidad { get; private set; }
    public string CorreoElectronico { get; private set; }
    public string? Telefono { get; private set; }
    public bool Activo { get; private set; }
    public DateTime FechaAlta { get; private set; }
    public DateTime? FechaBaja { get; private set; }

    // Requerido por EF Core
    private Profesional()
    {
        Nombre = string.Empty;
        Apellido = string.Empty;
        CorreoElectronico = string.Empty;
    }

    public static Profesional Crear(
        string nombre,
        string apellido,
        TipoProfesional tipoProfesional,
        string correoElectronico,
        string? numeroMatricula = null,
        string? especialidad = null,
        string? telefono = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nombre);
        ArgumentException.ThrowIfNullOrWhiteSpace(apellido);
        ArgumentException.ThrowIfNullOrWhiteSpace(correoElectronico);

        return new Profesional
        {
            Id = Guid.NewGuid(),
            Nombre = nombre.Trim(),
            Apellido = apellido.Trim(),
            TipoProfesional = tipoProfesional,
            CorreoElectronico = correoElectronico.Trim(),
            NumeroMatricula = numeroMatricula?.Trim(),
            Especialidad = especialidad?.Trim(),
            Telefono = telefono?.Trim(),
            Activo = true,
            FechaAlta = DateTime.UtcNow,
            FechaBaja = null
        };
    }

    /// <summary>Modifica los datos del profesional. No permite modificar a un profesional dado de baja.</summary>
    public void Actualizar(
        string nombre,
        string apellido,
        TipoProfesional tipoProfesional,
        string correoElectronico,
        string? numeroMatricula = null,
        string? especialidad = null,
        string? telefono = null)
    {
        if (!Activo)
            throw new InvalidOperationException("No se puede modificar un profesional dado de baja.");

        ArgumentException.ThrowIfNullOrWhiteSpace(nombre);
        ArgumentException.ThrowIfNullOrWhiteSpace(apellido);
        ArgumentException.ThrowIfNullOrWhiteSpace(correoElectronico);

        Nombre = nombre.Trim();
        Apellido = apellido.Trim();
        TipoProfesional = tipoProfesional;
        CorreoElectronico = correoElectronico.Trim();
        NumeroMatricula = numeroMatricula?.Trim();
        Especialidad = especialidad?.Trim();
        Telefono = telefono?.Trim();
    }

    /// <summary>Baja lógica: marca al profesional como inactivo y registra la fecha de baja. Es idempotente.</summary>
    public void DarDeBaja()
    {
        if (!Activo)
            return;

        Activo = false;
        FechaBaja = DateTime.UtcNow;
    }

    /// <summary>Revierte la baja lógica.</summary>
    public void Reactivar()
    {
        Activo = true;
        FechaBaja = null;
    }
}
