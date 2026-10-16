using BibliotecaApi.Models;
using BibliotecaApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;

namespace BibliotecaApi.Controllers;

[ApiController]
[Route("api/estudiantes")]
public class EstudiantesController(EstudiantesServices service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetList()
    {
        var estudiantes = await service.GetListAsync();

        return Ok(estudiantes);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var estudiante = await service.GetByIdAsync(id);

        if (estudiante == null)
            return NotFound("Estudiante no encontrado");

        return Ok(estudiante);
    }

    [HttpGet("matricula/{matricula}")]
    public async Task<IActionResult> GetByMatricula(string matricula)
    {
        var estudiante = await service.GetByMatriculaAsync(matricula);

        if (estudiante == null)
            return NotFound("Estudiante no encontrado");

        return Ok(estudiante);
    }

    [HttpPost]
    public async Task<IActionResult> Post(EstudiantesRecordSet estudiante)
    {
        try
        {
            var id = await service.SaveAsync(estudiante);

            return Ok(new
            {
                EstudianteId = id,
                Estudiante = estudiante
            });
        }
        catch (SqliteException ex) when (ex.SqliteErrorCode == 19)
        {
            return Conflict("La matrícula, teléfono o correo ya está registrado, o algún dato incumple una restricción.");
        }
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Put(long id, EstudiantesRecordSet estudiante)
    {
        try
        {
            var actualizado = await service.UpdateAsync(id, estudiante);

            if (!actualizado)
                return NotFound("Estudiante no encontrado");

            var resultado = await service.GetByIdAsync(id);

            return Ok(resultado);
        }
        catch (SqliteException ex) when (ex.SqliteErrorCode == 19)
        {
            return Conflict("Los datos proporcionados incumplen una restricción de la base de datos.");
        }
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        try
        {
            var eliminado = await service.DeleteAsync(id);

            if (!eliminado)
                return NotFound("Estudiante no encontrado");

            return Ok("Estudiante eliminado correctamente");
        }
        catch (SqliteException ex) when (ex.SqliteExtendedErrorCode == 787)
        {
            return Conflict("No se puede eliminar el estudiante porque tiene préstamos registrados.");
        }
    }

}