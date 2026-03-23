using Microsoft.AspNetCore.Mvc;
using MiniLenguajeLexico.Application.Interfaces;
using MiniLenguajeLexico.Web.Models;

namespace MiniLenguajeLexico.Web.Controllers;

public class PalabrasReservadasController : Controller
{
    private readonly IServicioPalabrasReservadas _servicioPalabrasReservadas;

    public PalabrasReservadasController(IServicioPalabrasReservadas servicioPalabrasReservadas)
    {
        _servicioPalabrasReservadas = servicioPalabrasReservadas;
    }

    [HttpGet("/PalabrasReservadas")]
    public async Task<IActionResult> Index()
    {
        return View(await CrearModeloAsync());
    }

    [HttpPost("/PalabrasReservadas")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(PalabrasReservadasViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Palabras = await _servicioPalabrasReservadas.ObtenerCatalogoAsync();
            return View(model);
        }

        var resultado = await _servicioPalabrasReservadas.AgregarAsync(model.NuevaPalabra);
        if (!resultado.Exito)
        {
            ModelState.AddModelError(nameof(model.NuevaPalabra), resultado.Error ?? "No se pudo agregar la palabra reservada.");
            model.Palabras = await _servicioPalabrasReservadas.ObtenerCatalogoAsync();
            return View(model);
        }

        EstablecerMensajeExito($"La palabra reservada '{model.NuevaPalabra.Trim()}' fue agregada correctamente.");
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("/PalabrasReservadas/Editar/{id:int}")]
    public async Task<IActionResult> Editar(int id)
    {
        var palabra = await _servicioPalabrasReservadas.ObtenerPorIdAsync(id);
        if (palabra is null)
        {
            return NotFound();
        }

        return View(new EditarPalabraReservadaViewModel
        {
            IdPalabraReservada = palabra.IdPalabraReservada,
            Palabra = palabra.Palabra,
            Activo = palabra.Activo,
            MensajeExito = ObtenerMensajeExito(),
            MensajeError = ObtenerMensajeError()
        });
    }

    [HttpPost("/PalabrasReservadas/Editar/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id, EditarPalabraReservadaViewModel model)
    {
        if (id != model.IdPalabraReservada)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var resultado = await _servicioPalabrasReservadas.ActualizarAsync(model.IdPalabraReservada, model.Palabra, model.Activo);
        if (!resultado.Exito)
        {
            ModelState.AddModelError(nameof(model.Palabra), resultado.Error ?? "No se pudo actualizar la palabra reservada.");
            return View(model);
        }

        EstablecerMensajeExito($"La palabra reservada '{model.Palabra.Trim()}' fue actualizada correctamente.");
        return RedirectToAction(nameof(Editar), new { id = model.IdPalabraReservada });
    }

    [HttpPost("/PalabrasReservadas/Eliminar/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Eliminar(int id)
    {
        var palabra = await _servicioPalabrasReservadas.ObtenerPorIdAsync(id);
        var resultado = await _servicioPalabrasReservadas.EliminarAsync(id);

        if (!resultado.Exito)
        {
            EstablecerMensajeError(resultado.Error ?? "No se pudo eliminar la palabra reservada.");
            return RedirectToAction(nameof(Index));
        }

        EstablecerMensajeExito($"La palabra reservada '{palabra?.Palabra ?? id.ToString()}' fue eliminada correctamente.");
        return RedirectToAction(nameof(Index));
    }

    private async Task<PalabrasReservadasViewModel> CrearModeloAsync()
    {
        return new PalabrasReservadasViewModel
        {
            Palabras = await _servicioPalabrasReservadas.ObtenerCatalogoAsync(),
            MensajeExito = ObtenerMensajeExito(),
            MensajeError = ObtenerMensajeError()
        };
    }

    private string? ObtenerMensajeExito()
    {
        return TempData == null ? null : TempData["MensajeExito"] as string;
    }

    private string? ObtenerMensajeError()
    {
        return TempData == null ? null : TempData["MensajeError"] as string;
    }

    private void EstablecerMensajeExito(string mensaje)
    {
        if (TempData != null)
        {
            TempData["MensajeExito"] = mensaje;
        }
    }

    private void EstablecerMensajeError(string mensaje)
    {
        if (TempData != null)
        {
            TempData["MensajeError"] = mensaje;
        }
    }
}
