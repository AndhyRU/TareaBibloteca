namespace BibliotecaApi.Models
{
    public record DevolucionesRecordSet(
       int PrestamoId,
       string? Observacion
   );

    public record DevolucionesRecordGet(
        int DevolucionId,
        int PrestamoId,
        string FechaDevolucion,
        string? Observacion
    );
}
