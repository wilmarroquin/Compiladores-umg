USE AnalizadorLexicoDb;
GO

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
GO

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
GO

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
GO

DECLARE @ConstraintName SYSNAME;

SELECT @ConstraintName = dc.name
FROM sys.default_constraints dc
INNER JOIN sys.columns c
    ON c.object_id = dc.parent_object_id
   AND c.column_id = dc.parent_column_id
WHERE dc.parent_object_id = OBJECT_ID(N'dbo.PalabrasReservadasCatalogo')
  AND c.name = N'Activo';

IF @ConstraintName IS NOT NULL
BEGIN
    EXEC(N'ALTER TABLE dbo.PalabrasReservadasCatalogo DROP CONSTRAINT [' + @ConstraintName + N']');
END;
GO

IF COL_LENGTH(N'dbo.PalabrasReservadasCatalogo', N'FechaCreacion') IS NOT NULL
BEGIN
    ALTER TABLE dbo.PalabrasReservadasCatalogo DROP COLUMN FechaCreacion;
END;
GO

IF COL_LENGTH(N'dbo.PalabrasReservadasCatalogo', N'FechaModificacion') IS NOT NULL
BEGIN
    ALTER TABLE dbo.PalabrasReservadasCatalogo DROP COLUMN FechaModificacion;
END;
GO

DECLARE @ConstraintName SYSNAME;

SELECT @ConstraintName = dc.name
FROM sys.default_constraints dc
INNER JOIN sys.columns c
    ON c.object_id = dc.parent_object_id
   AND c.column_id = dc.parent_column_id
WHERE dc.parent_object_id = OBJECT_ID(N'dbo.DelimitadoresCatalogo')
  AND c.name = N'Activo';

IF @ConstraintName IS NOT NULL
BEGIN
    EXEC(N'ALTER TABLE dbo.DelimitadoresCatalogo DROP CONSTRAINT [' + @ConstraintName + N']');
END;
GO

IF COL_LENGTH(N'dbo.DelimitadoresCatalogo', N'FechaCreacion') IS NOT NULL
BEGIN
    ALTER TABLE dbo.DelimitadoresCatalogo DROP COLUMN FechaCreacion;
END;
GO

IF COL_LENGTH(N'dbo.DelimitadoresCatalogo', N'FechaModificacion') IS NOT NULL
BEGIN
    ALTER TABLE dbo.DelimitadoresCatalogo DROP COLUMN FechaModificacion;
END;
GO

DECLARE @ConstraintName SYSNAME;

SELECT @ConstraintName = dc.name
FROM sys.default_constraints dc
INNER JOIN sys.columns c
    ON c.object_id = dc.parent_object_id
   AND c.column_id = dc.parent_column_id
WHERE dc.parent_object_id = OBJECT_ID(N'dbo.ErroresCatalogo')
  AND c.name = N'Activo';

IF @ConstraintName IS NOT NULL
BEGIN
    EXEC(N'ALTER TABLE dbo.ErroresCatalogo DROP CONSTRAINT [' + @ConstraintName + N']');
END;
GO

IF COL_LENGTH(N'dbo.ErroresCatalogo', N'FechaCreacion') IS NOT NULL
BEGIN
    ALTER TABLE dbo.ErroresCatalogo DROP COLUMN FechaCreacion;
END;
GO

IF COL_LENGTH(N'dbo.ErroresCatalogo', N'FechaModificacion') IS NOT NULL
BEGIN
    ALTER TABLE dbo.ErroresCatalogo DROP COLUMN FechaModificacion;
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = N'FK_ErroresAnalisis_ErroresCatalogo')
BEGIN
    ALTER TABLE dbo.ErroresAnalisis WITH NOCHECK
    ADD CONSTRAINT FK_ErroresAnalisis_ErroresCatalogo
        FOREIGN KEY (IdErrorCatalogo) REFERENCES dbo.ErroresCatalogo(IdErrorCatalogo);
END;
GO

IF EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = N'FK_ErroresAnalisis_BaulErrores')
BEGIN
    ALTER TABLE dbo.ErroresAnalisis DROP CONSTRAINT FK_ErroresAnalisis_BaulErrores;
END;
GO

IF OBJECT_ID(N'dbo.BaulErrores', N'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.BaulErrores;
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.PalabrasReservadasCatalogo)
BEGIN
    INSERT INTO dbo.PalabrasReservadasCatalogo (Palabra, Activo)
    VALUES
    (N'auto', 1),
    (N'bool', 1),
    (N'break', 1),
    (N'case', 1),
    (N'char', 1),
    (N'const', 1),
    (N'continue', 1),
    (N'default', 1),
    (N'do', 1),
    (N'double', 1),
    (N'else', 1),
    (N'enum', 1),
    (N'extern', 1),
    (N'float', 1),
    (N'for', 1),
    (N'goto', 1),
    (N'if', 1),
    (N'int', 1),
    (N'long', 1),
    (N'print', 1),
    (N'register', 1),
    (N'return', 1),
    (N'short', 1),
    (N'signed', 1),
    (N'sizeof', 1),
    (N'static', 1),
    (N'string', 1),
    (N'struct', 1),
    (N'switch', 1),
    (N'typedef', 1),
    (N'union', 1),
    (N'unsigned', 1),
    (N'void', 1),
    (N'volatile', 1),
    (N'while', 1);
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.DelimitadoresCatalogo)
BEGIN
    INSERT INTO dbo.DelimitadoresCatalogo (Simbolo, Activo)
    VALUES
    (N'(', 1),
    (N')', 1),
    (N'{', 1),
    (N'}', 1),
    (N'[', 1),
    (N']', 1),
    (N';', 1),
    (N',', 1);
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.ErroresCatalogo)
BEGIN
    SET IDENTITY_INSERT dbo.ErroresCatalogo ON;

    INSERT INTO dbo.ErroresCatalogo
    (IdErrorCatalogo, CodigoError, NombreError, DescripcionError, TipoError, Activo)
    VALUES
    (1, N'LEX001', N'Simbolo no reconocido', N'Simbolo no reconocido.', N'Lexico', 1),
    (2, N'LEX002', N'Cadena no cerrada', N'Cadena no cerrada correctamente.', N'Lexico', 1),
    (3, N'LEX003', N'Comentario de bloque no cerrado', N'Comentario de bloque no cerrado.', N'Lexico', 1),
    (4, N'LEX004', N'Literal de caracter invalido', N'Literal de caracter invalido.', N'Lexico', 1),
    (5, N'LEX005', N'Literal de caracter no cerrado', N'Literal de caracter no cerrado.', N'Lexico', 1),
    (6, N'LEX006', N'Delimitador sin apertura', N'Delimitador de cierre sin apertura correspondiente.', N'Lexico', 1),
    (7, N'LEX007', N'Delimitador no coincide', N'Delimitador de apertura no coincide con el cierre detectado.', N'Lexico', 1),
    (8, N'LEX008', N'Delimitador sin cierre', N'Delimitador sin cierre correspondiente.', N'Lexico', 1),
    (9, N'LEX009', N'Identificador invalido en contexto', N'El identificador no cumple con las reglas del lenguaje o su contexto de uso.', N'Contextual', 1);

    SET IDENTITY_INSERT dbo.ErroresCatalogo OFF;
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.ErroresCatalogo WHERE CodigoError = N'LEX009')
BEGIN
    INSERT INTO dbo.ErroresCatalogo (CodigoError, NombreError, DescripcionError, TipoError, Activo)
    VALUES (N'LEX009', N'Identificador invalido en contexto', N'El identificador no cumple con las reglas del lenguaje o su contexto de uso.', N'Contextual', 1);
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

DECLARE @IdErrorContextual INT;
DECLARE @IdErrorLex009 INT;

SELECT @IdErrorContextual = IdErrorCatalogo
FROM dbo.ErroresCatalogo
WHERE CodigoError = N'ERR_CONTEXT_INVALID_IDENTIFIER';

SELECT @IdErrorLex009 = IdErrorCatalogo
FROM dbo.ErroresCatalogo
WHERE CodigoError = N'LEX009';

IF @IdErrorContextual IS NOT NULL AND @IdErrorLex009 IS NULL
BEGIN
    UPDATE dbo.ErroresCatalogo
    SET CodigoError = N'LEX009'
    WHERE IdErrorCatalogo = @IdErrorContextual;

    SET @IdErrorLex009 = @IdErrorContextual;
    SET @IdErrorContextual = NULL;
END;

IF @IdErrorLex009 IS NOT NULL
BEGIN
    UPDATE dbo.ErroresAnalisis
    SET IdErrorCatalogo = @IdErrorLex009,
        CodigoError = N'LEX009'
    WHERE IdErrorCatalogo = @IdErrorLex009
       OR IdErrorCatalogo = @IdErrorContextual
       OR CodigoError IN (N'LEX009', N'ERR_CONTEXT_INVALID_IDENTIFIER');
END;

IF @IdErrorContextual IS NOT NULL AND @IdErrorLex009 IS NOT NULL
BEGIN
    DELETE FROM dbo.ErroresCatalogo
    WHERE IdErrorCatalogo = @IdErrorContextual;
END;
GO
