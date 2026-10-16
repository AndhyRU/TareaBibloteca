namespace BibliotecaApi.Models;

public record PrestamosRecordSet(
   int EstudianteId,
   int LibroId,
   string FechaLimite
);

public record PrestamosRecordGet(
    int PrestamoId,
    int EstudianteId,
    int LibroId,
    string FechaPrestamo,
    string FechaLimite,
    string Estado
);
