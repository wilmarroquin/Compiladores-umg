using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MiniLenguajeLexico.Application.Interfaces;
using MiniLenguajeLexico.Domain.Entities;
using System.Data;

namespace MiniLenguajeLexico.Infrastructure.Repositories;

public class RepositorioAnalisisSql : IRepositorioAnalisis
{
    private readonly string _cadenaConexion;

    public RepositorioAnalisisSql(IConfiguration configuration)
    {
        _cadenaConexion = configuration.GetConnectionString("CadenaSql")
            ?? throw new InvalidOperationException("No existe la cadena de conexión.");

        if (string.IsNullOrWhiteSpace(_cadenaConexion))
        {
            throw new InvalidOperationException("La cadena de conexión 'CadenaSql' es obligatoria.");
        }
    }

    public async Task<int> GuardarAnalisisAsync(Analisis analisis)
    {
        using SqlConnection conexion = new(_cadenaConexion);
        await conexion.OpenAsync();

        using SqlTransaction transaccion = (SqlTransaction)await conexion.BeginTransactionAsync();

        try
        {
            int idAnalisis;

            const string sqlAnalisis = """
                INSERT INTO Analisis (EstadoAnalisis)
                OUTPUT INSERTED.IdAnalisis
                VALUES (@EstadoAnalisis);
                """;

            using (SqlCommand comando = new(sqlAnalisis, conexion, transaccion))
            {
                comando.Parameters.Add("@EstadoAnalisis", SqlDbType.NVarChar, 50).Value = analisis.EstadoAnalisis;

                object? resultado = await comando.ExecuteScalarAsync();
                idAnalisis = resultado as int?
                    ?? throw new InvalidOperationException("No se pudo recuperar el identificador del analisis.");
            }

            foreach (var token in analisis.Tokens)
            {
                const string sqlToken = """
                    INSERT INTO TokensAnalisis
                    (IdAnalisis, Lexema, TipoToken, NumeroLinea, NumeroColumna)
                    VALUES
                    (@IdAnalisis, @Lexema, @TipoToken, @NumeroLinea, @NumeroColumna);
                    """;

                using SqlCommand comandoToken = new(sqlToken, conexion, transaccion);
                comandoToken.Parameters.Add("@IdAnalisis", SqlDbType.Int).Value = idAnalisis;
                comandoToken.Parameters.Add("@Lexema", SqlDbType.NVarChar, 400).Value = token.Lexema;
                comandoToken.Parameters.Add("@TipoToken", SqlDbType.NVarChar, 100).Value = token.TipoToken;
                comandoToken.Parameters.Add("@NumeroLinea", SqlDbType.Int).Value = token.NumeroLinea;
                comandoToken.Parameters.Add("@NumeroColumna", SqlDbType.Int).Value = token.NumeroColumna;

                await comandoToken.ExecuteNonQueryAsync();
            }

            foreach (var error in analisis.Errores)
            {
                const string sqlError = """
                    INSERT INTO ErroresAnalisis
                    (IdAnalisis, IdErrorCatalogo, CodigoError, MensajeError, Lexema, NumeroLinea, NumeroColumna)
                    VALUES
                    (@IdAnalisis, @IdErrorCatalogo, @CodigoError, @MensajeError, @Lexema, @NumeroLinea, @NumeroColumna);
                    """;

                using SqlCommand comandoError = new(sqlError, conexion, transaccion);
                comandoError.Parameters.Add("@IdAnalisis", SqlDbType.Int).Value = idAnalisis;
                comandoError.Parameters.Add("@IdErrorCatalogo", SqlDbType.Int).Value = error.IdErrorCatalogo == 0 ? DBNull.Value : error.IdErrorCatalogo;
                comandoError.Parameters.Add("@CodigoError", SqlDbType.NVarChar, 50).Value = error.CodigoError;
                comandoError.Parameters.Add("@MensajeError", SqlDbType.NVarChar, 500).Value = error.MensajeError;
                comandoError.Parameters.Add("@Lexema", SqlDbType.NVarChar, 400).Value = (object?)error.Lexema ?? DBNull.Value;
                comandoError.Parameters.Add("@NumeroLinea", SqlDbType.Int).Value = error.NumeroLinea;
                comandoError.Parameters.Add("@NumeroColumna", SqlDbType.Int).Value = error.NumeroColumna;

                await comandoError.ExecuteNonQueryAsync();
            }

            await transaccion.CommitAsync();
            return idAnalisis;
        }
        catch
        {
            await transaccion.RollbackAsync();
            throw;
        }
    }
}
