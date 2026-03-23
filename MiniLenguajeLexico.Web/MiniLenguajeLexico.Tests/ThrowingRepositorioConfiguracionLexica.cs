using MiniLenguajeLexico.Application.Interfaces;
using MiniLenguajeLexico.Domain.Entities;

namespace MiniLenguajeLexico.Tests;

internal sealed class ThrowingRepositorioConfiguracionLexica : IRepositorioConfiguracionLexica
{
    public Task AgregarDelimitadorAsync(string simbolo)
    {
        throw new InvalidOperationException("db down");
    }

    public Task ActualizarDelimitadorAsync(DelimitadorCatalogo delimitador)
    {
        throw new InvalidOperationException("db down");
    }

    public Task EliminarDelimitadorAsync(int idDelimitador)
    {
        throw new InvalidOperationException("db down");
    }

    public Task InicializarCatalogosAsync()
    {
        throw new InvalidOperationException("db down");
    }

    public Task AgregarErrorCatalogoAsync(ErrorCatalogo errorCatalogo)
    {
        throw new InvalidOperationException("db down");
    }

    public Task AgregarPalabraReservadaAsync(string palabra)
    {
        throw new InvalidOperationException("db down");
    }

    public Task ActualizarPalabraReservadaAsync(PalabraReservadaCatalogo palabraReservada)
    {
        throw new InvalidOperationException("db down");
    }

    public Task EliminarPalabraReservadaAsync(int idPalabraReservada)
    {
        throw new InvalidOperationException("db down");
    }

    public Task ActualizarErrorCatalogoAsync(ErrorCatalogo errorCatalogo)
    {
        throw new InvalidOperationException("db down");
    }

    public Task EliminarErrorCatalogoAsync(int idErrorCatalogo)
    {
        throw new InvalidOperationException("db down");
    }

    public Task<IReadOnlyList<DelimitadorCatalogo>> ObtenerDelimitadoresAsync(bool soloActivos = true)
    {
        throw new InvalidOperationException("db down");
    }

    public Task<ErrorCatalogo?> ObtenerErrorCatalogoPorIdAsync(int idErrorCatalogo)
    {
        throw new InvalidOperationException("db down");
    }

    public Task<DelimitadorCatalogo?> ObtenerDelimitadorPorIdAsync(int idDelimitador)
    {
        throw new InvalidOperationException("db down");
    }

    public Task<IReadOnlyList<ErrorCatalogo>> ObtenerErroresCatalogoAsync(bool soloActivos = false)
    {
        throw new InvalidOperationException("db down");
    }

    public Task<IReadOnlyList<PalabraReservadaCatalogo>> ObtenerPalabrasReservadasAsync(bool soloActivas = true)
    {
        throw new InvalidOperationException("db down");
    }

    public Task<PalabraReservadaCatalogo?> ObtenerPalabraReservadaPorIdAsync(int idPalabraReservada)
    {
        throw new InvalidOperationException("db down");
    }
}
