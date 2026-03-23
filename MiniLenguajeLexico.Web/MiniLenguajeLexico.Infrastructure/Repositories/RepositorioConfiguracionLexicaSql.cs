using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MiniLenguajeLexico.Application.Interfaces;
using MiniLenguajeLexico.Domain.Constants;
using MiniLenguajeLexico.Domain.Entities;
using System.Data;

namespace MiniLenguajeLexico.Infrastructure.Repositories;

public class RepositorioConfiguracionLexicaSql : IRepositorioConfiguracionLexica
{
    private readonly string _cadenaConexion;

    public RepositorioConfiguracionLexicaSql(IConfiguration configuration)
    {
        _cadenaConexion = configuration.GetConnectionString("CadenaSql")
            ?? throw new InvalidOperationException("No existe la cadena de conexion.");

        if (string.IsNullOrWhiteSpace(_cadenaConexion))
        {
            throw new InvalidOperationException("La cadena de conexion 'CadenaSql' es obligatoria.");
        }
    }

    public async Task InicializarCatalogosAsync()
    {
        using SqlConnection conexion = new(_cadenaConexion);
        await conexion.OpenAsync();

        using SqlTransaction transaccion = (SqlTransaction)await conexion.BeginTransactionAsync();

        try
        {
            using SqlCommand comando = new(ConstruirScriptInicializacion(), conexion, transaccion);
            await comando.ExecuteNonQueryAsync();

            await transaccion.CommitAsync();
        }
        catch
        {
            await transaccion.RollbackAsync();
            throw;
        }
    }

    public async Task<IReadOnlyList<PalabraReservadaCatalogo>> ObtenerPalabrasReservadasAsync(bool soloActivas = true)
    {
        List<PalabraReservadaCatalogo> lista = [];

        using SqlConnection conexion = new(_cadenaConexion);
        await conexion.OpenAsync();

        string sql = $"""
            SELECT IdPalabraReservada, Palabra, Activo
            FROM PalabrasReservadasCatalogo
            {(soloActivas ? "WHERE Activo = 1" : string.Empty)}
            ORDER BY Palabra;
            """;

        using SqlCommand comando = new(sql, conexion);
        using SqlDataReader lector = await comando.ExecuteReaderAsync();

        while (await lector.ReadAsync())
        {
            lista.Add(new PalabraReservadaCatalogo
            {
                IdPalabraReservada = lector.GetInt32(lector.GetOrdinal("IdPalabraReservada")),
                Palabra = lector.GetString(lector.GetOrdinal("Palabra")),
                Activo = lector.GetBoolean(lector.GetOrdinal("Activo"))
            });
        }

        return lista;
    }

    public async Task<PalabraReservadaCatalogo?> ObtenerPalabraReservadaPorIdAsync(int idPalabraReservada)
    {
        using SqlConnection conexion = new(_cadenaConexion);
        await conexion.OpenAsync();

        const string sql = """
            SELECT IdPalabraReservada, Palabra, Activo
            FROM PalabrasReservadasCatalogo
            WHERE IdPalabraReservada = @IdPalabraReservada;
            """;

        using SqlCommand comando = new(sql, conexion);
        comando.Parameters.Add("@IdPalabraReservada", SqlDbType.Int).Value = idPalabraReservada;
        using SqlDataReader lector = await comando.ExecuteReaderAsync();

        if (!await lector.ReadAsync())
        {
            return null;
        }

        return new PalabraReservadaCatalogo
        {
            IdPalabraReservada = lector.GetInt32(lector.GetOrdinal("IdPalabraReservada")),
            Palabra = lector.GetString(lector.GetOrdinal("Palabra")),
            Activo = lector.GetBoolean(lector.GetOrdinal("Activo"))
        };
    }

    public async Task AgregarPalabraReservadaAsync(string palabra)
    {
        using SqlConnection conexion = new(_cadenaConexion);
        await conexion.OpenAsync();

        const string sql = """
            INSERT INTO PalabrasReservadasCatalogo (Palabra, Activo)
            VALUES (@Palabra, @Activo);
            """;

        using SqlCommand comando = new(sql, conexion);
        comando.Parameters.Add("@Palabra", SqlDbType.NVarChar, 100).Value = palabra;
        comando.Parameters.Add("@Activo", SqlDbType.Bit).Value = true;
        await comando.ExecuteNonQueryAsync();
    }

    public async Task ActualizarPalabraReservadaAsync(PalabraReservadaCatalogo palabraReservada)
    {
        using SqlConnection conexion = new(_cadenaConexion);
        await conexion.OpenAsync();

        const string sql = """
            UPDATE PalabrasReservadasCatalogo
            SET Palabra = @Palabra,
                Activo = @Activo
            WHERE IdPalabraReservada = @IdPalabraReservada;
            """;

        using SqlCommand comando = new(sql, conexion);
        comando.Parameters.Add("@IdPalabraReservada", SqlDbType.Int).Value = palabraReservada.IdPalabraReservada;
        comando.Parameters.Add("@Palabra", SqlDbType.NVarChar, 100).Value = palabraReservada.Palabra;
        comando.Parameters.Add("@Activo", SqlDbType.Bit).Value = palabraReservada.Activo;
        await comando.ExecuteNonQueryAsync();
    }

    public async Task EliminarPalabraReservadaAsync(int idPalabraReservada)
    {
        using SqlConnection conexion = new(_cadenaConexion);
        await conexion.OpenAsync();

        const string sql = """
            DELETE FROM PalabrasReservadasCatalogo
            WHERE IdPalabraReservada = @IdPalabraReservada;
            """;

        using SqlCommand comando = new(sql, conexion);
        comando.Parameters.Add("@IdPalabraReservada", SqlDbType.Int).Value = idPalabraReservada;
        await comando.ExecuteNonQueryAsync();
    }

    public async Task<IReadOnlyList<DelimitadorCatalogo>> ObtenerDelimitadoresAsync(bool soloActivos = true)
    {
        List<DelimitadorCatalogo> lista = [];

        using SqlConnection conexion = new(_cadenaConexion);
        await conexion.OpenAsync();

        string sql = $"""
            SELECT IdDelimitador, Simbolo, Activo
            FROM DelimitadoresCatalogo
            {(soloActivos ? "WHERE Activo = 1" : string.Empty)}
            ORDER BY Simbolo;
            """;

        using SqlCommand comando = new(sql, conexion);
        using SqlDataReader lector = await comando.ExecuteReaderAsync();

        while (await lector.ReadAsync())
        {
            lista.Add(new DelimitadorCatalogo
            {
                IdDelimitador = lector.GetInt32(lector.GetOrdinal("IdDelimitador")),
                Simbolo = lector.GetString(lector.GetOrdinal("Simbolo")),
                Activo = lector.GetBoolean(lector.GetOrdinal("Activo"))
            });
        }

        return lista;
    }

    public async Task<DelimitadorCatalogo?> ObtenerDelimitadorPorIdAsync(int idDelimitador)
    {
        using SqlConnection conexion = new(_cadenaConexion);
        await conexion.OpenAsync();

        const string sql = """
            SELECT IdDelimitador, Simbolo, Activo
            FROM DelimitadoresCatalogo
            WHERE IdDelimitador = @IdDelimitador;
            """;

        using SqlCommand comando = new(sql, conexion);
        comando.Parameters.Add("@IdDelimitador", SqlDbType.Int).Value = idDelimitador;
        using SqlDataReader lector = await comando.ExecuteReaderAsync();

        if (!await lector.ReadAsync())
        {
            return null;
        }

        return new DelimitadorCatalogo
        {
            IdDelimitador = lector.GetInt32(lector.GetOrdinal("IdDelimitador")),
            Simbolo = lector.GetString(lector.GetOrdinal("Simbolo")),
            Activo = lector.GetBoolean(lector.GetOrdinal("Activo"))
        };
    }

    public async Task AgregarDelimitadorAsync(string simbolo)
    {
        using SqlConnection conexion = new(_cadenaConexion);
        await conexion.OpenAsync();

        const string sql = """
            INSERT INTO DelimitadoresCatalogo (Simbolo, Activo)
            VALUES (@Simbolo, @Activo);
            """;

        using SqlCommand comando = new(sql, conexion);
        comando.Parameters.Add("@Simbolo", SqlDbType.NVarChar, 5).Value = simbolo;
        comando.Parameters.Add("@Activo", SqlDbType.Bit).Value = true;
        await comando.ExecuteNonQueryAsync();
    }

    public async Task ActualizarDelimitadorAsync(DelimitadorCatalogo delimitador)
    {
        using SqlConnection conexion = new(_cadenaConexion);
        await conexion.OpenAsync();

        const string sql = """
            UPDATE DelimitadoresCatalogo
            SET Simbolo = @Simbolo,
                Activo = @Activo
            WHERE IdDelimitador = @IdDelimitador;
            """;

        using SqlCommand comando = new(sql, conexion);
        comando.Parameters.Add("@IdDelimitador", SqlDbType.Int).Value = delimitador.IdDelimitador;
        comando.Parameters.Add("@Simbolo", SqlDbType.NVarChar, 5).Value = delimitador.Simbolo;
        comando.Parameters.Add("@Activo", SqlDbType.Bit).Value = delimitador.Activo;
        await comando.ExecuteNonQueryAsync();
    }

    public async Task EliminarDelimitadorAsync(int idDelimitador)
    {
        using SqlConnection conexion = new(_cadenaConexion);
        await conexion.OpenAsync();

        const string sql = """
            DELETE FROM DelimitadoresCatalogo
            WHERE IdDelimitador = @IdDelimitador;
            """;

        using SqlCommand comando = new(sql, conexion);
        comando.Parameters.Add("@IdDelimitador", SqlDbType.Int).Value = idDelimitador;
        await comando.ExecuteNonQueryAsync();
    }

    public async Task<IReadOnlyList<ErrorCatalogo>> ObtenerErroresCatalogoAsync(bool soloActivos = false)
    {
        List<ErrorCatalogo> lista = [];

        using SqlConnection conexion = new(_cadenaConexion);
        await conexion.OpenAsync();

        string sql = $"""
            SELECT IdErrorCatalogo, CodigoError, NombreError, DescripcionError, TipoError, Activo
            FROM ErroresCatalogo
            {(soloActivos ? "WHERE Activo = 1" : string.Empty)}
            ORDER BY CodigoError;
            """;

        using SqlCommand comando = new(sql, conexion);
        using SqlDataReader lector = await comando.ExecuteReaderAsync();

        while (await lector.ReadAsync())
        {
            lista.Add(new ErrorCatalogo
            {
                IdErrorCatalogo = lector.GetInt32(lector.GetOrdinal("IdErrorCatalogo")),
                CodigoError = lector.GetString(lector.GetOrdinal("CodigoError")),
                NombreError = lector.GetString(lector.GetOrdinal("NombreError")),
                DescripcionError = lector.GetString(lector.GetOrdinal("DescripcionError")),
                TipoError = lector.GetString(lector.GetOrdinal("TipoError")),
                Activo = lector.GetBoolean(lector.GetOrdinal("Activo"))
            });
        }

        return lista;
    }

    public async Task<ErrorCatalogo?> ObtenerErrorCatalogoPorIdAsync(int idErrorCatalogo)
    {
        using SqlConnection conexion = new(_cadenaConexion);
        await conexion.OpenAsync();

        const string sql = """
            SELECT IdErrorCatalogo, CodigoError, NombreError, DescripcionError, TipoError, Activo
            FROM ErroresCatalogo
            WHERE IdErrorCatalogo = @IdErrorCatalogo;
            """;

        using SqlCommand comando = new(sql, conexion);
        comando.Parameters.Add("@IdErrorCatalogo", SqlDbType.Int).Value = idErrorCatalogo;
        using SqlDataReader lector = await comando.ExecuteReaderAsync();

        if (!await lector.ReadAsync())
        {
            return null;
        }

        return new ErrorCatalogo
        {
            IdErrorCatalogo = lector.GetInt32(lector.GetOrdinal("IdErrorCatalogo")),
            CodigoError = lector.GetString(lector.GetOrdinal("CodigoError")),
            NombreError = lector.GetString(lector.GetOrdinal("NombreError")),
            DescripcionError = lector.GetString(lector.GetOrdinal("DescripcionError")),
            TipoError = lector.GetString(lector.GetOrdinal("TipoError")),
            Activo = lector.GetBoolean(lector.GetOrdinal("Activo"))
        };
    }

    public async Task AgregarErrorCatalogoAsync(ErrorCatalogo errorCatalogo)
    {
        using SqlConnection conexion = new(_cadenaConexion);
        await conexion.OpenAsync();

        const string sql = """
            INSERT INTO ErroresCatalogo
            (CodigoError, NombreError, DescripcionError, TipoError, Activo)
            VALUES
            (@CodigoError, @NombreError, @DescripcionError, @TipoError, @Activo);
            """;

        using SqlCommand comando = new(sql, conexion);
        ConfigurarParametrosErrorCatalogo(comando, errorCatalogo, incluirId: false);
        await comando.ExecuteNonQueryAsync();
    }

    public async Task ActualizarErrorCatalogoAsync(ErrorCatalogo errorCatalogo)
    {
        using SqlConnection conexion = new(_cadenaConexion);
        await conexion.OpenAsync();

        const string sql = """
            UPDATE ErroresCatalogo
            SET CodigoError = @CodigoError,
                NombreError = @NombreError,
                DescripcionError = @DescripcionError,
                TipoError = @TipoError,
                Activo = @Activo
            WHERE IdErrorCatalogo = @IdErrorCatalogo;
            """;

        using SqlCommand comando = new(sql, conexion);
        ConfigurarParametrosErrorCatalogo(comando, errorCatalogo, incluirId: true);
        await comando.ExecuteNonQueryAsync();
    }

    public async Task EliminarErrorCatalogoAsync(int idErrorCatalogo)
    {
        using SqlConnection conexion = new(_cadenaConexion);
        await conexion.OpenAsync();

        const string sql = """
            DELETE FROM ErroresCatalogo
            WHERE IdErrorCatalogo = @IdErrorCatalogo;
            """;

        using SqlCommand comando = new(sql, conexion);
        comando.Parameters.Add("@IdErrorCatalogo", SqlDbType.Int).Value = idErrorCatalogo;
        await comando.ExecuteNonQueryAsync();
    }

    private static void ConfigurarParametrosErrorCatalogo(SqlCommand comando, ErrorCatalogo errorCatalogo, bool incluirId)
    {
        if (incluirId)
        {
            comando.Parameters.Add("@IdErrorCatalogo", SqlDbType.Int).Value = errorCatalogo.IdErrorCatalogo;
        }

        comando.Parameters.Add("@CodigoError", SqlDbType.NVarChar, 50).Value = errorCatalogo.CodigoError;
        comando.Parameters.Add("@NombreError", SqlDbType.NVarChar, 150).Value = errorCatalogo.NombreError;
        comando.Parameters.Add("@DescripcionError", SqlDbType.NVarChar, 500).Value = errorCatalogo.DescripcionError;
        comando.Parameters.Add("@TipoError", SqlDbType.NVarChar, 50).Value = errorCatalogo.TipoError;
        comando.Parameters.Add("@Activo", SqlDbType.Bit).Value = errorCatalogo.Activo;
    }

    private static string ConstruirScriptInicializacion()
    {
        string insercionesPalabras = string.Join(
            ",\n                ",
            PalabrasReservadas.ObtenerPredeterminadas()
                .Select(palabra => $"(N'{EscaparSqlLiteral(palabra)}', 1)"));

        string insercionesDelimitadores = string.Join(
            ",\n                ",
            CatalogoDelimitadores.ObtenerPredeterminados()
                .Select(simbolo => $"(N'{EscaparSqlLiteral(simbolo)}', 1)"));

        string insercionesErrores = string.Join(
            ",\n                ",
            BaulErroresCatalogo.ObtenerPredeterminados()
                .Select(error => $"({error.IdErrorCatalogo}, N'{EscaparSqlLiteral(error.CodigoError)}', N'{EscaparSqlLiteral(error.NombreError)}', N'{EscaparSqlLiteral(error.DescripcionError)}', N'{EscaparSqlLiteral(error.TipoError)}', {(error.Activo ? 1 : 0)})"));

        string insercionesErroresFaltantes = string.Join(
            "\n\n            ",
            BaulErroresCatalogo.ObtenerPredeterminados()
                .Select(error => $$"""
                    IF NOT EXISTS (SELECT 1 FROM dbo.ErroresCatalogo WHERE CodigoError = N'{{EscaparSqlLiteral(error.CodigoError)}}')
                    BEGIN
                        INSERT INTO dbo.ErroresCatalogo (CodigoError, NombreError, DescripcionError, TipoError, Activo)
                        VALUES (N'{{EscaparSqlLiteral(error.CodigoError)}}', N'{{EscaparSqlLiteral(error.NombreError)}}', N'{{EscaparSqlLiteral(error.DescripcionError)}}', N'{{EscaparSqlLiteral(error.TipoError)}}', {{(error.Activo ? 1 : 0)}});
                    END;
                    """));

        return $$"""
            IF OBJECT_ID(N'dbo.Analisis', N'U') IS NULL
            BEGIN
                CREATE TABLE dbo.Analisis
                (
                    IdAnalisis INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                    EstadoAnalisis NVARCHAR(50) NOT NULL
                );
            END;

            DECLARE @constraintName sysname;

            IF COL_LENGTH(N'dbo.Analisis', N'FechaRegistro') IS NOT NULL
            BEGIN
                SELECT @constraintName = dc.name
                FROM sys.default_constraints dc
                INNER JOIN sys.columns c
                    ON c.object_id = dc.parent_object_id
                   AND c.column_id = dc.parent_column_id
                WHERE dc.parent_object_id = OBJECT_ID(N'dbo.Analisis')
                  AND c.name = N'FechaRegistro';

                IF @constraintName IS NOT NULL
                BEGIN
                    EXEC(N'ALTER TABLE dbo.Analisis DROP CONSTRAINT [' + @constraintName + N']');
                END;

                ALTER TABLE dbo.Analisis DROP COLUMN FechaRegistro;
            END;

            IF COL_LENGTH(N'dbo.Analisis', N'CodigoFuente') IS NOT NULL
            BEGIN
                ALTER TABLE dbo.Analisis DROP COLUMN CodigoFuente;
            END;

            IF OBJECT_ID(N'dbo.TokensAnalisis', N'U') IS NULL
            BEGIN
                CREATE TABLE dbo.TokensAnalisis
                (
                    IdToken INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                    IdAnalisis INT NOT NULL,
                    Lexema NVARCHAR(400) NOT NULL,
                    TipoToken NVARCHAR(100) NOT NULL,
                    NumeroLinea INT NOT NULL,
                    NumeroColumna INT NOT NULL,
                    CONSTRAINT FK_TokensAnalisis_Analisis
                        FOREIGN KEY (IdAnalisis) REFERENCES dbo.Analisis(IdAnalisis)
                        ON DELETE CASCADE
                );
            END;

            IF NOT EXISTS (
                    SELECT 1
                    FROM sys.indexes
                    WHERE name = N'IX_TokensAnalisis_IdAnalisis'
                      AND object_id = OBJECT_ID(N'dbo.TokensAnalisis'))
            BEGIN
                CREATE INDEX IX_TokensAnalisis_IdAnalisis ON dbo.TokensAnalisis(IdAnalisis);
            END;

            IF OBJECT_ID(N'dbo.ErroresCatalogo', N'U') IS NULL
            BEGIN
                CREATE TABLE dbo.ErroresCatalogo
                (
                    IdErrorCatalogo INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                    CodigoError NVARCHAR(50) NOT NULL,
                    NombreError NVARCHAR(150) NOT NULL,
                    DescripcionError NVARCHAR(500) NOT NULL,
                    TipoError NVARCHAR(50) NOT NULL,
                    Activo BIT NOT NULL,
                    CONSTRAINT UQ_ErroresCatalogo_CodigoError UNIQUE (CodigoError)
                );
            END;

            IF OBJECT_ID(N'dbo.PalabrasReservadasCatalogo', N'U') IS NULL
            BEGIN
                CREATE TABLE dbo.PalabrasReservadasCatalogo
                (
                    IdPalabraReservada INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                    Palabra NVARCHAR(100) NOT NULL,
                    Activo BIT NOT NULL,
                    CONSTRAINT UQ_PalabrasReservadasCatalogo_Palabra UNIQUE (Palabra)
                );
            END;

            IF OBJECT_ID(N'dbo.DelimitadoresCatalogo', N'U') IS NULL
            BEGIN
                CREATE TABLE dbo.DelimitadoresCatalogo
                (
                    IdDelimitador INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                    Simbolo NVARCHAR(5) NOT NULL,
                    Activo BIT NOT NULL,
                    CONSTRAINT UQ_DelimitadoresCatalogo_Simbolo UNIQUE (Simbolo)
                );
            END;

            IF OBJECT_ID(N'dbo.ErroresAnalisis', N'U') IS NULL
            BEGIN
                CREATE TABLE dbo.ErroresAnalisis
                (
                    IdError INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                    IdAnalisis INT NOT NULL,
                    IdErrorCatalogo INT NULL,
                    CodigoError NVARCHAR(50) NOT NULL,
                    MensajeError NVARCHAR(500) NOT NULL,
                    Lexema NVARCHAR(400) NULL,
                    NumeroLinea INT NOT NULL,
                    NumeroColumna INT NOT NULL,
                    CONSTRAINT FK_ErroresAnalisis_Analisis
                        FOREIGN KEY (IdAnalisis) REFERENCES dbo.Analisis(IdAnalisis)
                        ON DELETE CASCADE
                );
            END;

            IF COL_LENGTH(N'dbo.ErroresAnalisis', N'IdErrorCatalogo') IS NULL
            BEGIN
                ALTER TABLE dbo.ErroresAnalisis ADD IdErrorCatalogo INT NULL;
            END;

            IF COL_LENGTH(N'dbo.ErroresAnalisis', N'CodigoError') IS NULL
            BEGIN
                ALTER TABLE dbo.ErroresAnalisis ADD CodigoError NVARCHAR(50) NOT NULL CONSTRAINT DF_ErroresAnalisis_CodigoError DEFAULT (N'');
            END;

            SELECT @constraintName = dc.name
            FROM sys.default_constraints dc
            INNER JOIN sys.columns c
                ON c.object_id = dc.parent_object_id
               AND c.column_id = dc.parent_column_id
            WHERE dc.parent_object_id = OBJECT_ID(N'dbo.ErroresAnalisis')
              AND c.name = N'CodigoError';

            IF @constraintName IS NOT NULL
            BEGIN
                EXEC(N'ALTER TABLE dbo.ErroresAnalisis DROP CONSTRAINT [' + @constraintName + N']');
            END;

            IF EXISTS (
                    SELECT 1
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_NAME = 'ErroresAnalisis'
                      AND COLUMN_NAME = 'CodigoError'
                      AND CHARACTER_MAXIMUM_LENGTH < 50)
            BEGIN
                ALTER TABLE dbo.ErroresAnalisis ALTER COLUMN CodigoError NVARCHAR(50) NOT NULL;
            END;

            IF EXISTS (
                    SELECT 1
                    FROM sys.columns
                    WHERE object_id = OBJECT_ID(N'dbo.ErroresAnalisis')
                      AND name = N'IdErrorCatalogo'
                      AND is_nullable = 0)
            BEGIN
                ALTER TABLE dbo.ErroresAnalisis ALTER COLUMN IdErrorCatalogo INT NULL;
            END;

            IF OBJECT_ID(N'dbo.ErroresAnalisis', N'U') IS NOT NULL
               AND NOT EXISTS (
                    SELECT 1
                    FROM sys.foreign_keys
                    WHERE name = N'FK_ErroresAnalisis_ErroresCatalogo')
            BEGIN
                ALTER TABLE dbo.ErroresAnalisis WITH NOCHECK
                ADD CONSTRAINT FK_ErroresAnalisis_ErroresCatalogo
                    FOREIGN KEY (IdErrorCatalogo) REFERENCES dbo.ErroresCatalogo(IdErrorCatalogo);
            END;

            IF EXISTS (
                    SELECT 1
                    FROM sys.foreign_keys
                    WHERE name = N'FK_ErroresAnalisis_BaulErrores')
            BEGIN
                ALTER TABLE dbo.ErroresAnalisis DROP CONSTRAINT FK_ErroresAnalisis_BaulErrores;
            END;

            IF OBJECT_ID(N'dbo.BaulErrores', N'U') IS NOT NULL
            BEGIN
                DROP TABLE dbo.BaulErrores;
            END;

            SELECT @constraintName = dc.name
            FROM sys.default_constraints dc
            INNER JOIN sys.columns c
                ON c.object_id = dc.parent_object_id
               AND c.column_id = dc.parent_column_id
            WHERE dc.parent_object_id = OBJECT_ID(N'dbo.PalabrasReservadasCatalogo')
              AND c.name = N'Activo';

            IF @constraintName IS NOT NULL
            BEGIN
                EXEC(N'ALTER TABLE dbo.PalabrasReservadasCatalogo DROP CONSTRAINT [' + @constraintName + N']');
            END;

            IF COL_LENGTH(N'dbo.PalabrasReservadasCatalogo', N'FechaCreacion') IS NOT NULL
            BEGIN
                ALTER TABLE dbo.PalabrasReservadasCatalogo DROP COLUMN FechaCreacion;
            END;

            IF COL_LENGTH(N'dbo.PalabrasReservadasCatalogo', N'FechaModificacion') IS NOT NULL
            BEGIN
                ALTER TABLE dbo.PalabrasReservadasCatalogo DROP COLUMN FechaModificacion;
            END;

            SELECT @constraintName = dc.name
            FROM sys.default_constraints dc
            INNER JOIN sys.columns c
                ON c.object_id = dc.parent_object_id
               AND c.column_id = dc.parent_column_id
            WHERE dc.parent_object_id = OBJECT_ID(N'dbo.DelimitadoresCatalogo')
              AND c.name = N'Activo';

            IF @constraintName IS NOT NULL
            BEGIN
                EXEC(N'ALTER TABLE dbo.DelimitadoresCatalogo DROP CONSTRAINT [' + @constraintName + N']');
            END;

            IF COL_LENGTH(N'dbo.DelimitadoresCatalogo', N'FechaCreacion') IS NOT NULL
            BEGIN
                ALTER TABLE dbo.DelimitadoresCatalogo DROP COLUMN FechaCreacion;
            END;

            IF COL_LENGTH(N'dbo.DelimitadoresCatalogo', N'FechaModificacion') IS NOT NULL
            BEGIN
                ALTER TABLE dbo.DelimitadoresCatalogo DROP COLUMN FechaModificacion;
            END;

            SELECT @constraintName = dc.name
            FROM sys.default_constraints dc
            INNER JOIN sys.columns c
                ON c.object_id = dc.parent_object_id
               AND c.column_id = dc.parent_column_id
            WHERE dc.parent_object_id = OBJECT_ID(N'dbo.ErroresCatalogo')
              AND c.name = N'Activo';

            IF @constraintName IS NOT NULL
            BEGIN
                EXEC(N'ALTER TABLE dbo.ErroresCatalogo DROP CONSTRAINT [' + @constraintName + N']');
            END;

            IF COL_LENGTH(N'dbo.ErroresCatalogo', N'FechaCreacion') IS NOT NULL
            BEGIN
                ALTER TABLE dbo.ErroresCatalogo DROP COLUMN FechaCreacion;
            END;

            IF COL_LENGTH(N'dbo.ErroresCatalogo', N'FechaModificacion') IS NOT NULL
            BEGIN
                ALTER TABLE dbo.ErroresCatalogo DROP COLUMN FechaModificacion;
            END;

            IF NOT EXISTS (SELECT 1 FROM dbo.PalabrasReservadasCatalogo)
            BEGIN
                INSERT INTO dbo.PalabrasReservadasCatalogo (Palabra, Activo)
                VALUES
                {{insercionesPalabras}};
            END;

            IF NOT EXISTS (SELECT 1 FROM dbo.DelimitadoresCatalogo)
            BEGIN
                INSERT INTO dbo.DelimitadoresCatalogo (Simbolo, Activo)
                VALUES
                {{insercionesDelimitadores}};
            END;

            IF NOT EXISTS (SELECT 1 FROM dbo.ErroresCatalogo)
            BEGIN
                SET IDENTITY_INSERT dbo.ErroresCatalogo ON;

                INSERT INTO dbo.ErroresCatalogo
                (IdErrorCatalogo, CodigoError, NombreError, DescripcionError, TipoError, Activo)
                VALUES
                {{insercionesErrores}};

                SET IDENTITY_INSERT dbo.ErroresCatalogo OFF;
            END;

            {{insercionesErroresFaltantes}}

            DECLARE @idErrorContextual INT;
            DECLARE @idErrorLex009 INT;

            SELECT @idErrorContextual = IdErrorCatalogo
            FROM dbo.ErroresCatalogo
            WHERE CodigoError = N'ERR_CONTEXT_INVALID_IDENTIFIER';

            SELECT @idErrorLex009 = IdErrorCatalogo
            FROM dbo.ErroresCatalogo
            WHERE CodigoError = N'LEX009';

            IF @idErrorContextual IS NOT NULL AND @idErrorLex009 IS NULL
            BEGIN
                UPDATE dbo.ErroresCatalogo
                SET CodigoError = N'LEX009'
                WHERE IdErrorCatalogo = @idErrorContextual;

                SET @idErrorLex009 = @idErrorContextual;
                SET @idErrorContextual = NULL;
            END;

            IF @idErrorLex009 IS NOT NULL
            BEGIN
                UPDATE dbo.ErroresAnalisis
                SET IdErrorCatalogo = @idErrorLex009,
                    CodigoError = N'LEX009'
                WHERE IdErrorCatalogo = @idErrorLex009
                   OR IdErrorCatalogo = @idErrorContextual
                   OR CodigoError IN (N'LEX009', N'ERR_CONTEXT_INVALID_IDENTIFIER');
            END;

            IF @idErrorContextual IS NOT NULL AND @idErrorLex009 IS NOT NULL
            BEGIN
                DELETE FROM dbo.ErroresCatalogo
                WHERE IdErrorCatalogo = @idErrorContextual;
            END;
            """;
    }

    private static string EscaparSqlLiteral(string valor)
    {
        return valor.Replace("'", "''", StringComparison.Ordinal);
    }
}
