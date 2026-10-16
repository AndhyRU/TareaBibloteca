namespace BibliotecaApi.Models;

public record PrestamosRecordSet(
   long EstudianteId,
   long LibroId,
   string FechaLimite
);

public record PrestamosRecordGet(
    long PrestamoId,
    long EstudianteId,
    long LibroId,
    string FechaPrestamo,
    string FechaLimite,
    string Estado
);
