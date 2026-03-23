using Microsoft.AspNetCore.Mvc;
using MiniLenguajeLexico.Application.Interfaces;
using MiniLenguajeLexico.Domain.Entities;
using MiniLenguajeLexico.Web.Models;

namespace MiniLenguajeLexico.Web.Controllers;

public class BaulErroresController : Controller
{
    private readonly IServicioBaulErrores _servicioBaulErrores;

    public BaulErroresController(IServicioBaulErrores servicioBaulErrores)
    {
        _servicioBaulErrores = servicioBaulErrores;
    }

    [HttpGet("/BaulErrores")]
    public async Task<IActionResult> Index()
    {
        return View(await CrearModeloAsync());
    }

    [HttpPost("/BaulErrores")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(BaulErroresViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Errores = await _servicioBaulErrores.ObtenerTodosAsync();
            return View(model);
        }

        var resultado = await _servicioBaulErrores.AgregarAsync(new ErrorCatalogo
        {
            CodigoError = model.CodigoError,
            NombreError = model.NombreError,
            DescripcionError = model.DescripcionError,
            TipoError = model.TipoError,
            Activo = model.Activo
        });

        if (!resultado.Exito)
        {
            ModelState.AddModelError(nameof(model.CodigoError), resultado.Error ?? "No se pudo agregar el error.");
            model.Errores = await _servicioBaulErrores.ObtenerTodosAsync();
            return View(model);
        }

        EstablecerMensajeExito($"El error '{model.CodigoError.Trim()}' fue agregado correctamente.");
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("/BaulErrores/Editar/{id:int}")]
    public async Task<IActionResult> Editar(int id)
    {
        var error = await _servicioBaulErrores.ObtenerPorIdAsync(id);
        if (error is null)
        {
            return NotFound();
        }

        return View(new EditarErrorCatalogoViewModel
        {
            IdErrorCatalogo = error.IdErrorCatalogo,
            CodigoError = error.CodigoError,
            NombreError = error.NombreError,
            DescripcionError = error.DescripcionError,
            TipoError = error.TipoError,
            Activo = error.Activo,
            MensajeExito = ObtenerMensajeExito(),
            MensajeError = ObtenerMensajeError()
        });
    }

    [HttpPost("/BaulErrores/Editar/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id, EditarErrorCatalogoViewModel model)
    {
        if (id != model.IdErrorCatalogo)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var resultado = await _servicioBaulErrores.ActualizarAsync(new ErrorCatalogo
        {
            IdErrorCatalogo = model.IdErrorCatalogo,
            CodigoError = model.CodigoError,
            NombreError = model.NombreError,
            DescripcionError = model.DescripcionError,
            TipoError = model.TipoError,
            Activo = model.Activo
        });

        if (!resultado.Exito)
        {
            ModelState.AddModelError(nameof(model.CodigoError), resultado.Error ?? "No se pudo actualizar el error.");
            return View(model);
        }

        EstablecerMensajeExito($"El error '{model.CodigoError.Trim()}' fue actualizado correctamente.");
        return RedirectToAction(nameof(Editar), new { id = model.IdErrorCatalogo });
    }

    [HttpPost("/BaulErrores/Eliminar/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Eliminar(int id)
    {
        var error = await _servicioBaulErrores.ObtenerPorIdAsync(id);
        var resultado = await _servicioBaulErrores.EliminarAsync(id);

        if (!resultado.Exito)
        {
            EstablecerMensajeError(resultado.Error ?? "No se pudo eliminar el error.");
            return RedirectToAction(nameof(Index));
        }

        EstablecerMensajeExito($"El error '{error?.CodigoError ?? id.ToString()}' fue eliminado correctamente.");
        return RedirectToAction(nameof(Index));
    }

    private async Task<BaulErroresViewModel> CrearModeloAsync()
    {
        return new BaulErroresViewModel
        {
            Errores = await _servicioBaulErrores.ObtenerTodosAsync(),
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
