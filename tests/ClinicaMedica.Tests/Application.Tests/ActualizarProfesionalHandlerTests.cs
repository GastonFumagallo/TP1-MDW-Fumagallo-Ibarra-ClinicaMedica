using ClinicaMedica.Application.Commands;
using ClinicaMedica.Application.Common;
using ClinicaMedica.Domain.Entities;
using ClinicaMedica.Domain.Enums;
using ClinicaMedica.Domain.Interfaces;
using Moq;
using Xunit;

namespace ClinicaMedica.Tests.Application.Tests;

public class ActualizarProfesionalHandlerTests
{
    private readonly Mock<IProfesionalRepository> _repositorio = new();
    private readonly ActualizarProfesionalHandler _handler;

    public ActualizarProfesionalHandlerTests()
    {
        _handler = new ActualizarProfesionalHandler(_repositorio.Object);
    }

    private static Profesional CrearMedico() => Profesional.Crear(
        "Laura", "Gómez", TipoProfesional.Medico, "laura.gomez@clinica.com", "MP-12345", "Cardiología");

    private static ActualizarProfesionalCommand ComandoValido(Guid id) => new(
        Id: id,
        Nombre: "Laura Beatriz",
        Apellido: "Gómez",
        TipoProfesional: TipoProfesional.Medico,
        CorreoElectronico: "Laura.Gomez@clinica.com",
        NumeroMatricula: "mp-12345",
        Especialidad: "Neurología",
        Telefono: "341-5559999");

    [Fact]
    public async Task Handle_ConDatosValidos_ActualizaYDevuelveExito()
    {
        var profesional = CrearMedico();
        _repositorio
            .Setup(r => r.ObtenerPorIdAsync(profesional.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profesional);

        var resultado = await _handler.Handle(ComandoValido(profesional.Id), CancellationToken.None);

        Assert.True(resultado.EsExitoso);
        Assert.Equal("Laura Beatriz", resultado.Valor.Nombre);
        Assert.Equal("Neurología", resultado.Valor.Especialidad);
        Assert.Equal("laura.gomez@clinica.com", resultado.Valor.CorreoElectronico);

        _repositorio.Verify(
            r => r.ActualizarAsync(profesional, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ProfesionalInexistente_DevuelveNoEncontrado()
    {
        var resultado = await _handler.Handle(ComandoValido(Guid.NewGuid()), CancellationToken.None);

        Assert.True(resultado.EsFallido);
        Assert.Equal(TipoError.NoEncontrado, resultado.Error.Tipo);

        _repositorio.Verify(
            r => r.ActualizarAsync(It.IsAny<Profesional>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ProfesionalDadoDeBaja_DevuelveConflicto()
    {
        var profesional = CrearMedico();
        profesional.DarDeBaja();
        _repositorio
            .Setup(r => r.ObtenerPorIdAsync(profesional.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profesional);

        var resultado = await _handler.Handle(ComandoValido(profesional.Id), CancellationToken.None);

        Assert.True(resultado.EsFallido);
        Assert.Equal("Profesional.Inactivo", resultado.Error.Codigo);
    }

    [Fact]
    public async Task Handle_CorreoUsadoPorOtroProfesional_DevuelveConflicto()
    {
        var profesional = CrearMedico();
        _repositorio
            .Setup(r => r.ObtenerPorIdAsync(profesional.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profesional);
        _repositorio
            .Setup(r => r.ExisteCorreoEnOtroProfesionalAsync(
                "laura.gomez@clinica.com", profesional.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var resultado = await _handler.Handle(ComandoValido(profesional.Id), CancellationToken.None);

        Assert.True(resultado.EsFallido);
        Assert.Equal("Profesional.CorreoDuplicado", resultado.Error.Codigo);
    }

    [Fact]
    public async Task Handle_ConCorreoInvalido_DevuelveErrorDeValidacion()
    {
        var comando = ComandoValido(Guid.NewGuid()) with { CorreoElectronico = "no-es-un-correo" };

        var resultado = await _handler.Handle(comando, CancellationToken.None);

        Assert.True(resultado.EsFallido);
        Assert.Equal(TipoError.Validacion, resultado.Error.Tipo);
    }
}
