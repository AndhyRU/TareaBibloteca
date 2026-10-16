using BibliotecaApi.Models;
using BibliotecaApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;

namespace BibliotecaApi.Controllers;

[ApiController]
[Route("api/prestamos")]
public class PrestamosController(PrestamosService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetList()
    {
        var prestamos = await service.GetListAsync();

        return Ok(prestamos);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var prestamo = await service.GetByIdAsync(id);

        if (prestamo == null)
            return NotFound("Préstamo no encontrado");

        return Ok(prestamo);
    }

    [HttpGet("estudiante/{id:long}")]
    public async Task<IActionResult> GetByEstudiante(long id)
    {
        var prestamos = await service.GetByEstudianteAsync(id);

        return Ok(prestamos);
    }

    [HttpGet("libro/{id:long}")]
    public async Task<IActionResult> GetByLibro(long id)
    {
        var prestamos = await service.GetByLibroAsync(id);

        return Ok(prestamos);
    }

    [HttpPost]
    public async Task<IActionResult> Post(PrestamosRecordSet prestamo)
    {
        if (!DateTime.TryParseExact(
            prestamo.FechaLimite,
            "yyyy-MM-dd",
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None,
            out var fechaLimite))
        {
            return BadRequest("La fecha debe tener el formato yyyy-MM-dd.");
        }

        if (fechaLimite.Date < DateTime.Today)
            return BadRequest("La fecha límite no puede ser anterior a hoy.");

        try
        {
            var id = await service.SaveAsync(prestamo);

            if (id == 0)
                return Conflict("El libro no existe o no está disponible.");

            var resultado = await service.GetByIdAsync(id);

            return Ok(resultado);
        }
        catch (SqliteException ex) when (ex.SqliteExtendedErrorCode == 787)
        {
            return BadRequest("El estudiante indicado no existe.");
        }
        catch (SqliteException ex) when (ex.SqliteErrorCode == 19)
        {
            return Conflict("No se pudo registrar el préstamo por una restricción de la base de datos.");
        }
    }
}