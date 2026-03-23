using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using MiniLenguajeLexico.Application.DTOs;
using MiniLenguajeLexico.Application.Interfaces;
using MiniLenguajeLexico.Web.Controllers;
using MiniLenguajeLexico.Web.Models;

namespace MiniLenguajeLexico.Tests;

public class AnalizadorControllerTests
{
    [Fact]
    public void IndexGet_ReturnsViewWithEmptyModel()
    {
        AnalizadorController controller = new(new FakeServicioAnalizadorLexico(), NullLogger<AnalizadorController>.Instance);

        var result = controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<AnalizadorViewModel>(viewResult.Model);
        Assert.Null(model.Resultado);
        Assert.Equal(string.Empty, model.CodigoFuente);
    }

    [Fact]
    public async Task IndexPost_BlankInput_ReturnsValidationErrorAndSkipsService()
    {
        FakeServicioAnalizadorLexico servicio = new();
        AnalizadorController controller = new(servicio, NullLogger<AnalizadorController>.Instance);

        var result = await controller.Index(new AnalizadorViewModel { CodigoFuente = "   " });

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<AnalizadorViewModel>(viewResult.Model);
        Assert.False(controller.ModelState.IsValid);
        Assert.Equal(0, servicio.AnalizarCalls);
        Assert.Contains(controller.ModelState[nameof(AnalizadorViewModel.CodigoFuente)]!.Errors,
            error => error.ErrorMessage == "El codigo fuente es obligatorio.");
        Assert.Equal("   ", model.CodigoFuente);
    }

    [Fact]
    public async Task IndexPost_ValidInput_ReturnsViewWithResult()
    {
        FakeServicioAnalizadorLexico servicio = new
        (
            solicitud => Task.FromResult(new ResultadoAnalisisDto
            {
                IdAnalisis = 42,
                EstadoAnalisis = "Exitoso",
                Tokens =
                [
                    new TokenDto
                    {
                        Lexema = "contador",
                        TipoToken = "Identificador",
                        NumeroLinea = 1,
                        NumeroColumna = 1
                    }
                ]
            })
        );
        AnalizadorController controller = new(servicio, NullLogger<AnalizadorController>.Instance);

        var result = await controller.Index(new AnalizadorViewModel { CodigoFuente = "contador" });

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<AnalizadorViewModel>(viewResult.Model);
        Assert.True(controller.ModelState.IsValid);
        Assert.Equal(1, servicio.AnalizarCalls);
        Assert.NotNull(model.Resultado);
        Assert.Equal(42, model.Resultado!.IdAnalisis);
        Assert.Single(model.Resultado.Tokens);
    }

    [Fact]
    public async Task IndexPost_WhenServiceFails_ReturnsFriendlyError()
    {
        FakeServicioAnalizadorLexico servicio = new(_ => throw new InvalidOperationException("fallo"));
        AnalizadorController controller = new(servicio, NullLogger<AnalizadorController>.Instance);

        var result = await controller.Index(new AnalizadorViewModel { CodigoFuente = "contador" });

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<AnalizadorViewModel>(viewResult.Model);
        Assert.Equal(1, servicio.AnalizarCalls);
        Assert.Null(model.Resultado);
        Assert.Equal("No se pudo completar el analisis en este momento.", model.ErrorGeneral);
    }

    private sealed class FakeServicioAnalizadorLexico : IServicioAnalizadorLexico
    {
        private readonly Func<SolicitudAnalisisDto, Task<ResultadoAnalisisDto>> _analizarAsync;

        public FakeServicioAnalizadorLexico()
            : this(_ => Task.FromResult(new ResultadoAnalisisDto()))
        {
        }

        public FakeServicioAnalizadorLexico(Func<SolicitudAnalisisDto, Task<ResultadoAnalisisDto>> analizarAsync)
        {
            _analizarAsync = analizarAsync;
        }

        public int AnalizarCalls { get; private set; }

        public Task<ResultadoAnalisisDto> AnalizarAsync(SolicitudAnalisisDto solicitud)
        {
            AnalizarCalls++;
            return _analizarAsync(solicitud);
        }
    }
}
