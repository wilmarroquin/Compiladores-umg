namespace MiniLenguajeLexico.Domain.Entities;

public class ErrorCatalogo
{
    public int IdErrorCatalogo { get; set; }
    public string CodigoError { get; set; } = string.Empty;
    public string NombreError { get; set; } = string.Empty;
    public string DescripcionError { get; set; } = string.Empty;
    public string TipoError { get; set; } = string.Empty;
    public bool Activo { get; set; }
}
