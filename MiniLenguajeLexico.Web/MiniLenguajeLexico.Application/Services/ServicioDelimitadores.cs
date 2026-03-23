using MiniLenguajeLexico.Application.Interfaces;
using MiniLenguajeLexico.Domain.Entities;

namespace MiniLenguajeLexico.Application.Services;

public class ServicioDelimitadores : IServicioDelimitadores
{
    private const string CaracteresNoPermitidos = "+-*/%=<>!&|^~?:.\"'";
    private readonly IRepositorioConfiguracionLexica _repositorioConfiguracionLexica;

    public ServicioDelimitadores(IRepositorioConfiguracionLexica repositorioConfiguracionLexica)
    {
        _repositorioConfiguracionLexica = repositorioConfiguracionLexica;
    }

    public async Task<IReadOnlyList<string>> ObtenerTodosAsync()
    {
        var delimitadores = await ObtenerCatalogoAsync(soloActivos: true);

        return delimitadores
            .Select(item => item.Simbolo)
            .OrderBy(item => item, StringComparer.Ordinal)
            .ToList();
    }

    public async Task<IReadOnlyList<DelimitadorCatalogo>> ObtenerCatalogoAsync(bool soloActivos = false)
    {
        var delimitadores = await _repositorioConfiguracionLexica.ObtenerDelimitadoresAsync(soloActivos);

        return delimitadores
            .OrderBy(item => item.Simbolo, StringComparer.Ordinal)
            .ToList();
    }

    public async Task<DelimitadorCatalogo?> ObtenerPorIdAsync(int idDelimitador)
    {
        return await _repositorioConfiguracionLexica.ObtenerDelimitadorPorIdAsync(idDelimitador);
    }

    public async Task<(bool Exito, string? Error)> AgregarAsync(string delimitador)
    {
        string normalizado = Normalizar(delimitador);

        var validacion = Validar(normalizado);
        if (validacion is not null)
        {
            return (false, validacion);
        }

        var existentes = await ObtenerCatalogoAsync();
        if (existentes.Any(item => string.Equals(item.Simbolo, normalizado, StringComparison.Ordinal)))
        {
            return (false, "El delimitador ya existe.");
        }

        try
        {
            await _repositorioConfiguracionLexica.AgregarDelimitadorAsync(normalizado);
            return (true, null);
        }
        catch
        {
            return (false, "No se pudo guardar el delimitador en la base de datos.");
        }
    }

    public async Task<(bool Exito, string? Error)> ActualizarAsync(int idDelimitador, string delimitador, bool activo)
    {
        string normalizado = Normalizar(delimitador);

        var validacion = Validar(normalizado);
        if (validacion is not null)
        {
            return (false, validacion);
        }

        var actual = await ObtenerPorIdAsync(idDelimitador);
        if (actual is null)
        {
            return (false, "El delimitador indicado no existe.");
        }

        var existentes = await ObtenerCatalogoAsync();
        if (existentes.Any(item =>
                item.IdDelimitador != idDelimitador &&
                string.Equals(item.Simbolo, normalizado, StringComparison.Ordinal)))
        {
            return (false, "El delimitador ya existe.");
        }

        try
        {
            await _repositorioConfiguracionLexica.ActualizarDelimitadorAsync(new DelimitadorCatalogo
            {
                IdDelimitador = idDelimitador,
                Simbolo = normalizado,
                Activo = activo
            });
            return (true, null);
        }
        catch
        {
            return (false, "No se pudo actualizar el delimitador en la base de datos.");
        }
    }

    public async Task<(bool Exito, string? Error)> EliminarAsync(int idDelimitador)
    {
        var actual = await ObtenerPorIdAsync(idDelimitador);
        if (actual is null)
        {
            return (false, "El delimitador indicado no existe.");
        }

        try
        {
            await _repositorioConfiguracionLexica.EliminarDelimitadorAsync(idDelimitador);
            return (true, null);
        }
        catch
        {
            return (false, "No se pudo eliminar el delimitador de la base de datos.");
        }
    }

    private static string Normalizar(string? delimitador)
    {
        return (delimitador ?? string.Empty).Trim();
    }

    private static string? Validar(string delimitador)
    {
        if (string.IsNullOrWhiteSpace(delimitador))
        {
            return "El delimitador es obligatorio.";
        }

        if (delimitador.Length != 1)
        {
            return "El delimitador debe contener un solo simbolo.";
        }

        char simbolo = delimitador[0];
        if (char.IsLetterOrDigit(simbolo) || simbolo == '_')
        {
            return "El delimitador no puede ser una letra, numero o guion bajo.";
        }

        if (char.IsWhiteSpace(simbolo) || CaracteresNoPermitidos.Contains(simbolo))
        {
            return "El simbolo indicado no puede registrarse como delimitador.";
        }

        return null;
    }
}
