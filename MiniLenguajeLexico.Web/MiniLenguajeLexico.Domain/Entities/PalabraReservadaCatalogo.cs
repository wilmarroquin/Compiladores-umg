namespace MiniLenguajeLexico.Domain.Entities;

public class PalabraReservadaCatalogo
{
    public int IdPalabraReservada { get; set; }
    public string Palabra { get; set; } = string.Empty;
    public bool Activo { get; set; }
}
