using System.ComponentModel.DataAnnotations;
using MiniLenguajeLexico.Domain.Entities;

namespace MiniLenguajeLexico.Web.Models;

public class PalabrasReservadasViewModel
{
    [Required(ErrorMessage = "La palabra reservada es obligatoria.")]
    [Display(Name = "Nueva palabra reservada")]
    public string NuevaPalabra { get; set; } = string.Empty;

    public IReadOnlyList<PalabraReservadaCatalogo> Palabras { get; set; } = [];

    public string? MensajeExito { get; set; }
    public string? MensajeError { get; set; }
}
