namespace BibliotecaApi.Models
{
    public record DevolucionesRecordSet(
       long PrestamoId,
       string? Observacion
   );

    public record DevolucionesRecordGet(
        long DevolucionId,
        long PrestamoId,
        string FechaDevolucion,
        string? Observacion
    );
}
