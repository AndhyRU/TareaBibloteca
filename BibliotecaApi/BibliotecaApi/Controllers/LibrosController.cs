using BibliotecaApi.Models;
using BibliotecaApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;

namespace BibliotecaApi.Controllers;

[ApiController]
[Route("api/libros")]
public class LibrosController(LibrosService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetList()
    {
        var libros = await service.GetListAsync();

        return Ok(libros);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var libro = await service.GetByIdAsync(id);

        if (libro == null)
            return NotFound("Libro no encontrado");

        return Ok(libro);
    }

    [HttpGet("titulo/{titulo}")]
    public async Task<IActionResult> GetByTitulo(string titulo)
    {
        var libros = await service.GetByTituloAsync(titulo);

        return Ok(libros);
    }

    [HttpGet("isbn/{isbn}")]
    public async Task<IActionResult> GetByISBN(string isbn)
    {
        var libro = await service.GetByISBNAsync(isbn);

        if (libro == null)
            return NotFound("Libro no encontrado");

        return Ok(libro);
    }

    [HttpGet("disponibles")]
    public async Task<IActionResult> GetDisponibles()
    {
        var libros = await service.GetDisponiblesAsync();

        return Ok(libros);
    }

    [HttpGet("{id:long}/estado")]
    public async Task<IActionResult> GetEstado(long id)
    {
        var estado = await service.GetEstadoAsync(id);

        if (estado == null)
            return NotFound("Libro no encontrado");

        return Ok(estado);
    }

    [HttpPost]
    public async Task<IActionResult> Post(LibrosRecordSet libro)
    {
        try
        {
            var id = await service.SaveAsync(libro);

            return Ok(new
            {
                LibroId = id,
                Libro = libro
            });
        }
        catch (SqliteException ex) when (ex.SqliteExtendedErrorCode == 2067)
        {
            return Conflict("Ya existe un libro con ese ISBN.");
        }
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Put(long id, LibrosRecordSet libro)
    {
        try
        {
            var actualizado = await service.UpdateAsync(id, libro);

            if (!actualizado)
                return NotFound("Libro no encontrado");

            var resultado = await service.GetByIdAsync(id);

            return Ok(resultado);
        }
        catch (SqliteException ex) when (ex.SqliteExtendedErrorCode == 2067)
        {
            return Conflict("Ya existe otro libro con ese ISBN.");
        }
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        try
        {
            var eliminado = await service.DeleteAsync(id);

            if (!eliminado)
                return NotFound("Libro no encontrado");

            return Ok("Libro eliminado correctamente");
        }
        catch (SqliteException ex) when (ex.SqliteExtendedErrorCode == 787)
        {
            return Conflict(
                "No se puede eliminar el libro porque tiene préstamos registrados.");
        }
    }
}
