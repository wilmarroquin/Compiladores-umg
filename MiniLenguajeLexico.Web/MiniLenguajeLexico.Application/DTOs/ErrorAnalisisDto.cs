namespace MiniLenguajeLexico.Application.DTOs;

public class ErrorAnalisisDto
{
    public int IdErrorCatalogo { get; set; }
    public string CodigoError { get; set; } = string.Empty;
    public string MensajeError { get; set; } = string.Empty;
    public string? Lexema { get; set; }
    public int NumeroLinea { get; set; }
    public int NumeroColumna { get; set; }
}