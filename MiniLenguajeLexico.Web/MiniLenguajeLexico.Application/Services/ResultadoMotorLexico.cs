namespace MiniLenguajeLexico.Application.Services;

internal sealed class ResultadoMotorLexico
{
    public List<TokenMotor> Tokens { get; set; } = new();
    public List<ErrorMotor> Errores { get; set; } = new();
}

internal sealed class TokenMotor
{
    public string Lexema { get; set; } = string.Empty;
    public string TipoToken { get; set; } = string.Empty;
    public int NumeroLinea { get; set; }
    public int NumeroColumna { get; set; }
}

internal sealed class ErrorMotor
{
    public int IdErrorCatalogo { get; set; }
    public string CodigoError { get; set; } = string.Empty;
    public string MensajeError { get; set; } = string.Empty;
    public string? Lexema { get; set; }
    public int NumeroLinea { get; set; }
    public int NumeroColumna { get; set; }
}
