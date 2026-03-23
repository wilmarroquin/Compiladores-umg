using MiniLenguajeLexico.Application.DTOs;
using MiniLenguajeLexico.Application.Interfaces;
using MiniLenguajeLexico.Application.Services;
using MiniLenguajeLexico.Domain.Entities;

namespace MiniLenguajeLexico.Tests;

public class ServicioAnalizadorLexicoTests
{
    [Fact]
    public async Task AnalizarAsync_WithValidInput_SavesSuccessfulAnalysis()
    {
        FakeRepositorioAnalisis repositorio = new();
        ServicioAnalizadorLexico servicio = CrearServicio(repositorio);

        var resultado = await servicio.AnalizarAsync(new SolicitudAnalisisDto
        {
            CodigoFuente = "contador = 15;"
        });

        Assert.Equal(1, resultado.IdAnalisis);
        Assert.Equal("Exitoso", resultado.EstadoAnalisis);
        Assert.Empty(resultado.Errores);
        Assert.Equal(4, resultado.Tokens.Count);
        Assert.Collection(
            resultado.Tokens,
            token => Assert.Equal(("contador", "Identificador"), (token.Lexema, token.TipoToken)),
            token => Assert.Equal(("=", "Operador"), (token.Lexema, token.TipoToken)),
            token => Assert.Equal(("15", "Constante"), (token.Lexema, token.TipoToken)),
            token => Assert.Equal((";", "Delimitador"), (token.Lexema, token.TipoToken)));
        Assert.Single(repositorio.AnalisisGuardados);
        Assert.Equal("Exitoso", repositorio.AnalisisGuardados[0].EstadoAnalisis);
    }

    [Fact]
    public async Task AnalizarAsync_WithConfiguredReservedWord_KeepsReservedWordSupport()
    {
        FakeRepositorioAnalisis repositorio = new();
        FakeRepositorioConfiguracionLexica configuracion = new();
        ServicioPalabrasReservadas servicioPalabrasReservadas = new(configuracion);
        ServicioAnalizadorLexico servicio = CrearServicio(repositorio, configuracion);
        string nuevaReservada = $"custom{Guid.NewGuid():N}";

        var registro = await servicioPalabrasReservadas.AgregarAsync(nuevaReservada);
        Assert.True(registro.Exito);

        var resultado = await servicio.AnalizarAsync(new SolicitudAnalisisDto
        {
            CodigoFuente = $"{nuevaReservada} valor;"
        });

        Assert.Collection(
            resultado.Tokens,
            token => Assert.Equal((nuevaReservada, "PalabraReservada"), (token.Lexema, token.TipoToken)),
            token => Assert.Equal(("valor", "Identificador"), (token.Lexema, token.TipoToken)),
            token => Assert.Equal((";", "Delimitador"), (token.Lexema, token.TipoToken)));
    }

    [Fact]
    public async Task AnalizarAsync_WithConfiguredDelimiter_RecognizesItAsDelimiter()
    {
        FakeRepositorioAnalisis repositorio = new();
        FakeRepositorioConfiguracionLexica configuracion = new();
        ServicioDelimitadores servicioDelimitadores = new(configuracion);
        ServicioAnalizadorLexico servicio = CrearServicio(repositorio, configuracion);

        var registro = await servicioDelimitadores.AgregarAsync("#");
        Assert.True(registro.Exito);

        var resultado = await servicio.AnalizarAsync(new SolicitudAnalisisDto
        {
            CodigoFuente = "valor#otro"
        });

        Assert.Collection(
            resultado.Tokens,
            token => Assert.Equal(("valor", "Identificador"), (token.Lexema, token.TipoToken)),
            token => Assert.Equal(("#", "Delimitador"), (token.Lexema, token.TipoToken)),
            token => Assert.Equal(("otro", "Identificador"), (token.Lexema, token.TipoToken)));
    }

    [Fact]
    public async Task AnalizarAsync_WithBooleanConstant_RecognizesItCorrectly()
    {
        ServicioAnalizadorLexico servicio = CrearServicio(new FakeRepositorioAnalisis());

        var resultado = await servicio.AnalizarAsync(new SolicitudAnalisisDto
        {
            CodigoFuente = "bool activo = true;"
        });

        Assert.Collection(
            resultado.Tokens,
            token => Assert.Equal(("bool", "PalabraReservada"), (token.Lexema, token.TipoToken)),
            token => Assert.Equal(("activo", "Identificador"), (token.Lexema, token.TipoToken)),
            token => Assert.Equal(("=", "Operador"), (token.Lexema, token.TipoToken)),
            token => Assert.Equal(("true", "Constante"), (token.Lexema, token.TipoToken)),
            token => Assert.Equal((";", "Delimitador"), (token.Lexema, token.TipoToken)));
    }

    [Fact]
    public async Task AnalizarAsync_WithDigitThenLetters_ReportsInvalidIdentifier()
    {
        ServicioAnalizadorLexico servicio = CrearServicio(new FakeRepositorioAnalisis());

        var resultado = await servicio.AnalizarAsync(new SolicitudAnalisisDto
        {
            CodigoFuente = "int 1variable = 1;"
        });

        Assert.Contains(resultado.Tokens, token => token.Lexema == "int" && token.TipoToken == "PalabraReservada");
        Assert.Contains(resultado.Tokens, token => token.Lexema == "=" && token.TipoToken == "Operador");
        Assert.Contains(resultado.Tokens, token => token.Lexema == "1" && token.TipoToken == "Constante");
        Assert.Contains(resultado.Tokens, token => token.Lexema == ";" && token.TipoToken == "Delimitador");
        Assert.DoesNotContain(resultado.Tokens, token => token.Lexema == "1variable");
        Assert.Contains(resultado.Errores, error =>
            error.CodigoError == "LEX009"
            && error.Lexema == "1variable"
            && error.MensajeError.Contains("Identificador invalido", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task AnalizarAsync_WithSpecialSymbolInsideIdentifier_ReportsLexicalError()
    {
        ServicioAnalizadorLexico servicio = CrearServicio(new FakeRepositorioAnalisis());

        var resultado = await servicio.AnalizarAsync(new SolicitudAnalisisDto
        {
            CodigoFuente = "int mi@var = 1;"
        });

        Assert.DoesNotContain(resultado.Tokens, token => token.Lexema == "mi" && token.TipoToken == "Identificador");
        Assert.DoesNotContain(resultado.Tokens, token => token.Lexema == "var" && token.TipoToken == "Identificador");
        Assert.Contains(resultado.Errores, error =>
            error.CodigoError == "LEX009"
            && error.Lexema == "mi@var"
            && error.MensajeError.Contains("Caracter ilegal", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task AnalizarAsync_WithReservedWordName_ReportsContextError()
    {
        ServicioAnalizadorLexico servicio = CrearServicio(new FakeRepositorioAnalisis());

        var resultado = await servicio.AnalizarAsync(new SolicitudAnalisisDto
        {
            CodigoFuente = "int int = 1;"
        });

        Assert.Equal(2, resultado.Tokens.Count(token => token.Lexema == "int" && token.TipoToken == "PalabraReservada"));
        Assert.DoesNotContain(resultado.Tokens, token => token.Lexema == "int" && token.TipoToken == "Identificador");
        Assert.Contains(resultado.Errores, error =>
            error.CodigoError == "LEX009"
            && error.Lexema == "int"
            && error.MensajeError.Contains("palabra reservada", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task AnalizarAsync_WithDuplicateIdentifierOnSameLine_ReportsContextError()
    {
        ServicioAnalizadorLexico servicio = CrearServicio(new FakeRepositorioAnalisis());

        var resultado = await servicio.AnalizarAsync(new SolicitudAnalisisDto
        {
            CodigoFuente = "valor = valor + 1;"
        });

        Assert.Contains(resultado.Tokens, token => token.Lexema == "valor" && token.TipoToken == "Identificador");
        Assert.Contains(resultado.Errores, error =>
            error.CodigoError == "LEX009"
            && error.Lexema == "valor"
            && error.MensajeError.Contains("Duplicidad", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task AnalizarAsync_WithCStyleTokens_RecognizesCommentsAndStrings()
    {
        ServicioAnalizadorLexico servicio = CrearServicio(new FakeRepositorioAnalisis());

        var resultado = await servicio.AnalizarAsync(new SolicitudAnalisisDto
        {
            CodigoFuente = "if (x >= 10) { /* ok */ return \"hola\"; }"
        });

        Assert.Contains(resultado.Tokens, token => token.Lexema == "if" && token.TipoToken == "PalabraReservada");
        Assert.Contains(resultado.Tokens, token => token.Lexema == ">=" && token.TipoToken == "Operador");
        Assert.Contains(resultado.Tokens, token => token.Lexema == "/* ok */" && token.TipoToken == "Comentario");
        Assert.Contains(resultado.Tokens, token => token.Lexema == "\"hola\"" && token.TipoToken == "Constante");
        Assert.DoesNotContain(resultado.Errores, error => !string.IsNullOrWhiteSpace(error.MensajeError));
    }

    [Fact]
    public async Task AnalizarAsync_WithUnclosedBlockComment_ReportsError()
    {
        ServicioAnalizadorLexico servicio = CrearServicio(new FakeRepositorioAnalisis());

        var resultado = await servicio.AnalizarAsync(new SolicitudAnalisisDto
        {
            CodigoFuente = "int x = 0; /* comentario"
        });

        Assert.Contains(resultado.Errores, error => error.CodigoError == "LEX003");
    }

    [Fact]
    public async Task AnalizarAsync_WithUnbalancedDelimiters_ReportsError()
    {
        ServicioAnalizadorLexico servicio = CrearServicio(new FakeRepositorioAnalisis());

        var resultado = await servicio.AnalizarAsync(new SolicitudAnalisisDto
        {
            CodigoFuente = "if (x > 0 { return x; }"
        });

        Assert.Contains(resultado.Errores, error => error.CodigoError is "LEX007" or "LEX008");
    }

    [Fact]
    public async Task AnalizarAsync_WhenRepositoryFails_ReturnsTokensAndWarning()
    {
        ServicioAnalizadorLexico servicio = CrearServicio(new ThrowingRepositorioAnalisis());

        var resultado = await servicio.AnalizarAsync(new SolicitudAnalisisDto
        {
            CodigoFuente = "int car \""
        });

        Assert.False(resultado.FuePersistido);
        Assert.Equal(0, resultado.IdAnalisis);
        Assert.NotNull(resultado.Advertencia);
        Assert.Contains(resultado.Tokens, token => token.Lexema == "int");
        Assert.Contains(resultado.Tokens, token => token.Lexema == "car");
        Assert.Contains(resultado.Errores, error => error.CodigoError == "LEX002");
    }

    [Fact]
    public async Task AnalizarAsync_CuandoFallaLaConfiguracionLexica_LanzaExcepcion()
    {
        ServicioAnalizadorLexico servicio = new(
            new FakeRepositorioAnalisis(),
            new ServicioPalabrasReservadas(new ThrowingRepositorioConfiguracionLexica()),
            new ServicioDelimitadores(new ThrowingRepositorioConfiguracionLexica()),
            new ServicioBaulErrores(new ThrowingRepositorioConfiguracionLexica()));

        await Assert.ThrowsAsync<InvalidOperationException>(() => servicio.AnalizarAsync(new SolicitudAnalisisDto
        {
            CodigoFuente = "int valor = 1;"
        }));
    }

    [Fact]
    public async Task AnalizarAsync_WithLexicalErrors_SavesErrorState()
    {
        FakeRepositorioAnalisis repositorio = new();
        ServicioAnalizadorLexico servicio = CrearServicio(repositorio);

        var resultado = await servicio.AnalizarAsync(new SolicitudAnalisisDto
        {
            CodigoFuente = "@"
        });

        Assert.Equal("ConErrores", resultado.EstadoAnalisis);
        Assert.Empty(resultado.Tokens);
        Assert.Single(resultado.Errores);
        Assert.Equal("LEX001", resultado.Errores[0].CodigoError);
        Assert.Single(repositorio.AnalisisGuardados);
        Assert.Equal("ConErrores", repositorio.AnalisisGuardados[0].EstadoAnalisis);
    }

    [Fact]
    public async Task AnalizarAsync_WithBlankInput_ThrowsArgumentException()
    {
        ServicioAnalizadorLexico servicio = CrearServicio(new FakeRepositorioAnalisis());

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => servicio.AnalizarAsync(new SolicitudAnalisisDto
        {
            CodigoFuente = " "
        }));

        Assert.Equal("El codigo fuente es obligatorio.", exception.Message);
    }

    private static ServicioAnalizadorLexico CrearServicio(
        IRepositorioAnalisis repositorioAnalisis,
        FakeRepositorioConfiguracionLexica? configuracion = null)
    {
        configuracion ??= new FakeRepositorioConfiguracionLexica();

        return new ServicioAnalizadorLexico(
            repositorioAnalisis,
            new ServicioPalabrasReservadas(configuracion),
            new ServicioDelimitadores(configuracion),
            new ServicioBaulErrores(configuracion));
    }

    private sealed class FakeRepositorioAnalisis : IRepositorioAnalisis
    {
        public List<Analisis> AnalisisGuardados { get; } = [];

        public Task<int> GuardarAnalisisAsync(Analisis analisis)
        {
            Analisis clon = new()
            {
                IdAnalisis = AnalisisGuardados.Count + 1,
                EstadoAnalisis = analisis.EstadoAnalisis,
                Tokens = analisis.Tokens.Select(token => new TokenAnalisis
                {
                    Lexema = token.Lexema,
                    TipoToken = token.TipoToken,
                    NumeroLinea = token.NumeroLinea,
                    NumeroColumna = token.NumeroColumna
                }).ToList(),
                Errores = analisis.Errores.Select(error => new ErrorAnalisis
                {
                    IdErrorCatalogo = error.IdErrorCatalogo,
                    CodigoError = error.CodigoError,
                    MensajeError = error.MensajeError,
                    Lexema = error.Lexema,
                    NumeroLinea = error.NumeroLinea,
                    NumeroColumna = error.NumeroColumna
                }).ToList()
            };

            AnalisisGuardados.Add(clon);
            return Task.FromResult(clon.IdAnalisis);
        }
    }

    private sealed class ThrowingRepositorioAnalisis : IRepositorioAnalisis
    {
        public Task<int> GuardarAnalisisAsync(Analisis analisis)
        {
            throw new InvalidOperationException("db down");
        }
    }
}
