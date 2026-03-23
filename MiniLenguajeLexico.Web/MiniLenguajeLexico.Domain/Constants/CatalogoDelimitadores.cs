namespace MiniLenguajeLexico.Domain.Constants;

public static class CatalogoDelimitadores
{
    private static readonly IReadOnlyList<string> Predeterminados =
    [
        "(", ")", "{", "}", "[", "]", ";", ","
    ];

    public static IReadOnlyList<string> ObtenerPredeterminados()
    {
        return Predeterminados.ToList();
    }
}
