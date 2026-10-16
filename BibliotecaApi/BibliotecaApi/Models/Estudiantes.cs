namespace BibliotecaApi.Models;

public record EstudiantesRecordSet(
    string Matricula,
    string Nombres,
    string Telefono,
    string? Email,
    string? Direccion
);

public record EstudiantesRecordGet(
    long EstudianteId,
    string Matricula,
    string Nombres,
    string Telefono,
    string? Email,
    string? Direccion
);
