using MiniLenguajeLexico.Domain.Entities;

namespace MiniLenguajeLexico.Application.Interfaces;

public interface IRepositorioConfiguracionLexica
{
    Task InicializarCatalogosAsync();

    Task<IReadOnlyList<PalabraReservadaCatalogo>> ObtenerPalabrasReservadasAsync(bool soloActivas = true);
    Task<PalabraReservadaCatalogo?> ObtenerPalabraReservadaPorIdAsync(int idPalabraReservada);
    Task AgregarPalabraReservadaAsync(string palabra);
    Task ActualizarPalabraReservadaAsync(PalabraReservadaCatalogo palabraReservada);
    Task EliminarPalabraReservadaAsync(int idPalabraReservada);

    Task<IReadOnlyList<DelimitadorCatalogo>> ObtenerDelimitadoresAsync(bool soloActivos = true);
    Task<DelimitadorCatalogo?> ObtenerDelimitadorPorIdAsync(int idDelimitador);
    Task AgregarDelimitadorAsync(string simbolo);
    Task ActualizarDelimitadorAsync(DelimitadorCatalogo delimitador);
    Task EliminarDelimitadorAsync(int idDelimitador);

    Task<IReadOnlyList<ErrorCatalogo>> ObtenerErroresCatalogoAsync(bool soloActivos = false);
    Task<ErrorCatalogo?> ObtenerErrorCatalogoPorIdAsync(int idErrorCatalogo);
    Task AgregarErrorCatalogoAsync(ErrorCatalogo errorCatalogo);
    Task ActualizarErrorCatalogoAsync(ErrorCatalogo errorCatalogo);
    Task EliminarErrorCatalogoAsync(int idErrorCatalogo);
}
