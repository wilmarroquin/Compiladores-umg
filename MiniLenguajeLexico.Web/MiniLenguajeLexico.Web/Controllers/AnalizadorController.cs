using Microsoft.AspNetCore.Mvc;
using MiniLenguajeLexico.Application.DTOs;
using MiniLenguajeLexico.Application.Interfaces;
using MiniLenguajeLexico.Web.Models;

namespace MiniLenguajeLexico.Web.Controllers;

public class AnalizadorController : Controller
{
    private readonly IServicioAnalizadorLexico _servicioAnalizadorLexico;
    private readonly ILogger<AnalizadorController> _logger;

    public AnalizadorController(
        IServicioAnalizadorLexico servicioAnalizadorLexico,
        ILogger<AnalizadorController> logger)
    {
        _servicioAnalizadorLexico = servicioAnalizadorLexico;
        _logger = logger;
    }

    [HttpGet("/")]
    [HttpGet("/Analizador")]
    public IActionResult Index()
    {
        return View(new AnalizadorViewModel());
    }

    [HttpGet("/Error")]
    public IActionResult Error()
    {
        return View("~/Views/Shared/Error.cshtml");
    }

    [HttpPost("/Analizador")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(AnalizadorViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.CodigoFuente)
            && (!ModelState.TryGetValue(nameof(model.CodigoFuente), out var entry) || entry.Errors.Count == 0))
        {
            ModelState.AddModelError(nameof(model.CodigoFuente), "El codigo fuente es obligatorio.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            model.Resultado = await _servicioAnalizadorLexico.AnalizarAsync(new SolicitudAnalisisDto
            {
                CodigoFuente = model.CodigoFuente
            });

            if (!string.IsNullOrWhiteSpace(model.Resultado.Advertencia))
            {
                _logger.LogWarning("El analisis se completo con advertencia de persistencia: {Advertencia}", model.Resultado.Advertencia);
            }

            return View(model);
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(nameof(model.CodigoFuente), ex.Message);
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ocurrio un error al analizar el codigo fuente.");
            model.ErrorGeneral = "No se pudo completar el analisis en este momento.";
            return View(model);
        }
    }
}
