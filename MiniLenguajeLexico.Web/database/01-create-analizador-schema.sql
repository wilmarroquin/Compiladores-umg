IF DB_ID(N'AnalizadorLexicoDb') IS NULL
BEGIN
    CREATE DATABASE AnalizadorLexicoDb;
END;
GO

USE AnalizadorLexicoDb;
GO

IF OBJECT_ID(N'dbo.Analisis', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Analisis
    (
        IdAnalisis INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        EstadoAnalisis NVARCHAR(50) NOT NULL
    );
END;
GO

DECLARE @ConstraintName SYSNAME;

IF COL_LENGTH(N'dbo.Analisis', N'FechaRegistro') IS NOT NULL
BEGIN
    SELECT @ConstraintName = dc.name
    FROM sys.default_constraints dc
    INNER JOIN sys.columns c
        ON c.object_id = dc.parent_object_id
       AND c.column_id = dc.parent_column_id
    WHERE dc.parent_object_id = OBJECT_ID(N'dbo.Analisis')
      AND c.name = N'FechaRegistro';

    IF @ConstraintName IS NOT NULL
    BEGIN
        EXEC(N'ALTER TABLE dbo.Analisis DROP CONSTRAINT [' + @ConstraintName + N']');
    END;

    ALTER TABLE dbo.Analisis DROP COLUMN FechaRegistro;
END;
GO

IF COL_LENGTH(N'dbo.Analisis', N'CodigoFuente') IS NOT NULL
BEGIN
    ALTER TABLE dbo.Analisis DROP COLUMN CodigoFuente;
END;
GO

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
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_TokensAnalisis_IdAnalisis'
      AND object_id = OBJECT_ID(N'dbo.TokensAnalisis'))
BEGIN
    CREATE INDEX IX_TokensAnalisis_IdAnalisis ON dbo.TokensAnalisis(IdAnalisis);
END;
GO

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
GO

IF COL_LENGTH(N'dbo.ErroresAnalisis', N'IdErrorCatalogo') IS NULL
BEGIN
    ALTER TABLE dbo.ErroresAnalisis ADD IdErrorCatalogo INT NULL;
END;
GO

IF COL_LENGTH(N'dbo.ErroresAnalisis', N'CodigoError') IS NULL
BEGIN
    ALTER TABLE dbo.ErroresAnalisis ADD CodigoError NVARCHAR(50) NOT NULL CONSTRAINT DF_ErroresAnalisis_CodigoError DEFAULT (N'');
END;
GO

DECLARE @CodigoErrorDefault SYSNAME;

SELECT @CodigoErrorDefault = dc.name
FROM sys.default_constraints dc
INNER JOIN sys.columns c
    ON c.object_id = dc.parent_object_id
   AND c.column_id = dc.parent_column_id
WHERE dc.parent_object_id = OBJECT_ID(N'dbo.ErroresAnalisis')
  AND c.name = N'CodigoError';

IF @CodigoErrorDefault IS NOT NULL
BEGIN
    EXEC(N'ALTER TABLE dbo.ErroresAnalisis DROP CONSTRAINT [' + @CodigoErrorDefault + N']');
END;
GO

IF EXISTS (
    SELECT 1
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.ErroresAnalisis')
      AND name = N'IdErrorCatalogo'
      AND is_nullable = 0)
BEGIN
    ALTER TABLE dbo.ErroresAnalisis ALTER COLUMN IdErrorCatalogo INT NULL;
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_ErroresAnalisis_IdAnalisis'
      AND object_id = OBJECT_ID(N'dbo.ErroresAnalisis'))
BEGIN
    CREATE INDEX IX_ErroresAnalisis_IdAnalisis ON dbo.ErroresAnalisis(IdAnalisis);
END;
GO

IF EXISTS (
    SELECT 1
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'ErroresAnalisis'
      AND COLUMN_NAME = 'CodigoError'
      AND CHARACTER_MAXIMUM_LENGTH < 50)
BEGIN
    ALTER TABLE dbo.ErroresAnalisis ALTER COLUMN CodigoError NVARCHAR(50) NOT NULL;
END;
GO
