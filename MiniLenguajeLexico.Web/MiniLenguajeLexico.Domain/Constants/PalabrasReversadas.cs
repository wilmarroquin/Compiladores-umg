namespace MiniLenguajeLexico.Domain.Constants;

public static class PalabrasReservadas
{
    private static readonly IReadOnlyList<string> Predeterminadas =
    [
        "auto", "bool", "break", "case", "char", "const", "continue", "default",
        "do", "double", "else", "enum", "extern", "float", "for", "goto", "if",
        "int", "long", "print", "register", "return", "short", "signed", "sizeof",
        "static", "string", "struct", "switch", "typedef", "union", "unsigned",
        "void", "volatile", "while"
    ];

    public static IReadOnlyList<string> ObtenerPredeterminadas()
    {
        return Predeterminadas.ToList();
    }
}
