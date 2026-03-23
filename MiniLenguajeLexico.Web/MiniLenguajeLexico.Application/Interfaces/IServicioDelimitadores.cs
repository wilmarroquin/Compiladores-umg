using MiniLenguajeLexico.Domain.Entities;

namespace MiniLenguajeLexico.Application.Interfaces;

public interface IServicioDelimitadores
{
    Task<IReadOnlyList<string>> ObtenerTodosAsync();
    Task<IReadOnlyList<DelimitadorCatalogo>> ObtenerCatalogoAsync(bool soloActivos = false);
    Task<DelimitadorCatalogo?> ObtenerPorIdAsync(int idDelimitador);
    Task<(bool Exito, string? Error)> AgregarAsync(string delimitador);
    Task<(bool Exito, string? Error)> ActualizarAsync(int idDelimitador, string delimitador, bool activo);
    Task<(bool Exito, string? Error)> EliminarAsync(int idDelimitador);
}
