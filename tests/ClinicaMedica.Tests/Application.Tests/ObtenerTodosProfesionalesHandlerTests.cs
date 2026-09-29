using ClinicaMedica.Application.Queries;
using ClinicaMedica.Domain.Entities;
using ClinicaMedica.Domain.Enums;
using ClinicaMedica.Domain.Interfaces;
using Moq;
using Xunit;

namespace ClinicaMedica.Tests.Application.Tests;

public class ObtenerTodosProfesionalesHandlerTests
{
    [Fact]
    public async Task Handle_DevuelveLosProfesionalesActivosQueEntregaElRepositorio()
    {
        IReadOnlyList<Profesional> activos = new List<Profesional>
        {
            Profesional.Crear("Marta", "Pérez", TipoProfesional.Recepcionista, "marta.perez@clinica.com"),
            Profesional.Crear("Juan", "Ruiz", TipoProfesional.Tecnico, "juan.ruiz@clinica.com")
        };

        var repositorio = new Mock<IProfesionalRepository>();
        repositorio
            .Setup(r => r.ObtenerTodosActivosAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(activos);

        var handler = new ObtenerTodosProfesionalesHandler(repositorio.Object);

        var resultado = await handler.Handle(new ObtenerTodosProfesionalesQuery(), CancellationToken.None);

        Assert.True(resultado.EsExitoso);
        Assert.Equal(2, resultado.Valor.Count);
        Assert.All(resultado.Valor, dto => Assert.True(dto.Activo));
    }
}
