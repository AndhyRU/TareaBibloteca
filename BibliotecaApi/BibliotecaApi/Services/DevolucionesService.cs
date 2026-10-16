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

        using var connection = CreateConnecion;

        await connection.ExecuteAsync(sql);
    }
}
