using System.ComponentModel.DataAnnotations;
using MiniLenguajeLexico.Application.DTOs;

namespace MiniLenguajeLexico.Web.Models;

public class AnalizadorViewModel
{
    [Required(ErrorMessage = "El codigo fuente es obligatorio.")]
    [Display(Name = "Codigo fuente")]
    public string CodigoFuente { get; set; } = string.Empty;

    public ResultadoAnalisisDto? Resultado { get; set; }

    public string? ErrorGeneral { get; set; }
}
