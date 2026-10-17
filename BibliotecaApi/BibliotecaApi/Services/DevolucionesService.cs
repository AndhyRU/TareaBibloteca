using BibliotecaApi.Models;
using Microsoft.Data.Sqlite;
using Dapper;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BibliotecaApi.Services;

public class DevolucionesService (IConfiguration configuration)
{
    private readonly string _connectionString =
        configuration.GetConnectionString("Coneccion")!;

    private SqliteConnection CreateConnecion =>
        new SqliteConnection(_connectionString);
    public async Task InitializeAsync()
    {
        const string sql = """
                CREATE TABLE IF NOT EXISTS Devoluciones
                (
                    DevolucionId INTEGER PRIMARY KEY AUTOINCREMENT,
                    PrestamoId INTEGER NOT NULL UNIQUE,
                    FechaDevolucion TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    Observacion TEXT,

                    FOREIGN KEY (PrestamoId)
                        REFERENCES Prestamos(PrestamoId)
                );
                """;

       var connection = CreateConnecion;

        await connection.ExecuteAsync(sql);
    }
    public async Task<IEnumerable<DevolucionesRecordGet>> GetListAsync()
    {
        const string sql = """
        SELECT DevolucionId,
               PrestamoId,
               FechaDevolucion,
               Observacion
        FROM Devoluciones;
        """;

        var connection = CreateConnecion;

        return await connection.QueryAsync<DevolucionesRecordGet>(sql);
    }
    public async Task<DevolucionesRecordGet?> GetByIdAsync(long id)
    {
        const string sql = """
        SELECT DevolucionId,
               PrestamoId,
               FechaDevolucion,
               Observacion
        FROM Devoluciones
        WHERE DevolucionId = @Id;
        """;

        var connection = CreateConnecion;

        return await connection.QuerySingleOrDefaultAsync<DevolucionesRecordGet>(
            sql, new { Id = id });
    }
    public async Task<DevolucionesRecordGet?> GetByPrestamoAsync(long id)
    {
        const string sql = """
        SELECT DevolucionId,
               PrestamoId,
               FechaDevolucion,
               Observacion
        FROM Devoluciones
        WHERE PrestamoId = @Id;
        """;

        var connection = CreateConnecion;

        return await connection.QuerySingleOrDefaultAsync<DevolucionesRecordGet>(
            sql, new { Id = id });
    }
    public async Task<long> SaveAsync(DevolucionesRecordSet devolucion)
    {
        var connection = CreateConnecion;
        await connection.OpenAsync();

        using var transaction = connection.BeginTransaction();

        const string sqlPrestamo = """
        UPDATE Prestamos
        SET Estado = 'Devuelto'
        WHERE PrestamoId = @PrestamoId
        AND Estado = 'Activo';
        """;

        var resultado = await connection.ExecuteAsync(
            sqlPrestamo,
            new { devolucion.PrestamoId },
            transaction);

        if (resultado == 0)
        {
            transaction.Rollback();
            return 0;
        }

        const string sqlDevolucion = """
        INSERT INTO Devoluciones
        (PrestamoId, Observacion)
        VALUES
        (@PrestamoId, @Observacion)
        RETURNING DevolucionId;
        """;

        var id = await connection.ExecuteScalarAsync<long>(
            sqlDevolucion,
            devolucion,
            transaction);

        const string sqlLibro = """
        UPDATE Libros
        SET Disponible = 1
        WHERE LibroId = (
            SELECT LibroId
            FROM Prestamos
            WHERE PrestamoId = @PrestamoId
        );
        """;

        await connection.ExecuteAsync(
            sqlLibro,
            new { devolucion.PrestamoId },
            transaction);

        transaction.Commit();

        return id;
    }
}
