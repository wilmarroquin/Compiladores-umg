namespace MiniLenguajeLexico.Domain.Entities;

public class TokenAnalisis
{
    public string Lexema { get; set; } = string.Empty;
    public string TipoToken { get; set; } = string.Empty;
    public int NumeroLinea { get; set; }
    public int NumeroColumna { get; set; }
}
