using MiniLenguajeLexico.Domain.Entities;

namespace MiniLenguajeLexico.Domain.Constants;

public static class BaulErroresCatalogo
{
    private static readonly IReadOnlyList<ErrorCatalogo> Predeterminados =
    [
        Crear(1, "LEX001", "Simbolo no reconocido", "Simbolo no reconocido.", "Lexico"),
        Crear(2, "LEX002", "Cadena no cerrada", "Cadena no cerrada correctamente.", "Lexico"),
        Crear(3, "LEX003", "Comentario de bloque no cerrado", "Comentario de bloque no cerrado.", "Lexico"),
        Crear(4, "LEX004", "Literal de caracter invalido", "Literal de caracter invalido.", "Lexico"),
        Crear(5, "LEX005", "Literal de caracter no cerrado", "Literal de caracter no cerrado.", "Lexico"),
        Crear(6, "LEX006", "Delimitador sin apertura", "Delimitador de cierre sin apertura correspondiente.", "Lexico"),
        Crear(7, "LEX007", "Delimitador no coincide", "Delimitador de apertura no coincide con el cierre detectado.", "Lexico"),
        Crear(8, "LEX008", "Delimitador sin cierre", "Delimitador sin cierre correspondiente.", "Lexico"),
        Crear(9, "LEX009", "Identificador invalido en contexto", "El identificador no cumple con las reglas del lenguaje o su contexto de uso.", "Contextual")
    ];

    public static IReadOnlyList<ErrorCatalogo> ObtenerPredeterminados()
    {
        return Predeterminados.Select(Clonar).ToList();
    }

    private static ErrorCatalogo Crear(int id, string codigo, string nombre, string descripcion, string tipo)
    {
        return new ErrorCatalogo
        {
            IdErrorCatalogo = id,
            CodigoError = codigo,
            NombreError = nombre,
            DescripcionError = descripcion,
            TipoError = tipo,
            Activo = true
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
