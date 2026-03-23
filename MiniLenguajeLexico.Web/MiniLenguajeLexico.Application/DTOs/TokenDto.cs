namespace MiniLenguajeLexico.Application.DTOs;

public class TokenDto
{
    public string Lexema { get; set; } = string.Empty;
    public string TipoToken { get; set; } = string.Empty;
    public int NumeroLinea { get; set; }
    public int NumeroColumna { get; set; }
}