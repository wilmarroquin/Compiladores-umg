using MiniLenguajeLexico.Application.DTOs;
using MiniLenguajeLexico.Application.Interfaces;
using MiniLenguajeLexico.Domain.Entities;
using MiniLenguajeLexico.Domain.Enums;

namespace MiniLenguajeLexico.Application.Services;

public class ServicioAnalizadorLexico : IServicioAnalizadorLexico
{
    private readonly IRepositorioAnalisis _repositorioAnalisis;
    private readonly IServicioPalabrasReservadas _servicioPalabrasReservadas;
    private readonly IServicioDelimitadores _servicioDelimitadores;
    private readonly IServicioBaulErrores _servicioBaulErrores;

    public ServicioAnalizadorLexico(
        IRepositorioAnalisis repositorioAnalisis,
        IServicioPalabrasReservadas servicioPalabrasReservadas,
        IServicioDelimitadores servicioDelimitadores,
        IServicioBaulErrores servicioBaulErrores)
    {
        _repositorioAnalisis = repositorioAnalisis;
        _servicioPalabrasReservadas = servicioPalabrasReservadas;
        _servicioDelimitadores = servicioDelimitadores;
        _servicioBaulErrores = servicioBaulErrores;
    }

    public async Task<ResultadoAnalisisDto> AnalizarAsync(SolicitudAnalisisDto solicitud)
    {
        if (string.IsNullOrWhiteSpace(solicitud.CodigoFuente))
        {
            throw new ArgumentException("El codigo fuente es obligatorio.");
        }

        var palabrasReservadas = await ObtenerPalabrasReservadasAsync();
        var delimitadores = await ObtenerDelimitadoresAsync();
        var catalogoErrores = await ObtenerCatalogoErroresAsync();

        AnalizadorLexicoMotor motor = new(palabrasReservadas, delimitadores, catalogoErrores);
        var resultadoMotor = motor.Analizar(solicitud.CodigoFuente);

        Analisis analisis = new()
        {
            EstadoAnalisis = resultadoMotor.Errores.Count == 0
                ? EstadoAnalisis.Exitoso.ToString()
                : EstadoAnalisis.ConErrores.ToString(),
            Tokens = resultadoMotor.Tokens.Select(t => new TokenAnalisis
            {
                Lexema = t.Lexema,
                TipoToken = t.TipoToken,
                NumeroLinea = t.NumeroLinea,
                NumeroColumna = t.NumeroColumna
            }).ToList(),
            Errores = resultadoMotor.Errores.Select(e => new ErrorAnalisis
            {
                IdErrorCatalogo = e.IdErrorCatalogo,
                CodigoError = e.CodigoError,
                MensajeError = e.MensajeError,
                Lexema = e.Lexema,
                NumeroLinea = e.NumeroLinea,
                NumeroColumna = e.NumeroColumna
            }).ToList()
        };

        ResultadoAnalisisDto resultado = new()
        {
            IdAnalisis = 0,
            FuePersistido = false,
            EstadoAnalisis = analisis.EstadoAnalisis,
            Tokens = analisis.Tokens.Select(t => new TokenDto
            {
                Lexema = t.Lexema,
                TipoToken = t.TipoToken,
                NumeroLinea = t.NumeroLinea,
                NumeroColumna = t.NumeroColumna
            }).ToList(),
            Errores = analisis.Errores.Select(e => new ErrorAnalisisDto
            {
                IdErrorCatalogo = e.IdErrorCatalogo,
                CodigoError = e.CodigoError,
                MensajeError = e.MensajeError,
                Lexema = e.Lexema,
                NumeroLinea = e.NumeroLinea,
                NumeroColumna = e.NumeroColumna
            }).ToList()
        };

        try
        {
            resultado.IdAnalisis = await _repositorioAnalisis.GuardarAnalisisAsync(analisis);
            resultado.FuePersistido = true;
        }
        catch
        {
            resultado.Advertencia = "El analisis se proceso correctamente, pero no pudo guardarse en la base de datos.";
        }

        return resultado;
    }

    private async Task<IReadOnlyList<string>> ObtenerPalabrasReservadasAsync()
    {
        return await _servicioPalabrasReservadas.ObtenerTodasAsync();
    }

    private async Task<IReadOnlyList<string>> ObtenerDelimitadoresAsync()
    {
        return await _servicioDelimitadores.ObtenerTodosAsync();
    }

    private async Task<IReadOnlyList<ErrorCatalogo>> ObtenerCatalogoErroresAsync()
    {
        return await _servicioBaulErrores.ObtenerTodosAsync(soloActivos: true);
    }
}
