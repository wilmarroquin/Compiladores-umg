using Microsoft.AspNetCore.Mvc;
using MiniLenguajeLexico.Application.Interfaces;
using MiniLenguajeLexico.Web.Models;

namespace MiniLenguajeLexico.Web.Controllers;

public class DelimitadoresController : Controller
{
    private readonly IServicioDelimitadores _servicioDelimitadores;

    public DelimitadoresController(IServicioDelimitadores servicioDelimitadores)
    {
        _servicioDelimitadores = servicioDelimitadores;
    }

    [HttpGet("/Delimitadores")]
    public async Task<IActionResult> Index()
    {
        return View(await CrearModeloAsync());
    }

    [HttpPost("/Delimitadores")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(DelimitadoresViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Delimitadores = await _servicioDelimitadores.ObtenerCatalogoAsync();
            return View(model);
        }

        var resultado = await _servicioDelimitadores.AgregarAsync(model.NuevoDelimitador);
        if (!resultado.Exito)
        {
            ModelState.AddModelError(nameof(model.NuevoDelimitador), resultado.Error ?? "No se pudo agregar el delimitador.");
            model.Delimitadores = await _servicioDelimitadores.ObtenerCatalogoAsync();
            return View(model);
        }

        EstablecerMensajeExito($"El delimitador '{model.NuevoDelimitador.Trim()}' fue agregado correctamente.");
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("/Delimitadores/Editar/{id:int}")]
    public async Task<IActionResult> Editar(int id)
    {
        var delimitador = await _servicioDelimitadores.ObtenerPorIdAsync(id);
        if (delimitador is null)
        {
            return NotFound();
        }

        return View(new EditarDelimitadorViewModel
        {
            IdDelimitador = delimitador.IdDelimitador,
            Simbolo = delimitador.Simbolo,
            Activo = delimitador.Activo,
            MensajeExito = ObtenerMensajeExito(),
            MensajeError = ObtenerMensajeError()
        });
    }

    [HttpPost("/Delimitadores/Editar/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id, EditarDelimitadorViewModel model)
    {
        if (id != model.IdDelimitador)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var resultado = await _servicioDelimitadores.ActualizarAsync(model.IdDelimitador, model.Simbolo, model.Activo);
        if (!resultado.Exito)
        {
            ModelState.AddModelError(nameof(model.Simbolo), resultado.Error ?? "No se pudo actualizar el delimitador.");
            return View(model);
        }

        EstablecerMensajeExito($"El delimitador '{model.Simbolo.Trim()}' fue actualizado correctamente.");
        return RedirectToAction(nameof(Editar), new { id = model.IdDelimitador });
    }

    [HttpPost("/Delimitadores/Eliminar/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Eliminar(int id)
    {
        var delimitador = await _servicioDelimitadores.ObtenerPorIdAsync(id);
        var resultado = await _servicioDelimitadores.EliminarAsync(id);

        if (!resultado.Exito)
        {
            EstablecerMensajeError(resultado.Error ?? "No se pudo eliminar el delimitador.");
            return RedirectToAction(nameof(Index));
        }

        EstablecerMensajeExito($"El delimitador '{delimitador?.Simbolo ?? id.ToString()}' fue eliminado correctamente.");
        return RedirectToAction(nameof(Index));
    }

    private async Task<DelimitadoresViewModel> CrearModeloAsync()
    {
        return new DelimitadoresViewModel
        {
            Delimitadores = await _servicioDelimitadores.ObtenerCatalogoAsync(),
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
