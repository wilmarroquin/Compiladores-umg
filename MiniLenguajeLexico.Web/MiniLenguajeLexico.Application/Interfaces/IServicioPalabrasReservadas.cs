using MiniLenguajeLexico.Domain.Entities;

namespace MiniLenguajeLexico.Application.Interfaces;

public interface IServicioPalabrasReservadas
{
    Task<IReadOnlyList<string>> ObtenerTodasAsync();
    Task<IReadOnlyList<PalabraReservadaCatalogo>> ObtenerCatalogoAsync(bool soloActivas = false);
    Task<PalabraReservadaCatalogo?> ObtenerPorIdAsync(int idPalabraReservada);
    Task<(bool Exito, string? Error)> AgregarAsync(string palabra);
    Task<(bool Exito, string? Error)> ActualizarAsync(int idPalabraReservada, string palabra, bool activo);
    Task<(bool Exito, string? Error)> EliminarAsync(int idPalabraReservada);
}
