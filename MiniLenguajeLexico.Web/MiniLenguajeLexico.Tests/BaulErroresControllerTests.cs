using Microsoft.AspNetCore.Mvc;
using MiniLenguajeLexico.Application.Interfaces;
using MiniLenguajeLexico.Domain.Entities;
using MiniLenguajeLexico.Web.Controllers;
using MiniLenguajeLexico.Web.Models;

namespace MiniLenguajeLexico.Tests;

public class BaulErroresControllerTests
{
    [Fact]
    public async Task IndexGet_RetornaVistaConErrores()
    {
        BaulErroresController controller = new(new FakeServicioBaulErrores());

        var result = await controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<BaulErroresViewModel>(viewResult.Model);
        Assert.NotEmpty(model.Errores);
    }

    [Fact]
    public async Task IndexPost_Invalido_RetornaModeloConListado()
    {
        FakeServicioBaulErrores servicio = new();
        BaulErroresController controller = new(servicio);
        controller.ModelState.AddModelError(nameof(BaulErroresViewModel.CodigoError), "requerido");

        var result = await controller.Index(new BaulErroresViewModel());

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<BaulErroresViewModel>(viewResult.Model);
        Assert.False(controller.ModelState.IsValid);
        Assert.NotEmpty(model.Errores);
        Assert.Equal(0, servicio.AddCalls);
    }

    [Fact]
    public async Task IndexPost_Valido_Redirecciona()
    {
        FakeServicioBaulErrores servicio = new();
        BaulErroresController controller = new(servicio);

        var result = await controller.Index(new BaulErroresViewModel
        {
            CodigoError = "LEX900",
            NombreError = "Nuevo error",
            DescripcionError = "Descripcion nueva",
            TipoError = "Lexico",
            Activo = true
        });

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(BaulErroresController.Index), redirectResult.ActionName);
        Assert.Equal(1, servicio.AddCalls);
    }

    [Fact]
    public async Task EditarGet_CuandoExiste_RetornaVista()
    {
        BaulErroresController controller = new(new FakeServicioBaulErrores());

        var result = await controller.Editar(1);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<EditarErrorCatalogoViewModel>(viewResult.Model);
        Assert.Equal(1, model.IdErrorCatalogo);
    }

    [Fact]
    public async Task EditarPost_Valido_Redirecciona()
    {
        FakeServicioBaulErrores servicio = new();
        BaulErroresController controller = new(servicio);

        var result = await controller.Editar(1, new EditarErrorCatalogoViewModel
        {
            IdErrorCatalogo = 1,
            CodigoError = "LEX001",
            NombreError = "Error actualizado",
            DescripcionError = "Descripcion actualizada",
            TipoError = "Lexico",
            Activo = false
        });

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(BaulErroresController.Editar), redirectResult.ActionName);
        Assert.Equal(1, servicio.UpdateCalls);
    }

    [Fact]
    public async Task EliminarPost_Valido_Redirecciona()
    {
        FakeServicioBaulErrores servicio = new();
        BaulErroresController controller = new(servicio);

        var result = await controller.Eliminar(1);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(BaulErroresController.Index), redirectResult.ActionName);
        Assert.Equal(1, servicio.DeleteCalls);
    }

    private sealed class FakeServicioBaulErrores : IServicioBaulErrores
    {
        private readonly bool _exito;
        private readonly string? _error;
        private readonly List<ErrorCatalogo> _errores =
        [
            new()
            {
                IdErrorCatalogo = 1,
                CodigoError = "LEX001",
                NombreError = "Simbolo no reconocido",
                DescripcionError = "Descripcion",
                TipoError = "Lexico",
                Activo = true
            }
        ];

        public FakeServicioBaulErrores(bool exito = true, string? error = null)
        {
            _exito = exito;
            _error = error;
        }

        public int AddCalls { get; private set; }
        public int UpdateCalls { get; private set; }
        public int DeleteCalls { get; private set; }

        public Task<(bool Exito, string? Error)> AgregarAsync(ErrorCatalogo errorCatalogo)
        {
            AddCalls++;
            return Task.FromResult((_exito, _error));
        }

        public Task<(bool Exito, string? Error)> ActualizarAsync(ErrorCatalogo errorCatalogo)
        {
            UpdateCalls++;
            return Task.FromResult((_exito, _error));
        }

        public Task<(bool Exito, string? Error)> EliminarAsync(int idErrorCatalogo)
        {
            DeleteCalls++;
            return Task.FromResult((_exito, _error));
        }

        public Task<ErrorCatalogo?> ObtenerPorIdAsync(int idErrorCatalogo)
        {
            return Task.FromResult(_errores.SingleOrDefault(item => item.IdErrorCatalogo == idErrorCatalogo));
        }

        public Task<IReadOnlyList<ErrorCatalogo>> ObtenerTodosAsync(bool soloActivos = false)
        {
            IReadOnlyList<ErrorCatalogo> lista = _errores
                .Where(item => !soloActivos || item.Activo)
                .ToList();

            return Task.FromResult(lista);
        }
    }
}
