using BibliotecaApi.Models;
using BibliotecaApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;

namespace BibliotecaApi.Controllers;

[ApiController]
[Route("api/devoluciones")]
public class DevolucionesController(DevolucionesService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetList()
    {
        var devoluciones = await service.GetListAsync();

        return Ok(devoluciones);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var devolucion = await service.GetByIdAsync(id);

        if (devolucion == null)
            return NotFound("Devolución no encontrada");

        return Ok(devolucion);
    }

    [HttpGet("prestamo/{id:long}")]
    public async Task<IActionResult> GetByPrestamo(long id)
    {
        var devolucion = await service.GetByPrestamoAsync(id);

        if (devolucion == null)
            return NotFound("Este préstamo no tiene una devolución registrada");

        return Ok(devolucion);
    }

    [HttpPost]
    public async Task<IActionResult> Post(DevolucionesRecordSet devolucion)
    {
        try
        {
            var id = await service.SaveAsync(devolucion);

            if (id == 0)
                return Conflict("El préstamo no existe o ya fue devuelto.");

            var resultado = await service.GetByIdAsync(id);

            return Ok(resultado);
        }
        catch (SqliteException ex) when (ex.SqliteErrorCode == 19)
        {
            return Conflict("No se pudo registrar la devolución.");
        }
    }
}
