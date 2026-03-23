namespace MiniLenguajeLexico.Domain.Entities;

public class DelimitadorCatalogo
{
    public int IdDelimitador { get; set; }
    public string Simbolo { get; set; } = string.Empty;
    public bool Activo { get; set; }
}
