using System.ComponentModel.DataAnnotations;

namespace MiniLenguajeLexico.Web.Models;

public class EditarDelimitadorViewModel
{
    public int IdDelimitador { get; set; }

    [Required(ErrorMessage = "El delimitador es obligatorio.")]
    [Display(Name = "Delimitador")]
    public string Simbolo { get; set; } = string.Empty;

    [Display(Name = "Activo")]
    public bool Activo { get; set; }

    public string? MensajeExito { get; set; }
    public string? MensajeError { get; set; }
}
