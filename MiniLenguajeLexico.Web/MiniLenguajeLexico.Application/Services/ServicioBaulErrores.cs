using MiniLenguajeLexico.Application.Interfaces;
using MiniLenguajeLexico.Domain.Entities;

namespace MiniLenguajeLexico.Application.Services;

public class ServicioBaulErrores : IServicioBaulErrores
{
    private readonly IRepositorioConfiguracionLexica _repositorioConfiguracionLexica;

    public ServicioBaulErrores(IRepositorioConfiguracionLexica repositorioConfiguracionLexica)
    {
        _repositorioConfiguracionLexica = repositorioConfiguracionLexica;
    }

    public async Task<IReadOnlyList<ErrorCatalogo>> ObtenerTodosAsync(bool soloActivos = false)
    {
        return await _repositorioConfiguracionLexica.ObtenerErroresCatalogoAsync(soloActivos);
    }

    public async Task<ErrorCatalogo?> ObtenerPorIdAsync(int idErrorCatalogo)
    {
        return await _repositorioConfiguracionLexica.ObtenerErrorCatalogoPorIdAsync(idErrorCatalogo);
    }

    public async Task<(bool Exito, string? Error)> AgregarAsync(ErrorCatalogo errorCatalogo)
    {
        var validacion = Validar(errorCatalogo);
        if (validacion is not null)
        {
            return (false, validacion);
        }

        var existentes = await ObtenerTodosAsync();
        if (existentes.Any(item => string.Equals(item.CodigoError, errorCatalogo.CodigoError.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            return (false, "El codigo de error ya existe.");
        }

        try
        {
            await _repositorioConfiguracionLexica.AgregarErrorCatalogoAsync(Normalizar(errorCatalogo));
            return (true, null);
        }
        catch
        {
            return (false, "No se pudo guardar el error en la base de datos.");
        }
    }

    public async Task<(bool Exito, string? Error)> ActualizarAsync(ErrorCatalogo errorCatalogo)
    {
        var validacion = Validar(errorCatalogo);
        if (validacion is not null)
        {
            return (false, validacion);
        }

        var existentes = await ObtenerTodosAsync();
        if (existentes.Any(item =>
                item.IdErrorCatalogo != errorCatalogo.IdErrorCatalogo &&
                string.Equals(item.CodigoError, errorCatalogo.CodigoError.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            return (false, "El codigo de error ya existe.");
        }

        try
        {
            await _repositorioConfiguracionLexica.ActualizarErrorCatalogoAsync(Normalizar(errorCatalogo));
            return (true, null);
        }
        catch
        {
            return (false, "No se pudo actualizar el error en la base de datos.");
        }
    }

    public async Task<(bool Exito, string? Error)> EliminarAsync(int idErrorCatalogo)
    {
        var actual = await ObtenerPorIdAsync(idErrorCatalogo);
        if (actual is null)
        {
            return (false, "El error indicado no existe.");
        }

        try
        {
            await _repositorioConfiguracionLexica.EliminarErrorCatalogoAsync(idErrorCatalogo);
            return (true, null);
        }
        catch
        {
            return (false, "No se pudo eliminar el error de la base de datos.");
        }
    }

    private static ErrorCatalogo Normalizar(ErrorCatalogo errorCatalogo)
    {
        return new ErrorCatalogo
        {
            IdErrorCatalogo = errorCatalogo.IdErrorCatalogo,
            CodigoError = (errorCatalogo.CodigoError ?? string.Empty).Trim(),
            NombreError = (errorCatalogo.NombreError ?? string.Empty).Trim(),
            DescripcionError = (errorCatalogo.DescripcionError ?? string.Empty).Trim(),
            TipoError = (errorCatalogo.TipoError ?? string.Empty).Trim(),
            Activo = errorCatalogo.Activo
        };
    }

    private static string? Validar(ErrorCatalogo errorCatalogo)
    {
        if (string.IsNullOrWhiteSpace(errorCatalogo.CodigoError))
        {
            return "El codigo de error es obligatorio.";
        }

        if (!errorCatalogo.CodigoError.Trim().All(caracter => char.IsLetterOrDigit(caracter) || caracter is '-' or '_'))
        {
            return "El codigo de error solo puede contener letras, numeros, guion o guion bajo.";
        }

        if (string.IsNullOrWhiteSpace(errorCatalogo.NombreError))
        {
            return "El nombre del error es obligatorio.";
        }

        if (string.IsNullOrWhiteSpace(errorCatalogo.DescripcionError))
        {
            return "La descripcion del error es obligatoria.";
        }

        if (string.IsNullOrWhiteSpace(errorCatalogo.TipoError))
        {
            return "El tipo de error es obligatorio.";
        }

        return null;
    }
}
