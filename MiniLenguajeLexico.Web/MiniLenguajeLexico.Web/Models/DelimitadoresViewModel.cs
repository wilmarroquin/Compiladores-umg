using System.ComponentModel.DataAnnotations;
using MiniLenguajeLexico.Domain.Entities;

namespace MiniLenguajeLexico.Web.Models;

public class DelimitadoresViewModel
{
    [Required(ErrorMessage = "El delimitador es obligatorio.")]
    [Display(Name = "Nuevo delimitador")]
    public string NuevoDelimitador { get; set; } = string.Empty;

    public IReadOnlyList<DelimitadorCatalogo> Delimitadores { get; set; } = [];

    public string? MensajeExito { get; set; }
    public string? MensajeError { get; set; }
}
