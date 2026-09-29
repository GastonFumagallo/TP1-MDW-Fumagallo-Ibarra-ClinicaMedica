using ClinicaMedica.Application.Commands;
using ClinicaMedica.Application.Common;
using ClinicaMedica.Domain.Entities;
using ClinicaMedica.Domain.Enums;
using ClinicaMedica.Domain.Interfaces;
using Moq;
using Xunit;

namespace ClinicaMedica.Tests.Application.Tests;

public class EliminarProfesionalHandlerTests
{
    private readonly Mock<IProfesionalRepository> _repositorio = new();
    private readonly EliminarProfesionalHandler _handler;

    public EliminarProfesionalHandlerTests()
    {
        _handler = new EliminarProfesionalHandler(_repositorio.Object);
    }

    private static Profesional CrearRecepcionista() => Profesional.Crear(
        "Marta", "Pérez", TipoProfesional.Recepcionista, "marta.perez@clinica.com");

    [Fact]
    public async Task Handle_ProfesionalActivo_LoDaDeBajaLogicamente()
    {
        var profesional = CrearRecepcionista();
        _repositorio
            .Setup(r => r.ObtenerPorIdAsync(profesional.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profesional);

        var resultado = await _handler.Handle(new EliminarProfesionalCommand(profesional.Id), CancellationToken.None);

        Assert.True(resultado.EsExitoso);
        Assert.False(profesional.Activo);
        Assert.NotNull(profesional.FechaBaja);

        _repositorio.Verify(
            r => r.ActualizarAsync(profesional, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ProfesionalYaDadoDeBaja_EsIdempotenteYNoPersisteDeNuevo()
    {
        var profesional = CrearRecepcionista();
        profesional.DarDeBaja();
        _repositorio
            .Setup(r => r.ObtenerPorIdAsync(profesional.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profesional);

        var resultado = await _handler.Handle(new EliminarProfesionalCommand(profesional.Id), CancellationToken.None);

        Assert.True(resultado.EsExitoso);
        _repositorio.Verify(
            r => r.ActualizarAsync(It.IsAny<Profesional>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ProfesionalInexistente_DevuelveNoEncontrado()
    {
        var resultado = await _handler.Handle(new EliminarProfesionalCommand(Guid.NewGuid()), CancellationToken.None);

        Assert.True(resultado.EsFallido);
        Assert.Equal(TipoError.NoEncontrado, resultado.Error.Tipo);
    }
}
