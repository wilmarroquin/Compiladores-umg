using MiniLenguajeLexico.Application.Interfaces;
using MiniLenguajeLexico.Domain.Constants;
using MiniLenguajeLexico.Domain.Entities;

namespace MiniLenguajeLexico.Tests;

internal sealed class FakeRepositorioConfiguracionLexica : IRepositorioConfiguracionLexica
{
    private readonly List<PalabraReservadaCatalogo> _palabrasReservadas;
    private readonly List<DelimitadorCatalogo> _delimitadores;
    private readonly List<ErrorCatalogo> _erroresCatalogo;

    public FakeRepositorioConfiguracionLexica(
        IEnumerable<PalabraReservadaCatalogo>? palabrasReservadas = null,
        IEnumerable<DelimitadorCatalogo>? delimitadores = null,
        IEnumerable<ErrorCatalogo>? erroresCatalogo = null)
    {
        _palabrasReservadas = palabrasReservadas?.Select(Clonar).ToList()
            ?? PalabrasReservadas.ObtenerPredeterminadas()
                .Select((palabra, indice) => new PalabraReservadaCatalogo
                {
                    IdPalabraReservada = indice + 1,
                    Palabra = palabra,
                    Activo = true
                })
                .ToList();

        _delimitadores = delimitadores?.Select(Clonar).ToList()
            ?? CatalogoDelimitadores.ObtenerPredeterminados()
                .Select((simbolo, indice) => new DelimitadorCatalogo
                {
                    IdDelimitador = indice + 1,
                    Simbolo = simbolo,
                    Activo = true
                })
                .ToList();

        _erroresCatalogo = erroresCatalogo?.Select(Clonar).ToList()
            ?? BaulErroresCatalogo.ObtenerPredeterminados().Select(Clonar).ToList();
    }

    public Task AgregarDelimitadorAsync(string simbolo)
    {
        _delimitadores.Add(new DelimitadorCatalogo
        {
            IdDelimitador = _delimitadores.Count == 0 ? 1 : _delimitadores.Max(item => item.IdDelimitador) + 1,
            Simbolo = simbolo,
            Activo = true
        });
        return Task.CompletedTask;
    }

    public Task ActualizarDelimitadorAsync(DelimitadorCatalogo delimitador)
    {
        DelimitadorCatalogo? actual = _delimitadores.SingleOrDefault(item => item.IdDelimitador == delimitador.IdDelimitador);
        if (actual is not null)
        {
            actual.Simbolo = delimitador.Simbolo;
            actual.Activo = delimitador.Activo;
        }

        return Task.CompletedTask;
    }

    public Task EliminarDelimitadorAsync(int idDelimitador)
    {
        _delimitadores.RemoveAll(item => item.IdDelimitador == idDelimitador);
        return Task.CompletedTask;
    }

    public Task InicializarCatalogosAsync()
    {
        return Task.CompletedTask;
    }

    public Task AgregarErrorCatalogoAsync(ErrorCatalogo errorCatalogo)
    {
        ErrorCatalogo clon = Clonar(errorCatalogo);
        clon.IdErrorCatalogo = _erroresCatalogo.Count == 0 ? 1 : _erroresCatalogo.Max(item => item.IdErrorCatalogo) + 1;
        _erroresCatalogo.Add(clon);
        return Task.CompletedTask;
    }

    public Task AgregarPalabraReservadaAsync(string palabra)
    {
        _palabrasReservadas.Add(new PalabraReservadaCatalogo
        {
            IdPalabraReservada = _palabrasReservadas.Count == 0 ? 1 : _palabrasReservadas.Max(item => item.IdPalabraReservada) + 1,
            Palabra = palabra,
            Activo = true
        });
        return Task.CompletedTask;
    }

    public Task ActualizarPalabraReservadaAsync(PalabraReservadaCatalogo palabraReservada)
    {
        PalabraReservadaCatalogo? actual = _palabrasReservadas.SingleOrDefault(item => item.IdPalabraReservada == palabraReservada.IdPalabraReservada);
        if (actual is not null)
        {
            actual.Palabra = palabraReservada.Palabra;
            actual.Activo = palabraReservada.Activo;
        }

        return Task.CompletedTask;
    }

    public Task EliminarPalabraReservadaAsync(int idPalabraReservada)
    {
        _palabrasReservadas.RemoveAll(item => item.IdPalabraReservada == idPalabraReservada);
        return Task.CompletedTask;
    }

    public Task ActualizarErrorCatalogoAsync(ErrorCatalogo errorCatalogo)
    {
        ErrorCatalogo? actual = _erroresCatalogo.SingleOrDefault(item => item.IdErrorCatalogo == errorCatalogo.IdErrorCatalogo);
        if (actual is not null)
        {
            actual.CodigoError = errorCatalogo.CodigoError;
            actual.NombreError = errorCatalogo.NombreError;
            actual.DescripcionError = errorCatalogo.DescripcionError;
            actual.TipoError = errorCatalogo.TipoError;
            actual.Activo = errorCatalogo.Activo;
        }

        return Task.CompletedTask;
    }

    public Task EliminarErrorCatalogoAsync(int idErrorCatalogo)
    {
        _erroresCatalogo.RemoveAll(item => item.IdErrorCatalogo == idErrorCatalogo);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<DelimitadorCatalogo>> ObtenerDelimitadoresAsync(bool soloActivos = true)
    {
        IReadOnlyList<DelimitadorCatalogo> lista = _delimitadores
            .Where(item => !soloActivos || item.Activo)
            .Select(Clonar)
            .ToList();

        return Task.FromResult(lista);
    }

    public Task<ErrorCatalogo?> ObtenerErrorCatalogoPorIdAsync(int idErrorCatalogo)
    {
        ErrorCatalogo? error = _erroresCatalogo
            .Where(item => item.IdErrorCatalogo == idErrorCatalogo)
            .Select(Clonar)
            .SingleOrDefault();

        return Task.FromResult(error);
    }

    public Task<DelimitadorCatalogo?> ObtenerDelimitadorPorIdAsync(int idDelimitador)
    {
        DelimitadorCatalogo? delimitador = _delimitadores
            .Where(item => item.IdDelimitador == idDelimitador)
            .Select(Clonar)
            .SingleOrDefault();

        return Task.FromResult(delimitador);
    }

    public Task<IReadOnlyList<ErrorCatalogo>> ObtenerErroresCatalogoAsync(bool soloActivos = false)
    {
        IReadOnlyList<ErrorCatalogo> lista = _erroresCatalogo
            .Where(item => !soloActivos || item.Activo)
            .Select(Clonar)
            .ToList();

        return Task.FromResult(lista);
    }

    public Task<IReadOnlyList<PalabraReservadaCatalogo>> ObtenerPalabrasReservadasAsync(bool soloActivas = true)
    {
        IReadOnlyList<PalabraReservadaCatalogo> lista = _palabrasReservadas
            .Where(item => !soloActivas || item.Activo)
            .Select(Clonar)
            .ToList();

        return Task.FromResult(lista);
    }

    public Task<PalabraReservadaCatalogo?> ObtenerPalabraReservadaPorIdAsync(int idPalabraReservada)
    {
        PalabraReservadaCatalogo? palabra = _palabrasReservadas
            .Where(item => item.IdPalabraReservada == idPalabraReservada)
            .Select(Clonar)
            .SingleOrDefault();

        return Task.FromResult(palabra);
    }

    private static PalabraReservadaCatalogo Clonar(PalabraReservadaCatalogo palabra)
    {
        return new PalabraReservadaCatalogo
        {
            IdPalabraReservada = palabra.IdPalabraReservada,
            Palabra = palabra.Palabra,
            Activo = palabra.Activo
        };
    }

    private static DelimitadorCatalogo Clonar(DelimitadorCatalogo delimitador)
    {
        return new DelimitadorCatalogo
        {
            IdDelimitador = delimitador.IdDelimitador,
            Simbolo = delimitador.Simbolo,
            Activo = delimitador.Activo
        };
    }

    private static ErrorCatalogo Clonar(ErrorCatalogo error)
    {
        return new ErrorCatalogo
        {
            IdErrorCatalogo = error.IdErrorCatalogo,
            CodigoError = error.CodigoError,
            NombreError = error.NombreError,
            DescripcionError = error.DescripcionError,
            TipoError = error.TipoError,
            Activo = error.Activo
        };
    }
}
