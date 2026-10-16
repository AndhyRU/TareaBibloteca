using BibliotecaApi.Models;
using Microsoft.Data.Sqlite;
using Dapper;
using Microsoft.AspNetCore.Http.HttpResults;


namespace BibliotecaApi.Services;

public class EstudiantesServices(IConfiguration configuration)
{
    private readonly string _connectionString =
        configuration.GetConnectionString("Coneccion")!;

    private SqliteConnection CreateConnecion =>
        new SqliteConnection(_connectionString);


    public async Task InitializeAsync()
    {
        const string sql = """
                CREATE TABLE IF NOT EXISTS Estudiantes
                (
                    EstudianteId INTEGER PRIMARY KEY AUTOINCREMENT,
                    Matricula TEXT NOT NULL UNIQUE,
                    Nombres TEXT NOT NULL,
                    Telefono TEXT NOT NULL UNIQUE,
                    Email TEXT UNIQUE,
                    Direccion TEXT
                );
                """;

        var connection = CreateConnecion;

        await connection.ExecuteAsync(sql);
    }

    public async Task<IEnumerable<EstudiantesRecordGet>> GetListAsync()
    {
        const string sql = """
        SELECT EstudianteId, Matricula, Nombres,
               Telefono, Email, Direccion
        FROM Estudiantes;
        """;

        using var connection = CreateConnecion;

        return await connection.QueryAsync<EstudiantesRecordGet>(sql);
    }
    public async Task<EstudiantesRecordGet?> GetByIdAsync(long id)
    {
        const string sql = """
        SELECT EstudianteId, Matricula, Nombres,
               Telefono, Email, Direccion
        FROM Estudiantes
        WHERE EstudianteId = @Id;
        """;

        using var connection = CreateConnecion;

        return await connection.QuerySingleOrDefaultAsync<EstudiantesRecordGet>(
            sql, new { Id = id });
    }
    public async Task<EstudiantesRecordGet?> GetByMatriculaAsync(string matricula)
    {
        const string sql = """
        SELECT EstudianteId, Matricula, Nombres,
               Telefono, Email, Direccion
        FROM Estudiantes
        WHERE Matricula = @Matricula;
        """;

        using var connection = CreateConnecion;

        return await connection.QuerySingleOrDefaultAsync<EstudiantesRecordGet>(
            sql, new { Matricula = matricula });
    }
    public async Task<long> SaveAsync(EstudiantesRecordSet estudiante)
    {
        const string sql = """
        INSERT INTO Estudiantes
        (Matricula, Nombres, Telefono, Email, Direccion)
        VALUES
        (@Matricula, @Nombres, @Telefono, @Email, @Direccion)
        RETURNING EstudianteId;
        """;

        using var connection = CreateConnecion;

        return await connection.ExecuteScalarAsync<long>(sql, estudiante);
    }
    public async Task<bool> UpdateAsync(long id, EstudiantesRecordSet estudiante)
    {
        const string sql = """
        UPDATE Estudiantes
        SET Matricula = @Matricula,
            Nombres = @Nombres,
            Telefono = @Telefono,
            Email = @Email,
            Direccion = @Direccion
        WHERE EstudianteId = @Id;
        """;

        using var connection = CreateConnecion;

        var resultado = await connection.ExecuteAsync(sql, new
        {
            Id = id,
            estudiante.Matricula,
            estudiante.Nombres,
            estudiante.Telefono,
            estudiante.Email,
            estudiante.Direccion
        });

        return resultado > 0;
    }
    public async Task<bool> DeleteAsync(long id)
    {
        const string sql = """
        DELETE FROM Estudiantes
        WHERE EstudianteId = @Id;
        """;

        using var connection = CreateConnecion;

        var resultado = await connection.ExecuteAsync(sql, new { Id = id });

        return resultado > 0;
    }

}