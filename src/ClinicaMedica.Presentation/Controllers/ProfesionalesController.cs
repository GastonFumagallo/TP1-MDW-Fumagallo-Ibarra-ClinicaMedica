using ClinicaMedica.Application.Commands;
using ClinicaMedica.Application.Common;
using ClinicaMedica.Application.DTOs;
using ClinicaMedica.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaMedica.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProfesionalesController(ISender mediador) : ControllerBase
{
    /// <summary>Lista los profesionales activos.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ProfesionalDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerTodos(CancellationToken cancellationToken)
    {
        var resultado = await mediador.Send(new ObtenerTodosProfesionalesQuery(), cancellationToken);

        return resultado.EsExitoso
            ? Ok(resultado.Valor)
            : MapearError(resultado.Error);
    }

    /// <summary>Obtiene un profesional por su identificador.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProfesionalDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerPorId(Guid id, CancellationToken cancellationToken)
    {
        var resultado = await mediador.Send(new ObtenerProfesionalPorIdQuery(id), cancellationToken);

        return resultado.EsExitoso
            ? Ok(resultado.Valor)
            : MapearError(resultado.Error);
    }

    /// <summary>Registra un nuevo profesional de la clínica.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ProfesionalDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Crear(
        [FromBody] CrearProfesionalCommand comando,
        CancellationToken cancellationToken)
    {
        var resultado = await mediador.Send(comando, cancellationToken);

        return resultado.EsExitoso
            ? CreatedAtAction(nameof(ObtenerPorId), new { id = resultado.Valor.Id }, resultado.Valor)
            : MapearError(resultado.Error);
    }

    /// <summary>Modifica los datos de un profesional activo.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ProfesionalDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Actualizar(
        Guid id,
        [FromBody] ActualizarProfesionalDto solicitud,
        CancellationToken cancellationToken)
    {
        var comando = new ActualizarProfesionalCommand(
            id,
            solicitud.Nombre,
            solicitud.Apellido,
            solicitud.TipoProfesional,
            solicitud.CorreoElectronico,
            solicitud.NumeroMatricula,
            solicitud.Especialidad,
            solicitud.Telefono);

        var resultado = await mediador.Send(comando, cancellationToken);

        return resultado.EsExitoso
            ? Ok(resultado.Valor)
            : MapearError(resultado.Error);
    }

    /// <summary>Da de baja (eliminación lógica) a un profesional.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Eliminar(Guid id, CancellationToken cancellationToken)
    {
        var resultado = await mediador.Send(new EliminarProfesionalCommand(id), cancellationToken);

        return resultado.EsExitoso
            ? NoContent()
            : MapearError(resultado.Error);
    }

    private ObjectResult MapearError(Error error)
    {
        var estado = error.Tipo switch
        {
            TipoError.Validacion => StatusCodes.Status400BadRequest,
            TipoError.NoEncontrado => StatusCodes.Status404NotFound,
            TipoError.Conflicto => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

        return Problem(title: error.Codigo, detail: error.Descripcion, statusCode: estado);
    }
}
