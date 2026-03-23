using MiniLenguajeLexico.Application.Services;
using MiniLenguajeLexico.Domain.Entities;

namespace MiniLenguajeLexico.Tests;

public class ServicioBaulErroresTests
{
    [Fact]
    public async Task ObtenerTodosAsync_ContieneErroresBase()
    {
        ServicioBaulErrores servicio = new(new FakeRepositorioConfiguracionLexica());

        var resultado = await servicio.ObtenerTodosAsync();

        Assert.Contains(resultado, item => item.CodigoError == "LEX001");
    }

    [Fact]
    public async Task ObtenerTodosAsync_CuandoFallaLaBase_LanzaExcepcion()
    {
        ServicioBaulErrores servicio = new(new ThrowingRepositorioConfiguracionLexica());

        await Assert.ThrowsAsync<InvalidOperationException>(() => servicio.ObtenerTodosAsync());
    }

    [Fact]
    public async Task ObtenerPorIdAsync_CuandoFallaLaBase_LanzaExcepcion()
    {
        ServicioBaulErrores servicio = new(new ThrowingRepositorioConfiguracionLexica());

        await Assert.ThrowsAsync<InvalidOperationException>(() => servicio.ObtenerPorIdAsync(1));
    }

    [Fact]
    public async Task AgregarAsync_CuandoEsNuevo_LoRegistra()
    {
        ServicioBaulErrores servicio = new(new FakeRepositorioConfiguracionLexica());
        string codigo = $"LEX{Guid.NewGuid():N}"[..10].ToUpperInvariant();

        var resultado = await servicio.AgregarAsync(new ErrorCatalogo
        {
            CodigoError = codigo,
            NombreError = "Error personalizado",
            DescripcionError = "Descripcion de prueba",
            TipoError = "Lexico",
            Activo = true
        });

        var errores = await servicio.ObtenerTodosAsync();

        Assert.True(resultado.Exito);
        Assert.Contains(errores, item => item.CodigoError == codigo);
    }

    [Fact]
    public async Task ActualizarAsync_CuandoExiste_AplicaCambios()
    {
        ServicioBaulErrores servicio = new(new FakeRepositorioConfiguracionLexica());
        string codigo = $"LEX{Guid.NewGuid():N}"[..10].ToUpperInvariant();

        await servicio.AgregarAsync(new ErrorCatalogo
        {
            CodigoError = codigo,
            NombreError = "Error editable",
            DescripcionError = "Descripcion inicial",
            TipoError = "Lexico",
            Activo = true
        });

        var creado = (await servicio.ObtenerTodosAsync()).Single(item => item.CodigoError == codigo);

        var resultado = await servicio.ActualizarAsync(new ErrorCatalogo
        {
            IdErrorCatalogo = creado.IdErrorCatalogo,
            CodigoError = codigo,
            NombreError = "Error actualizado",
            DescripcionError = "Descripcion actualizada",
            TipoError = "Sintactico",
            Activo = false
        });

        var actualizado = await servicio.ObtenerPorIdAsync(creado.IdErrorCatalogo);

        Assert.True(resultado.Exito);
        Assert.NotNull(actualizado);
        Assert.Equal("Error actualizado", actualizado!.NombreError);
        Assert.Equal("Sintactico", actualizado.TipoError);
        Assert.False(actualizado.Activo);
    }

    [Fact]
    public async Task EliminarAsync_CuandoExiste_LoQuitaDelCatalogo()
    {
        ServicioBaulErrores servicio = new(new FakeRepositorioConfiguracionLexica());
        string codigo = $"LEX{Guid.NewGuid():N}"[..10].ToUpperInvariant();

        await servicio.AgregarAsync(new ErrorCatalogo
        {
            CodigoError = codigo,
            NombreError = "Error eliminable",
            DescripcionError = "Descripcion",
            TipoError = "Lexico",
            Activo = true
        });

        var creado = (await servicio.ObtenerTodosAsync()).Single(item => item.CodigoError == codigo);

        var resultado = await servicio.EliminarAsync(creado.IdErrorCatalogo);

        Assert.True(resultado.Exito);
        Assert.DoesNotContain(await servicio.ObtenerTodosAsync(), item => item.IdErrorCatalogo == creado.IdErrorCatalogo);
    }
}
