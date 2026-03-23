using MiniLenguajeLexico.Domain.Entities;
using MiniLenguajeLexico.Domain.Enums;
using System.Text;

namespace MiniLenguajeLexico.Application.Services;

internal sealed class AnalizadorLexicoMotor
{
    private static readonly HashSet<string> ReservadasQueIntroducenIdentificadores = new(StringComparer.OrdinalIgnoreCase)
    {
        "bool", "char", "class", "double", "enum", "float", "int", "long", "short", "signed", "struct", "typedef", "union", "unsigned", "void"
    };

    private static readonly HashSet<string> ConstantesBooleanas = new(StringComparer.OrdinalIgnoreCase)
    {
        "true", "false"
    };

    private const string ErrorSimboloNoReconocido = "LEX001";
    private const string ErrorCadenaNoCerrada = "LEX002";
    private const string ErrorComentarioBloqueNoCerrado = "LEX003";
    private const string ErrorCaracterInvalido = "LEX004";
    private const string ErrorCaracterNoCerrado = "LEX005";
    private const string ErrorCierreSinApertura = "LEX006";
    private const string ErrorDelimitadorNoCoincide = "LEX007";
    private const string ErrorSinCierre = "LEX008";
    private const string ErrorIdentificadorContextual = "LEX009";

    private static readonly string[] OperadoresDobles =
    [
        "==", "!=", "<=", ">=", "++", "--", "+=", "-=", "*=", "/=", "%=", "&&", "||", "->"
    ];

    private static readonly string OperadoresSimples = "+-*/%=<>!&|^~?:.";

    private readonly HashSet<string> _palabrasReservadas;
    private readonly HashSet<char> _delimitadores;
    private readonly Dictionary<string, ErrorCatalogo> _erroresPorCodigo;

    private string _codigo = string.Empty;
    private int _posicion;
    private int _linea;
    private int _columna;

    public AnalizadorLexicoMotor(
        IEnumerable<string> palabrasReservadas,
        IEnumerable<string> delimitadores,
        IEnumerable<ErrorCatalogo> catalogoErrores)
    {
        _palabrasReservadas = new HashSet<string>(palabrasReservadas, StringComparer.OrdinalIgnoreCase);
        _delimitadores = new HashSet<char>(
            delimitadores
                .Where(item => !string.IsNullOrWhiteSpace(item))
                .Select(item => item.Trim()[0]));
        _erroresPorCodigo = catalogoErrores
            .Where(item => item.Activo)
            .GroupBy(item => item.CodigoError, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);
    }

    public ResultadoMotorLexico Analizar(string codigoFuente)
    {
        _codigo = codigoFuente ?? string.Empty;
        _posicion = 0;
        _linea = 1;
        _columna = 1;

        ResultadoMotorLexico resultado = new();
        Stack<DelimitadorAbierto> delimitadoresAbiertos = new();

        while (!FinArchivo())
        {
            char actual = Actual();

            if (char.IsWhiteSpace(actual))
            {
                Avanzar();
                continue;
            }

            int lineaInicio = _linea;
            int columnaInicio = _columna;

            if (char.IsLetter(actual) || actual == '_')
            {
                string lexema = LeerIdentificador();

                if (!FinArchivo() && EsCaracterIlegalDentroDeIdentificador(Actual()))
                {
                    lexema += LeerRestoIdentificadorInvalido();
                    AgregarError(
                        resultado,
                        ErrorIdentificadorContextual,
                        lexema,
                        lineaInicio,
                        columnaInicio,
                        "Caracter ilegal en identificador. Solo se permiten letras, numeros y guion bajo.");
                    continue;
                }

                string tipo = ObtenerTipoIdentificadorOConstante(lexema);

                AgregarToken(resultado, lexema, tipo, lineaInicio, columnaInicio);
                continue;
            }

            if (char.IsDigit(actual))
            {
                (string lexema, bool esIdentificadorInvalido) = LeerNumeroOIdentificador();
                if (esIdentificadorInvalido)
                {
                    AgregarError(
                        resultado,
                        ErrorIdentificadorContextual,
                        lexema,
                        lineaInicio,
                        columnaInicio,
                        "Identificador invalido. Debe iniciar con letra o guion bajo.");
                }
                else
                {
                    AgregarToken(resultado, lexema, TipoToken.Constante.ToString(), lineaInicio, columnaInicio);
                }

                continue;
            }

            if (actual == '"')
            {
                string lexema = LeerCadena(out bool valida);
                if (valida)
                {
                    AgregarToken(resultado, lexema, TipoToken.Constante.ToString(), lineaInicio, columnaInicio);
                }
                else
                {
                    AgregarError(resultado, ErrorCadenaNoCerrada, lexema, lineaInicio, columnaInicio);
                }

                continue;
            }

            if (actual == '\'')
            {
                string lexema = LeerCaracter(out bool valido, out string codigoError);
                if (valido)
                {
                    AgregarToken(resultado, lexema, TipoToken.Constante.ToString(), lineaInicio, columnaInicio);
                }
                else
                {
                    AgregarError(resultado, codigoError, lexema, lineaInicio, columnaInicio);
                }

                continue;
            }

            if (actual == '/' && SiguienteEs('/'))
            {
                string comentario = LeerComentarioLinea();
                AgregarToken(resultado, comentario, TipoToken.Comentario.ToString(), lineaInicio, columnaInicio);
                continue;
            }

            if (actual == '/' && SiguienteEs('*'))
            {
                string comentario = LeerComentarioBloque(out bool comentarioValido);
                if (comentarioValido)
                {
                    AgregarToken(resultado, comentario, TipoToken.Comentario.ToString(), lineaInicio, columnaInicio);
                }
                else
                {
                    AgregarError(resultado, ErrorComentarioBloqueNoCerrado, comentario, lineaInicio, columnaInicio);
                }

                continue;
            }

            if (IntentarLeerOperador(out string operador))
            {
                AgregarToken(resultado, operador, TipoToken.Operador.ToString(), lineaInicio, columnaInicio);
                continue;
            }

            if (_delimitadores.Contains(actual))
            {
                Avanzar();
                string delimitador = actual.ToString();
                AgregarToken(resultado, delimitador, TipoToken.Delimitador.ToString(), lineaInicio, columnaInicio);
                ValidarDelimitador(resultado, delimitadoresAbiertos, actual, lineaInicio, columnaInicio);
                continue;
            }

            AgregarError(resultado, ErrorSimboloNoReconocido, actual.ToString(), lineaInicio, columnaInicio);
            Avanzar();
        }

        while (delimitadoresAbiertos.Count > 0)
        {
            DelimitadorAbierto pendiente = delimitadoresAbiertos.Pop();
            AgregarError(
                resultado,
                ErrorSinCierre,
                pendiente.Simbolo.ToString(),
                pendiente.Linea,
                pendiente.Columna,
                $"Delimitador '{pendiente.Simbolo}' sin cierre correspondiente.");
        }

        ValidarIdentificadoresEnContexto(resultado);

        return resultado;
    }

    private bool FinArchivo() => _posicion >= _codigo.Length;

    private char Actual() => _codigo[_posicion];

    private char Siguiente() => _posicion + 1 < _codigo.Length ? _codigo[_posicion + 1] : '\0';

    private bool SiguienteEs(char esperado) => Siguiente() == esperado;

    private void Avanzar()
    {
        if (FinArchivo()) return;

        if (_codigo[_posicion] == '\n')
        {
            _linea++;
            _columna = 1;
        }
        else
        {
            _columna++;
        }

        _posicion++;
    }

    private string LeerIdentificador()
    {
        StringBuilder sb = new();

        while (!FinArchivo() && (char.IsLetterOrDigit(Actual()) || Actual() == '_'))
        {
            sb.Append(Actual());
            Avanzar();
        }

        return sb.ToString();
    }

    private string LeerRestoIdentificadorInvalido()
    {
        StringBuilder sb = new();

        while (!FinArchivo() && !EsSeparadorDeToken(Actual()))
        {
            sb.Append(Actual());
            Avanzar();
        }

        return sb.ToString();
    }

    private string ObtenerTipoIdentificadorOConstante(string lexema)
    {
        if (ConstantesBooleanas.Contains(lexema))
        {
            return TipoToken.Constante.ToString();
        }

        return _palabrasReservadas.Contains(lexema)
            ? TipoToken.PalabraReservada.ToString()
            : TipoToken.Identificador.ToString();
    }

    private (string Lexema, bool EsIdentificador) LeerNumeroOIdentificador()
    {
        StringBuilder sb = new();
        bool esIdentificador = false;
        bool tienePuntoDecimal = false;

        while (!FinArchivo())
        {
            char actual = Actual();

            if (char.IsDigit(actual))
            {
                sb.Append(actual);
                Avanzar();
                continue;
            }

            if (char.IsLetter(actual) || actual == '_')
            {
                esIdentificador = true;
                sb.Append(actual);
                Avanzar();
                continue;
            }

            if (actual == '.' && !tienePuntoDecimal && !esIdentificador && char.IsDigit(Siguiente()))
            {
                tienePuntoDecimal = true;
                sb.Append(actual);
                Avanzar();
                continue;
            }

            break;
        }

        return (sb.ToString(), esIdentificador);
    }

    private string LeerCadena(out bool valida)
    {
        StringBuilder sb = new();
        valida = false;

        sb.Append(Actual());
        Avanzar();

        while (!FinArchivo())
        {
            char actual = Actual();
            sb.Append(actual);

            if (actual == '\\' && !FinArchivo())
            {
                Avanzar();
                if (!FinArchivo())
                {
                    sb.Append(Actual());
                }
                Avanzar();
                continue;
            }

            if (actual == '"')
            {
                Avanzar();
                valida = true;
                return sb.ToString();
            }

            if (actual == '\n')
            {
                return sb.ToString();
            }

            Avanzar();
        }

        return sb.ToString();
    }

    private string LeerCaracter(out bool valido, out string codigoError)
    {
        StringBuilder sb = new();
        valido = false;
        codigoError = ErrorCaracterNoCerrado;

        sb.Append(Actual());
        Avanzar();

        if (FinArchivo() || Actual() == '\n')
        {
            return sb.ToString();
        }

        if (Actual() == '\\')
        {
            sb.Append(Actual());
            Avanzar();

            if (FinArchivo() || Actual() == '\n')
            {
                return sb.ToString();
            }
        }

        sb.Append(Actual());
        Avanzar();

        if (FinArchivo() || Actual() != '\'')
        {
            codigoError = ErrorCaracterInvalido;
            while (!FinArchivo() && Actual() != '\n' && Actual() != '\'')
            {
                sb.Append(Actual());
                Avanzar();
            }

            if (!FinArchivo() && Actual() == '\'')
            {
                sb.Append(Actual());
                Avanzar();
            }

            return sb.ToString();
        }

        sb.Append(Actual());
        Avanzar();
        valido = true;
        return sb.ToString();
    }

    private string LeerComentarioLinea()
    {
        StringBuilder sb = new();

        while (!FinArchivo() && Actual() != '\n')
        {
            sb.Append(Actual());
            Avanzar();
        }

        return sb.ToString();
    }

    private string LeerComentarioBloque(out bool comentarioValido)
    {
        StringBuilder sb = new();
        comentarioValido = false;

        while (!FinArchivo())
        {
            char actual = Actual();
            sb.Append(actual);
            Avanzar();

            if (actual == '*' && !FinArchivo() && Actual() == '/')
            {
                sb.Append(Actual());
                Avanzar();
                comentarioValido = true;
                return sb.ToString();
            }
        }

        return sb.ToString();
    }

    private bool IntentarLeerOperador(out string operador)
    {
        foreach (string operadorDoble in OperadoresDobles)
        {
            if (Coincide(operadorDoble))
            {
                operador = operadorDoble;
                for (int i = 0; i < operadorDoble.Length; i++)
                {
                    Avanzar();
                }

                return true;
            }
        }

        if (OperadoresSimples.Contains(Actual()))
        {
            operador = Actual().ToString();
            Avanzar();
            return true;
        }

        operador = string.Empty;
        return false;
    }

    private bool EsCaracterIlegalDentroDeIdentificador(char caracter)
    {
        return !EsSeparadorDeToken(caracter);
    }

    private bool EsSeparadorDeToken(char caracter)
    {
        return char.IsWhiteSpace(caracter)
            || _delimitadores.Contains(caracter)
            || OperadoresSimples.Contains(caracter)
            || caracter is '"' or '\'';
    }

    private bool Coincide(string valor)
    {
        if (_posicion + valor.Length > _codigo.Length)
        {
            return false;
        }

        for (int i = 0; i < valor.Length; i++)
        {
            if (_codigo[_posicion + i] != valor[i])
            {
                return false;
            }
        }

        return true;
    }

    private void ValidarDelimitador(
        ResultadoMotorLexico resultado,
        Stack<DelimitadorAbierto> delimitadoresAbiertos,
        char delimitador,
        int linea,
        int columna)
    {
        if (EsDelimitadorApertura(delimitador))
        {
            delimitadoresAbiertos.Push(new DelimitadorAbierto(delimitador, linea, columna));
            return;
        }

        if (!EsDelimitadorCierre(delimitador))
        {
            return;
        }

        if (delimitadoresAbiertos.Count == 0)
        {
            AgregarError(
                resultado,
                ErrorCierreSinApertura,
                delimitador.ToString(),
                linea,
                columna,
                $"Delimitador de cierre '{delimitador}' sin apertura correspondiente.");
            return;
        }

        DelimitadorAbierto abierto = delimitadoresAbiertos.Pop();
        if (!Coinciden(abierto.Simbolo, delimitador))
        {
            AgregarError(
                resultado,
                ErrorDelimitadorNoCoincide,
                delimitador.ToString(),
                linea,
                columna,
                $"Delimitador '{abierto.Simbolo}' no coincide con cierre '{delimitador}'.");
        }
    }

    private static bool EsDelimitadorApertura(char delimitador)
    {
        return delimitador is '(' or '{' or '[';
    }

    private static bool EsDelimitadorCierre(char delimitador)
    {
        return delimitador is ')' or '}' or ']';
    }

    private static bool Coinciden(char apertura, char cierre)
    {
        return (apertura == '(' && cierre == ')')
            || (apertura == '{' && cierre == '}')
            || (apertura == '[' && cierre == ']');
    }

    private static void AgregarToken(
        ResultadoMotorLexico resultado,
        string lexema,
        string tipo,
        int linea,
        int columna)
    {
        resultado.Tokens.Add(new TokenMotor
        {
            Lexema = lexema,
            TipoToken = tipo,
            NumeroLinea = linea,
            NumeroColumna = columna
        });
    }

    private void ValidarIdentificadoresEnContexto(ResultadoMotorLexico resultado)
    {
        ValidarDuplicidadPorLinea(resultado);
        ValidarUsoDeReservadasComoIdentificador(resultado);
    }

    private void ValidarDuplicidadPorLinea(ResultadoMotorLexico resultado)
    {
        foreach (var grupoLinea in resultado.Tokens
                     .Where(token => token.TipoToken == TipoToken.Identificador.ToString())
                     .GroupBy(token => token.NumeroLinea)
                     .OrderBy(group => group.Key))
        {
            HashSet<string> vistos = new(StringComparer.Ordinal);

            foreach (var token in grupoLinea.OrderBy(token => token.NumeroColumna))
            {
                if (!vistos.Add(token.Lexema))
                {
                    AgregarError(
                        resultado,
                        ErrorIdentificadorContextual,
                        token.Lexema,
                        token.NumeroLinea,
                        token.NumeroColumna,
                        $"Duplicidad en la misma linea. El identificador '{token.Lexema}' ya fue usado.");
                }
            }
        }
    }

    private void ValidarUsoDeReservadasComoIdentificador(ResultadoMotorLexico resultado)
    {
        var tokens = resultado.Tokens
            .Where(token => token.TipoToken != TipoToken.Comentario.ToString())
            .OrderBy(token => token.NumeroLinea)
            .ThenBy(token => token.NumeroColumna)
            .ToList();

        for (int indice = 0; indice < tokens.Count; indice++)
        {
            var token = tokens[indice];
            if (token.TipoToken != TipoToken.PalabraReservada.ToString())
            {
                continue;
            }

            if (!EsUsoDeReservadaComoIdentificador(tokens, indice))
            {
                continue;
            }

            AgregarError(
                resultado,
                ErrorIdentificadorContextual,
                token.Lexema,
                token.NumeroLinea,
                token.NumeroColumna,
                $"Uso de palabra reservada. '{token.Lexema}' no puede utilizarse como identificador.");
        }
    }

    private static bool EsUsoDeReservadaComoIdentificador(IReadOnlyList<TokenMotor> tokens, int indice)
    {
        if (indice == 0)
        {
            return false;
        }

        TokenMotor actual = tokens[indice];
        TokenMotor anterior = tokens[indice - 1];

        if (anterior.NumeroLinea != actual.NumeroLinea)
        {
            return false;
        }

        if (anterior.TipoToken == TipoToken.PalabraReservada.ToString())
        {
            return ReservadasQueIntroducenIdentificadores.Contains(anterior.Lexema);
        }

        if (anterior.TipoToken != TipoToken.Delimitador.ToString() || anterior.Lexema != ",")
        {
            return false;
        }

        TokenMotor? previoAlAnterior = indice >= 2 ? tokens[indice - 2] : null;
        return previoAlAnterior is not null
            && previoAlAnterior.NumeroLinea == actual.NumeroLinea
            && (previoAlAnterior.TipoToken == TipoToken.Identificador.ToString()
                || (previoAlAnterior.TipoToken == TipoToken.PalabraReservada.ToString()
                    && ReservadasQueIntroducenIdentificadores.Contains(previoAlAnterior.Lexema)));
    }

    private void AgregarError(
        ResultadoMotorLexico resultado,
        string codigoError,
        string? lexema,
        int linea,
        int columna,
        string? mensajePredeterminado = null)
    {
        ErrorCatalogo? catalogo = _erroresPorCodigo.TryGetValue(codigoError, out var encontrado)
            ? encontrado
            : null;

        resultado.Errores.Add(new ErrorMotor
        {
            IdErrorCatalogo = catalogo?.IdErrorCatalogo ?? 0,
            CodigoError = codigoError,
            MensajeError = mensajePredeterminado ?? catalogo?.DescripcionError ?? codigoError,
            Lexema = lexema,
            NumeroLinea = linea,
            NumeroColumna = columna
        });
    }

    private readonly record struct DelimitadorAbierto(char Simbolo, int Linea, int Columna);
}
