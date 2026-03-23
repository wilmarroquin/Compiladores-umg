using MiniLenguajeLexico.Application.Services;

namespace MiniLenguajeLexico.Tests;

public class ServicioPalabrasReservadasTests
{
    [Fact]
    public async Task ObtenerTodasAsync_ContienePalabrasBase()
    {
        ServicioPalabrasReservadas servicio = new(new FakeRepositorioConfiguracionLexica());

        var resultado = await servicio.ObtenerTodasAsync();

        Assert.Contains("if", resultado);
        Assert.Contains("return", resultado);
    }

    [Fact]
    public async Task ObtenerTodasAsync_CuandoFallaLaBase_LanzaExcepcion()
    {
        ServicioPalabrasReservadas servicio = new(new ThrowingRepositorioConfiguracionLexica());

        await Assert.ThrowsAsync<InvalidOperationException>(() => servicio.ObtenerTodasAsync());
    }

    [Fact]
    public async Task AgregarAsync_CuandoEsNueva_LaRegistra()
    {
        FakeRepositorioConfiguracionLexica repositorio = new();
        ServicioPalabrasReservadas servicio = new(repositorio);
        string palabra = $"custom_{Guid.NewGuid():N}";

        var resultado = await servicio.AgregarAsync(palabra);
        var palabras = await servicio.ObtenerTodasAsync();

        Assert.True(resultado.Exito);
        Assert.Contains(palabra, palabras);
    }

    [Fact]
    public async Task ActualizarAsync_CuandoExiste_AplicaCambios()
    {
        FakeRepositorioConfiguracionLexica repositorio = new();
        ServicioPalabrasReservadas servicio = new(repositorio);

        await servicio.AgregarAsync("temporal");
        var creada = (await servicio.ObtenerCatalogoAsync()).Single(item => item.Palabra == "temporal");

        var resultado = await servicio.ActualizarAsync(creada.IdPalabraReservada, "temporal_editada", false);
        var actualizada = await servicio.ObtenerPorIdAsync(creada.IdPalabraReservada);

        Assert.True(resultado.Exito);
        Assert.NotNull(actualizada);
        Assert.Equal("temporal_editada", actualizada!.Palabra);
        Assert.False(actualizada.Activo);
    }

    [Fact]
    public async Task EliminarAsync_CuandoExiste_LaQuitaDelCatalogo()
    {
        FakeRepositorioConfiguracionLexica repositorio = new();
        ServicioPalabrasReservadas servicio = new(repositorio);

        await servicio.AgregarAsync("temp_delete");
        var creada = (await servicio.ObtenerCatalogoAsync()).Single(item => item.Palabra == "temp_delete");

        var resultado = await servicio.EliminarAsync(creada.IdPalabraReservada);

        Assert.True(resultado.Exito);
        Assert.DoesNotContain(await servicio.ObtenerCatalogoAsync(), item => item.IdPalabraReservada == creada.IdPalabraReservada);
    }
}
