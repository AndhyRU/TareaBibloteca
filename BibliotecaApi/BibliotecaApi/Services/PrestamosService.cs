using BibliotecaApi.Models;
using Microsoft.Data.Sqlite;
using Dapper;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BibliotecaApi.Services;

public class PrestamosService (IConfiguration configuration)
{
    private readonly string _connectionString =
        configuration.GetConnectionString("Coneccion")!;

    private SqliteConnection CreateConnecion =>
        new SqliteConnection(_connectionString);

    public async Task InitializeAsync()
    {
        const string sql = """
                CREATE TABLE IF NOT EXISTS Prestamos
                (
                    PrestamoId INTEGER PRIMARY KEY AUTOINCREMENT,
                    EstudianteId INTEGER NOT NULL,
                    LibroId INTEGER NOT NULL,
                    FechaPrestamo TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    FechaLimite TEXT NOT NULL,
                    Estado TEXT NOT NULL DEFAULT 'Activo'
                        CHECK (Estado IN ('Activo', 'Devuelto')),

                    FOREIGN KEY (EstudianteId)
                        REFERENCES Estudiantes(EstudianteId),

                    FOREIGN KEY (LibroId)
                        REFERENCES Libros(LibroId)
                );
                CREATE UNIQUE INDEX IF NOT EXISTS IX_Prestamos_LibroActivo
                ON Prestamos (LibroId)
                WHERE Estado = 'Activo';
                """;

        var connection = CreateConnecion;

        await connection.ExecuteAsync(sql);
    }
    public async Task<IEnumerable<PrestamosRecordGet>> GetListAsync()
    {
        const string sql = """
        SELECT PrestamoId,
               EstudianteId,
               LibroId,
               FechaPrestamo,
               FechaLimite,
               Estado
        FROM Prestamos;
        """;

        var connection = CreateConnecion;

        return await connection.QueryAsync<PrestamosRecordGet>(sql);
    }
    public async Task<PrestamosRecordGet?> GetByIdAsync(long id)
    {
        const string sql = """
        SELECT PrestamoId,
               EstudianteId,
               LibroId,
               FechaPrestamo,
               FechaLimite,
               Estado
        FROM Prestamos
        WHERE PrestamoId = @Id;
        """;

        var connection = CreateConnecion;

        return await connection.QuerySingleOrDefaultAsync<PrestamosRecordGet>(
            sql, new { Id = id });
    }
    public async Task<IEnumerable<PrestamosRecordGet>> GetByEstudianteAsync(long id)
    {
        const string sql = """
        SELECT PrestamoId,
               EstudianteId,
               LibroId,
               FechaPrestamo,
               FechaLimite,
               Estado
        FROM Prestamos
        WHERE EstudianteId = @Id;
        """;

        var connection = CreateConnecion;

        return await connection.QueryAsync<PrestamosRecordGet>(
            sql, new { Id = id });
    }
    public async Task<IEnumerable<PrestamosRecordGet>> GetByLibroAsync(long id)
    {
        const string sql = """
        SELECT PrestamoId,
               EstudianteId,
               LibroId,
               FechaPrestamo,
               FechaLimite,
               Estado
        FROM Prestamos
        WHERE LibroId = @Id;
        """;

        var connection = CreateConnecion;

        return await connection.QueryAsync<PrestamosRecordGet>(
            sql, new { Id = id });
    }
    public async Task<long> SaveAsync(PrestamosRecordSet prestamo)
    {
        var connection = CreateConnecion;
        await connection.OpenAsync();

        using var transaction = connection.BeginTransaction();

        const string sqlDisponible = """
        UPDATE Libros
        SET Disponible = 0
        WHERE LibroId = @LibroId
        AND Disponible = 1;
        """;

        var resultado = await connection.ExecuteAsync(
            sqlDisponible,
            new { prestamo.LibroId },
            transaction);

        if (resultado == 0)
        {
            transaction.Rollback();
            return 0;
        }

        const string sqlPrestamo = """
        INSERT INTO Prestamos
        (EstudianteId, LibroId, FechaLimite)
        VALUES
        (@EstudianteId, @LibroId, @FechaLimite)
        RETURNING PrestamoId;
        """;

        var id = await connection.ExecuteScalarAsync<long>(
            sqlPrestamo,
            prestamo,
            transaction);

        transaction.Commit();

        return id;
    }

}
