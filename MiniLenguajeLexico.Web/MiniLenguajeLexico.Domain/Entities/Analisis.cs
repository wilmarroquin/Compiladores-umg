namespace MiniLenguajeLexico.Domain.Entities;

public class Analisis
{
    public int IdAnalisis { get; set; }
    public string EstadoAnalisis { get; set; } = string.Empty;
    public List<TokenAnalisis> Tokens { get; set; } = new();
    public List<ErrorAnalisis> Errores { get; set; } = new();
}
