using MiniLenguajeLexico.Application.Services;

namespace MiniLenguajeLexico.Tests;

public class ServicioDelimitadoresTests
{
    [Fact]
    public async Task ObtenerTodosAsync_ContieneDelimitadoresBase()
    {
        ServicioDelimitadores servicio = new(new FakeRepositorioConfiguracionLexica());

        var resultado = await servicio.ObtenerTodosAsync();

        Assert.Contains("(", resultado);
        Assert.Contains(";", resultado);
    }

    [Fact]
    public async Task ObtenerTodosAsync_CuandoFallaLaBase_LanzaExcepcion()
    {
        ServicioDelimitadores servicio = new(new ThrowingRepositorioConfiguracionLexica());

        await Assert.ThrowsAsync<InvalidOperationException>(() => servicio.ObtenerTodosAsync());
    }

    [Fact]
    public async Task AgregarAsync_CuandoEsNuevo_LoRegistra()
    {
        FakeRepositorioConfiguracionLexica repositorio = new();
        ServicioDelimitadores servicio = new(repositorio);
        string delimitador = "#";

        var resultado = await servicio.AgregarAsync(delimitador);
        var delimitadores = await servicio.ObtenerTodosAsync();

        Assert.True(resultado.Exito);
        Assert.Contains(delimitador, delimitadores);
    }

    [Fact]
    public async Task ActualizarAsync_CuandoExiste_AplicaCambios()
    {
        FakeRepositorioConfiguracionLexica repositorio = new();
        ServicioDelimitadores servicio = new(repositorio);

        await servicio.AgregarAsync("#");
        var creado = (await servicio.ObtenerCatalogoAsync()).Single(item => item.Simbolo == "#");

        var resultado = await servicio.ActualizarAsync(creado.IdDelimitador, "$", false);
        var actualizado = await servicio.ObtenerPorIdAsync(creado.IdDelimitador);

        Assert.True(resultado.Exito);
        Assert.NotNull(actualizado);
        Assert.Equal("$", actualizado!.Simbolo);
        Assert.False(actualizado.Activo);
    }

    [Fact]
    public async Task EliminarAsync_CuandoExiste_LoQuitaDelCatalogo()
    {
        FakeRepositorioConfiguracionLexica repositorio = new();
        ServicioDelimitadores servicio = new(repositorio);

        await servicio.AgregarAsync("#");
        var creado = (await servicio.ObtenerCatalogoAsync()).Single(item => item.Simbolo == "#");

        var resultado = await servicio.EliminarAsync(creado.IdDelimitador);

        Assert.True(resultado.Exito);
        Assert.DoesNotContain(await servicio.ObtenerCatalogoAsync(), item => item.IdDelimitador == creado.IdDelimitador);
    }
}
