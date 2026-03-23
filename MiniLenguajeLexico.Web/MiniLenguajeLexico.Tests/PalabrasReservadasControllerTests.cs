using Microsoft.AspNetCore.Mvc;
using MiniLenguajeLexico.Application.Interfaces;
using MiniLenguajeLexico.Domain.Entities;
using MiniLenguajeLexico.Web.Controllers;
using MiniLenguajeLexico.Web.Models;

namespace MiniLenguajeLexico.Tests;

public class PalabrasReservadasControllerTests
{
    [Fact]
    public async Task IndexGet_RetornaVistaConListado()
    {
        PalabrasReservadasController controller = new(new FakeServicioPalabrasReservadas());

        var result = await controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<PalabrasReservadasViewModel>(viewResult.Model);
        Assert.NotEmpty(model.Palabras);
    }

    [Fact]
    public async Task IndexPost_Invalido_RetornaErrores()
    {
        FakeServicioPalabrasReservadas servicio = new();
        PalabrasReservadasController controller = new(servicio);
        controller.ModelState.AddModelError(nameof(PalabrasReservadasViewModel.NuevaPalabra), "requerido");

        var result = await controller.Index(new PalabrasReservadasViewModel());

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<PalabrasReservadasViewModel>(viewResult.Model);
        Assert.False(controller.ModelState.IsValid);
        Assert.NotEmpty(model.Palabras);
        Assert.Equal(0, servicio.AddCalls);
    }

    [Fact]
    public async Task IndexPost_Valido_Redirecciona()
    {
        FakeServicioPalabrasReservadas servicio = new();
        PalabrasReservadasController controller = new(servicio);

        var result = await controller.Index(new PalabrasReservadasViewModel { NuevaPalabra = "switch" });

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(PalabrasReservadasController.Index), redirectResult.ActionName);
        Assert.Equal(1, servicio.AddCalls);
    }

    [Fact]
    public async Task EditarGet_CuandoExiste_RetornaVista()
    {
        PalabrasReservadasController controller = new(new FakeServicioPalabrasReservadas());

        var result = await controller.Editar(1);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<EditarPalabraReservadaViewModel>(viewResult.Model);
        Assert.Equal(1, model.IdPalabraReservada);
    }

    [Fact]
    public async Task EditarPost_Valido_Redirecciona()
    {
        FakeServicioPalabrasReservadas servicio = new();
        PalabrasReservadasController controller = new(servicio);

        var result = await controller.Editar(1, new EditarPalabraReservadaViewModel
        {
            IdPalabraReservada = 1,
            Palabra = "while",
            Activo = false
        });

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(PalabrasReservadasController.Editar), redirectResult.ActionName);
        Assert.Equal(1, servicio.UpdateCalls);
    }

    [Fact]
    public async Task EliminarPost_Valido_Redirecciona()
    {
        FakeServicioPalabrasReservadas servicio = new();
        PalabrasReservadasController controller = new(servicio);

        var result = await controller.Eliminar(1);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(PalabrasReservadasController.Index), redirectResult.ActionName);
        Assert.Equal(1, servicio.DeleteCalls);
    }

    private sealed class FakeServicioPalabrasReservadas : IServicioPalabrasReservadas
    {
        private readonly bool _exito;
        private readonly string? _error;
        private readonly List<PalabraReservadaCatalogo> _palabras =
        [
            new() { IdPalabraReservada = 1, Palabra = "if", Activo = true },
            new() { IdPalabraReservada = 2, Palabra = "return", Activo = true }
        ];

        public FakeServicioPalabrasReservadas(bool exito = true, string? error = null)
        {
            _exito = exito;
            _error = error;
        }

        public int AddCalls { get; private set; }
        public int UpdateCalls { get; private set; }
        public int DeleteCalls { get; private set; }

        public Task<(bool Exito, string? Error)> AgregarAsync(string palabra)
        {
            AddCalls++;
            return Task.FromResult((_exito, _error));
        }

        public Task<(bool Exito, string? Error)> ActualizarAsync(int idPalabraReservada, string palabra, bool activo)
        {
            UpdateCalls++;
            return Task.FromResult((_exito, _error));
        }

        public Task<(bool Exito, string? Error)> EliminarAsync(int idPalabraReservada)
        {
            DeleteCalls++;
            return Task.FromResult((_exito, _error));
        }

        public Task<IReadOnlyList<PalabraReservadaCatalogo>> ObtenerCatalogoAsync(bool soloActivas = false)
        {
            IReadOnlyList<PalabraReservadaCatalogo> lista = _palabras
                .Where(item => !soloActivas || item.Activo)
                .ToList();

            return Task.FromResult(lista);
        }

        public Task<PalabraReservadaCatalogo?> ObtenerPorIdAsync(int idPalabraReservada)
        {
            return Task.FromResult(_palabras.SingleOrDefault(item => item.IdPalabraReservada == idPalabraReservada));
        }

        public Task<IReadOnlyList<string>> ObtenerTodasAsync()
        {
            return Task.FromResult<IReadOnlyList<string>>(_palabras.Where(item => item.Activo).Select(item => item.Palabra).ToList());
        }
    }
}
