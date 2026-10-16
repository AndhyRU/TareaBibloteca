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

        using var connection = CreateConnecion;

        await connection.ExecuteAsync(sql);
    }
}
