namespace MiniLenguajeLexico.Application.DTOs;

public class ResultadoAnalisisDto
{
    public int IdAnalisis { get; set; }
    public bool FuePersistido { get; set; }
    public string EstadoAnalisis { get; set; } = string.Empty;
    public string? Advertencia { get; set; }
    public List<TokenDto> Tokens { get; set; } = new();
    public List<ErrorAnalisisDto> Errores { get; set; } = new();
}
