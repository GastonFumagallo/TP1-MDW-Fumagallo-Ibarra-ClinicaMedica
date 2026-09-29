using ClinicaMedica.Application.Commands;
using ClinicaMedica.Application.Common;
using ClinicaMedica.Domain.Entities;
using ClinicaMedica.Domain.Enums;
using ClinicaMedica.Domain.Interfaces;
using Moq;
using Xunit;

namespace ClinicaMedica.Tests.Application.Tests;

public class CrearProfesionalHandlerTests
{
    private readonly Mock<IProfesionalRepository> _repositorio = new();
    private readonly CrearProfesionalHandler _handler;

    public CrearProfesionalHandlerTests()
    {
        _handler = new CrearProfesionalHandler(_repositorio.Object);
    }

    private static CrearProfesionalCommand ComandoMedicoValido() => new(
        Nombre: "Laura",
        Apellido: "Gómez",
        TipoProfesional: TipoProfesional.Medico,
        CorreoElectronico: "Laura.Gomez@clinica.com",
        NumeroMatricula: "mp-12345",
        Especialidad: "Cardiología",
        Telefono: "341-5551234");

    [Fact]
    public async Task Handle_ConDatosValidos_CreaElProfesionalYDevuelveExito()
    {
        var resultado = await _handler.Handle(ComandoMedicoValido(), CancellationToken.None);

        Assert.True(resultado.EsExitoso);
        Assert.Equal("Laura", resultado.Valor.Nombre);
        Assert.Equal("MP-12345", resultado.Valor.NumeroMatricula);
        Assert.Equal("laura.gomez@clinica.com", resultado.Valor.CorreoElectronico);
        Assert.True(resultado.Valor.Activo);

        _repositorio.Verify(
            r => r.AgregarAsync(It.IsAny<Profesional>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ConMatriculaDuplicada_DevuelveConflicto()
    {
        _repositorio
            .Setup(r => r.ExisteMatriculaAsync("MP-12345", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var resultado = await _handler.Handle(ComandoMedicoValido(), CancellationToken.None);

        Assert.True(resultado.EsFallido);
        Assert.Equal(TipoError.Conflicto, resultado.Error.Tipo);
        Assert.Equal("Profesional.MatriculaDuplicada", resultado.Error.Codigo);

        _repositorio.Verify(
            r => r.AgregarAsync(It.IsAny<Profesional>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ConCorreoDuplicado_DevuelveConflicto()
    {
        _repositorio
            .Setup(r => r.ExisteCorreoAsync("laura.gomez@clinica.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var resultado = await _handler.Handle(ComandoMedicoValido(), CancellationToken.None);

        Assert.True(resultado.EsFallido);
        Assert.Equal("Profesional.CorreoDuplicado", resultado.Error.Codigo);
    }

    [Fact]
    public async Task Handle_MedicoSinMatricula_DevuelveErrorDeValidacion()
    {
        var comando = ComandoMedicoValido() with { NumeroMatricula = null };

        var resultado = await _handler.Handle(comando, CancellationToken.None);

        Assert.True(resultado.EsFallido);
        Assert.Equal(TipoError.Validacion, resultado.Error.Tipo);
        Assert.Equal("Profesional.MatriculaRequerida", resultado.Error.Codigo);
    }

    [Fact]
    public async Task Handle_ConCorreoInvalido_DevuelveErrorDeValidacion()
    {
        var comando = ComandoMedicoValido() with { CorreoElectronico = "no-es-un-correo" };

        var resultado = await _handler.Handle(comando, CancellationToken.None);

        Assert.True(resultado.EsFallido);
        Assert.Equal("Profesional.CorreoInvalido", resultado.Error.Codigo);
    }

    [Fact]
    public async Task Handle_RecepcionistaSinMatricula_CreaElProfesional()
    {
        var comando = new CrearProfesionalCommand(
            Nombre: "Marta",
            Apellido: "Pérez",
            TipoProfesional: TipoProfesional.Recepcionista,
            CorreoElectronico: "marta.perez@clinica.com");

        var resultado = await _handler.Handle(comando, CancellationToken.None);

        Assert.True(resultado.EsExitoso);
        Assert.Null(resultado.Valor.NumeroMatricula);
    }
}
