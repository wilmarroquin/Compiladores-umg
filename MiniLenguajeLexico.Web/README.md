# MiniLenguajeLexico.Web

Aplicacion ASP.NET Core MVC para ejecutar un analisis lexico, persistir el resultado en SQL Server y mostrar el detalle de tokens y errores en la interfaz web.

## Documentacion

- [Proyectos y atribuciones](D:\Proyecto\MiniLenguajeLexico.Web\docs\proyectos-y-atribuciones.md)

## Requisitos

- .NET 8 SDK
- SQL Server o SQL Server Express

## Configuracion

La aplicacion necesita la cadena `ConnectionStrings:CadenaSql`.

1. Crea la base y tablas con los scripts:
   - [database/01-create-analizador-schema.sql](database/01-create-analizador-schema.sql)
   - [database/02-create-configuracion-lexica.sql](database/02-create-configuracion-lexica.sql)
2. Define la cadena de conexion en alguno de estos lugares:
   - `MiniLenguajeLexico.Web/appsettings.Development.json`
   - `MiniLenguajeLexico.Web/appsettings.Development.Local.json`
   - variables de entorno
   - secretos de usuario

Ejemplo:

```json
{
  "ConnectionStrings": {
    "CadenaSql": "Server=.\\SQLEXPRESS;Database=AnalizadorLexicoDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Para entornos compartidos, evita guardar usuarios y contrasenas reales dentro del repositorio. Prefiere secretos de usuario o variables de entorno.

Si necesitas una configuracion solo para tu maquina, usa `MiniLenguajeLexico.Web/appsettings.Development.Local.json`. Ese archivo tiene prioridad sobre `appsettings.Development.json` y esta ignorado por el repositorio.

Desde esta version, al iniciar la aplicacion tambien se validan y crean automaticamente las tablas de configuracion lexica (`PalabrasReservadasCatalogo`, `DelimitadoresCatalogo`, `ErroresCatalogo`) si no existen. Esos catalogos ya no se leen desde memoria como respaldo: la fuente de verdad es la base de datos.

## Ejecucion

```powershell
dotnet run --project .\MiniLenguajeLexico.Web
```

Luego abre la raiz del sitio y envia codigo fuente desde la pantalla del analizador.

## Pruebas

```powershell
dotnet test
```
