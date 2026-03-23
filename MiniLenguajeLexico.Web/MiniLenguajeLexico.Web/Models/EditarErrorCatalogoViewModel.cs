using System.ComponentModel.DataAnnotations;

namespace MiniLenguajeLexico.Web.Models;

public class EditarErrorCatalogoViewModel
{
    public int IdErrorCatalogo { get; set; }

    [Required(ErrorMessage = "El codigo de error es obligatorio.")]
    [Display(Name = "Codigo de error")]
    public string CodigoError { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre del error es obligatorio.")]
    [Display(Name = "Nombre del error")]
    public string NombreError { get; set; } = string.Empty;

    [Required(ErrorMessage = "La descripcion del error es obligatoria.")]
    [Display(Name = "Descripcion del error")]
    public string DescripcionError { get; set; } = string.Empty;

    [Required(ErrorMessage = "El tipo de error es obligatorio.")]
    [Display(Name = "Tipo de error")]
    public string TipoError { get; set; } = string.Empty;

    [Display(Name = "Activo")]
    public bool Activo { get; set; }

    public string? MensajeExito { get; set; }
    public string? MensajeError { get; set; }
}
