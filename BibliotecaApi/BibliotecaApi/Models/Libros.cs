namespace BibliotecaApi.Models;

public record LibrosRecordSet(
   string Titulo,
   string Autor,
   string ISBN,
   int? AnioPublicacion
);

public record LibrosRecordGet(
    int LibroId,
    string Titulo,
    string Autor,
    string ISBN,
    int? AnioPublicacion,
    bool Disponible
);
