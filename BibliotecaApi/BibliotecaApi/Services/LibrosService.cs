using BibliotecaApi.Models;
using Microsoft.Data.Sqlite;
using Dapper;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BibliotecaApi.Services;

public class LibrosService (IConfiguration configuration)
{
    private readonly string _connectionString =
        configuration.GetConnectionString("Coneccion")!;

    private SqliteConnection CreateConnecion =>
        new SqliteConnection(_connectionString);
    public async Task InitializeAsync()
    {
        const string sql = """
                CREATE TABLE IF NOT EXISTS Libros
                (
                    LibroId INTEGER PRIMARY KEY AUTOINCREMENT,
                    Titulo TEXT NOT NULL,
                    Autor TEXT NOT NULL,
                    ISBN TEXT NOT NULL UNIQUE,
                    AnioPublicacion INTEGER,
                    Disponible INTEGER NOT NULL DEFAULT 1
                        CHECK (Disponible IN (0, 1))
                );
                """;

        var connection = CreateConnecion;

        await connection.ExecuteAsync(sql);
    }
    public async Task<IEnumerable<LibrosRecordGet>> GetListAsync()
    {
        const string sql = """
        SELECT LibroId, Titulo, Autor, ISBN,
               AnioPublicacion, Disponible
        FROM Libros;
        """;

         var connection = CreateConnecion;

        return await connection.QueryAsync<LibrosRecordGet>(sql);
    }
    public async Task<LibrosRecordGet?> GetByIdAsync(long id)
    {
        const string sql = """
        SELECT LibroId, Titulo, Autor, ISBN,
               AnioPublicacion, Disponible
        FROM Libros
        WHERE LibroId = @Id;
        """;

        var connection = CreateConnecion;

        return await connection.QuerySingleOrDefaultAsync<LibrosRecordGet>(
            sql, new { Id = id });
    }
    public async Task<IEnumerable<LibrosRecordGet>> GetByTituloAsync(string titulo)
    {
        const string sql = """
        SELECT LibroId, Titulo, Autor, ISBN,
               AnioPublicacion, Disponible
        FROM Libros
        WHERE Titulo LIKE @Titulo;
        """;

        var connection = CreateConnecion;

        return await connection.QueryAsync<LibrosRecordGet>(
            sql, new { Titulo = $"%{titulo}%" });
    }
    public async Task<LibrosRecordGet?> GetByISBNAsync(string isbn)
    {
        const string sql = """
        SELECT LibroId, Titulo, Autor, ISBN,
               AnioPublicacion, Disponible
        FROM Libros
        WHERE ISBN = @ISBN;
        """;

        var connection = CreateConnecion;

        return await connection.QuerySingleOrDefaultAsync<LibrosRecordGet>(
            sql, new { ISBN = isbn });
    }
    public async Task<IEnumerable<LibrosRecordGet>> GetDisponiblesAsync()
    {
        const string sql = """
        SELECT LibroId, Titulo, Autor, ISBN,
               AnioPublicacion, Disponible
        FROM Libros
        WHERE Disponible = 1;
        """;

        var connection = CreateConnecion;

        return await connection.QueryAsync<LibrosRecordGet>(sql);
    }
    public async Task<LibroEstadoRecord?> GetEstadoAsync(long id)
    {
        const string sql = """
        SELECT l.LibroId,
               l.Titulo,
               l.Disponible,
               e.Matricula,
               e.Nombres,
               p.FechaLimite
        FROM Libros l
        LEFT JOIN Prestamos p
            ON l.LibroId = p.LibroId
            AND p.Estado = 'Activo'
        LEFT JOIN Estudiantes e
            ON p.EstudianteId = e.EstudianteId
        WHERE l.LibroId = @Id;
        """;

        var connection = CreateConnecion;

        return await connection.QuerySingleOrDefaultAsync<LibroEstadoRecord>(
            sql, new { Id = id });
    }
    public async Task<long> SaveAsync(LibrosRecordSet libro)
    {
        const string sql = """
        INSERT INTO Libros
        (Titulo, Autor, ISBN, AnioPublicacion)
        VALUES
        (@Titulo, @Autor, @ISBN, @AnioPublicacion)
        RETURNING LibroId;
        """;

        var connection = CreateConnecion;

        return await connection.ExecuteScalarAsync<long>(sql, libro);
    }
    public async Task<bool> UpdateAsync(long id, LibrosRecordSet libro)
    {
        const string sql = """
        UPDATE Libros
        SET Titulo = @Titulo,
            Autor = @Autor,
            ISBN = @ISBN,
            AnioPublicacion = @AnioPublicacion
        WHERE LibroId = @Id;
        """;

        var connection = CreateConnecion;

        var resultado = await connection.ExecuteAsync(sql, new
        {
            Id = id,
            libro.Titulo,
            libro.Autor,
            libro.ISBN,
            libro.AnioPublicacion
        });

        return resultado > 0;
    }
    public async Task<bool> DeleteAsync(long id)
    {
        const string sql = """
        DELETE FROM Libros
        WHERE LibroId = @Id;
        """;

        var connection = CreateConnecion;

        var resultado = await connection.ExecuteAsync(sql, new { Id = id });

        return resultado > 0;
    }
}
