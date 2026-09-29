using ClinicaMedica.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClinicaMedica.Infrastructure.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Profesional> Profesionales => Set<Profesional>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Profesional>(entidad =>
        {
            entidad.ToTable("Profesionales");

            entidad.HasKey(p => p.Id);

            entidad.Property(p => p.Nombre).IsRequired().HasMaxLength(100);
            entidad.Property(p => p.Apellido).IsRequired().HasMaxLength(100);
            entidad.Property(p => p.NumeroMatricula).HasMaxLength(50);
            entidad.Property(p => p.Especialidad).HasMaxLength(100);
            entidad.Property(p => p.CorreoElectronico).IsRequired().HasMaxLength(200);
            entidad.Property(p => p.Telefono).HasMaxLength(30);
            entidad.Property(p => p.Activo).IsRequired();
            entidad.Property(p => p.FechaAlta).IsRequired();
            entidad.Property(p => p.FechaBaja);

            entidad.Property(p => p.TipoProfesional)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            entidad.HasIndex(p => p.Activo);

            entidad.HasIndex(p => p.CorreoElectronico).IsUnique();

            // Única solo cuando hay matrícula (recepcionistas, etc. no la tienen)
            entidad.HasIndex(p => p.NumeroMatricula)
                .IsUnique()
                .HasFilter("\"NumeroMatricula\" IS NOT NULL");
        });
    }
}
