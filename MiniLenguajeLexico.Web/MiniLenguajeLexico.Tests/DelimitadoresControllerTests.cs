using Microsoft.AspNetCore.Mvc;
using MiniLenguajeLexico.Application.Interfaces;
using MiniLenguajeLexico.Domain.Entities;
using MiniLenguajeLexico.Web.Controllers;
using MiniLenguajeLexico.Web.Models;

namespace MiniLenguajeLexico.Tests;

public class DelimitadoresControllerTests
{
    [Fact]
    public async Task IndexGet_RetornaVistaConListado()
    {
        DelimitadoresController controller = new(new FakeServicioDelimitadores());

        var result = await controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<DelimitadoresViewModel>(viewResult.Model);
        Assert.NotEmpty(model.Delimitadores);
    }

    [Fact]
    public async Task IndexPost_Invalido_RetornaErrores()
    {
        FakeServicioDelimitadores servicio = new();
        DelimitadoresController controller = new(servicio);
        controller.ModelState.AddModelError(nameof(DelimitadoresViewModel.NuevoDelimitador), "requerido");

        var result = await controller.Index(new DelimitadoresViewModel());

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<DelimitadoresViewModel>(viewResult.Model);
        Assert.False(controller.ModelState.IsValid);
        Assert.NotEmpty(model.Delimitadores);
        Assert.Equal(0, servicio.AddCalls);
    }

    [Fact]
    public async Task IndexPost_Valido_Redirecciona()
    {
        FakeServicioDelimitadores servicio = new();
        DelimitadoresController controller = new(servicio);

        var result = await controller.Index(new DelimitadoresViewModel { NuevoDelimitador = "#" });

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(DelimitadoresController.Index), redirectResult.ActionName);
        Assert.Equal(1, servicio.AddCalls);
    }

    [Fact]
    public async Task EditarGet_CuandoExiste_RetornaVista()
    {
        DelimitadoresController controller = new(new FakeServicioDelimitadores());

        var result = await controller.Editar(1);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<EditarDelimitadorViewModel>(viewResult.Model);
        Assert.Equal(1, model.IdDelimitador);
    }

    [Fact]
    public async Task EditarPost_Valido_Redirecciona()
    {
        FakeServicioDelimitadores servicio = new();
        DelimitadoresController controller = new(servicio);

        var result = await controller.Editar(1, new EditarDelimitadorViewModel
        {
            IdDelimitador = 1,
            Simbolo = "]",
            Activo = false
        });

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(DelimitadoresController.Editar), redirectResult.ActionName);
        Assert.Equal(1, servicio.UpdateCalls);
    }

    [Fact]
    public async Task EliminarPost_Valido_Redirecciona()
    {
        FakeServicioDelimitadores servicio = new();
        DelimitadoresController controller = new(servicio);

        var result = await controller.Eliminar(1);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(DelimitadoresController.Index), redirectResult.ActionName);
        Assert.Equal(1, servicio.DeleteCalls);
    }

    private sealed class FakeServicioDelimitadores : IServicioDelimitadores
    {
        private readonly bool _exito;
        private readonly string? _error;
        private readonly List<DelimitadorCatalogo> _delimitadores =
        [
            new() { IdDelimitador = 1, Simbolo = "(", Activo = true },
            new() { IdDelimitador = 2, Simbolo = ";", Activo = true }
        ];

        public FakeServicioDelimitadores(bool exito = true, string? error = null)
        {
            _exito = exito;
            _error = error;
        }

        public int AddCalls { get; private set; }
        public int UpdateCalls { get; private set; }
        public int DeleteCalls { get; private set; }

        public Task<(bool Exito, string? Error)> AgregarAsync(string delimitador)
        {
            AddCalls++;
            return Task.FromResult((_exito, _error));
        }

        public Task<(bool Exito, string? Error)> ActualizarAsync(int idDelimitador, string delimitador, bool activo)
        {
            UpdateCalls++;
            return Task.FromResult((_exito, _error));
        }

        public Task<(bool Exito, string? Error)> EliminarAsync(int idDelimitador)
        {
            DeleteCalls++;
            return Task.FromResult((_exito, _error));
        }

        public Task<IReadOnlyList<DelimitadorCatalogo>> ObtenerCatalogoAsync(bool soloActivos = false)
        {
            IReadOnlyList<DelimitadorCatalogo> lista = _delimitadores
                .Where(item => !soloActivos || item.Activo)
                .ToList();

            return Task.FromResult(lista);
        }

        public Task<DelimitadorCatalogo?> ObtenerPorIdAsync(int idDelimitador)
        {
            return Task.FromResult(_delimitadores.SingleOrDefault(item => item.IdDelimitador == idDelimitador));
        }

        public Task<IReadOnlyList<string>> ObtenerTodosAsync()
        {
            return Task.FromResult<IReadOnlyList<string>>(_delimitadores.Where(item => item.Activo).Select(item => item.Simbolo).ToList());
        }
    }
}
