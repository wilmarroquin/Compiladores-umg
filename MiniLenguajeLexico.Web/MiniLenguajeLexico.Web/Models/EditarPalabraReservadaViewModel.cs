using System.ComponentModel.DataAnnotations;

namespace MiniLenguajeLexico.Web.Models;

public class EditarPalabraReservadaViewModel
{
    public int IdPalabraReservada { get; set; }

    [Required(ErrorMessage = "La palabra reservada es obligatoria.")]
    [Display(Name = "Palabra reservada")]
    public string Palabra { get; set; } = string.Empty;

    [Display(Name = "Activo")]
    public bool Activo { get; set; }

    public string? MensajeExito { get; set; }
    public string? MensajeError { get; set; }
}
