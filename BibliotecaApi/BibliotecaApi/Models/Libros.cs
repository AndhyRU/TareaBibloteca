namespace BibliotecaApi.Models;

public record LibrosRecordSet(
   string Titulo,
   string Autor,
   string ISBN,
   long? AnioPublicacion
);

public record LibrosRecordGet(
    long LibroId,
    string Titulo,
    string Autor,
    string ISBN,
    long? AnioPublicacion,
    long Disponible
);

public record LibroEstadoRecord(
    long LibroId,
    string Titulo,
    long Disponible,
    string? Matricula,
    string? Nombres,
    string? FechaLimite
);