using MiniLenguajeLexico.Application.Interfaces;
using MiniLenguajeLexico.Domain.Entities;

namespace MiniLenguajeLexico.Application.Services;

public class ServicioPalabrasReservadas : IServicioPalabrasReservadas
{
    private readonly IRepositorioConfiguracionLexica _repositorioConfiguracionLexica;

    public ServicioPalabrasReservadas(IRepositorioConfiguracionLexica repositorioConfiguracionLexica)
    {
        _repositorioConfiguracionLexica = repositorioConfiguracionLexica;
    }

    public async Task<IReadOnlyList<string>> ObtenerTodasAsync()
    {
        var palabras = await ObtenerCatalogoAsync(soloActivas: true);

        return palabras
            .Select(item => item.Palabra)
            .OrderBy(item => item, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public async Task<IReadOnlyList<PalabraReservadaCatalogo>> ObtenerCatalogoAsync(bool soloActivas = false)
    {
        var palabras = await _repositorioConfiguracionLexica.ObtenerPalabrasReservadasAsync(soloActivas);

        return palabras
            .OrderBy(item => item.Palabra, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public async Task<PalabraReservadaCatalogo?> ObtenerPorIdAsync(int idPalabraReservada)
    {
        return await _repositorioConfiguracionLexica.ObtenerPalabraReservadaPorIdAsync(idPalabraReservada);
    }

    public async Task<(bool Exito, string? Error)> AgregarAsync(string palabra)
    {
        string normalizada = Normalizar(palabra);

        var validacion = Validar(normalizada);
        if (validacion is not null)
        {
            return (false, validacion);
        }

        var existentes = await ObtenerCatalogoAsync();
        if (existentes.Any(item => string.Equals(item.Palabra, normalizada, StringComparison.OrdinalIgnoreCase)))
        {
            return (false, "La palabra reservada ya existe.");
        }

        try
        {
            await _repositorioConfiguracionLexica.AgregarPalabraReservadaAsync(normalizada);
            return (true, null);
        }
        catch
        {
            return (false, "No se pudo guardar la palabra reservada en la base de datos.");
        }
    }

    public async Task<(bool Exito, string? Error)> ActualizarAsync(int idPalabraReservada, string palabra, bool activo)
    {
        string normalizada = Normalizar(palabra);

        var validacion = Validar(normalizada);
        if (validacion is not null)
        {
            return (false, validacion);
        }

        var actual = await ObtenerPorIdAsync(idPalabraReservada);
        if (actual is null)
        {
            return (false, "La palabra reservada indicada no existe.");
        }

        var existentes = await ObtenerCatalogoAsync();
        if (existentes.Any(item =>
                item.IdPalabraReservada != idPalabraReservada &&
                string.Equals(item.Palabra, normalizada, StringComparison.OrdinalIgnoreCase)))
        {
            return (false, "La palabra reservada ya existe.");
        }

        try
        {
            await _repositorioConfiguracionLexica.ActualizarPalabraReservadaAsync(new PalabraReservadaCatalogo
            {
                IdPalabraReservada = idPalabraReservada,
                Palabra = normalizada,
                Activo = activo
            });
            return (true, null);
        }
        catch
        {
            return (false, "No se pudo actualizar la palabra reservada en la base de datos.");
        }
    }

    public async Task<(bool Exito, string? Error)> EliminarAsync(int idPalabraReservada)
    {
        var actual = await ObtenerPorIdAsync(idPalabraReservada);
        if (actual is null)
        {
            return (false, "La palabra reservada indicada no existe.");
        }

        try
        {
            await _repositorioConfiguracionLexica.EliminarPalabraReservadaAsync(idPalabraReservada);
            return (true, null);
        }
        catch
        {
            return (false, "No se pudo eliminar la palabra reservada de la base de datos.");
        }
    }

    private static string Normalizar(string? palabra)
    {
        return (palabra ?? string.Empty).Trim();
    }

    private static string? Validar(string palabra)
    {
        if (string.IsNullOrWhiteSpace(palabra))
        {
            return "La palabra reservada es obligatoria.";
        }

        if (!(char.IsLetter(palabra[0]) || palabra[0] == '_'))
        {
            return "La palabra reservada solo puede contener letras, numeros o guion bajo, y debe iniciar con letra o guion bajo.";
        }

        if (!palabra.All(caracter => char.IsLetterOrDigit(caracter) || caracter == '_'))
        {
            return "La palabra reservada solo puede contener letras, numeros o guion bajo, y debe iniciar con letra o guion bajo.";
        }

        return null;
    }
}
