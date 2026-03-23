using MiniLenguajeLexico.Domain.Entities;

namespace MiniLenguajeLexico.Application.Interfaces;

public interface IServicioBaulErrores
{
    Task<IReadOnlyList<ErrorCatalogo>> ObtenerTodosAsync(bool soloActivos = false);
    Task<ErrorCatalogo?> ObtenerPorIdAsync(int idErrorCatalogo);
    Task<(bool Exito, string? Error)> AgregarAsync(ErrorCatalogo errorCatalogo);
    Task<(bool Exito, string? Error)> ActualizarAsync(ErrorCatalogo errorCatalogo);
    Task<(bool Exito, string? Error)> EliminarAsync(int idErrorCatalogo);
}
