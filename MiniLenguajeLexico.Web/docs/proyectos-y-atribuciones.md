# Proyectos y atribuciones

Este documento describe la responsabilidad de cada proyecto dentro de la solucion `MiniLenguajeLexico.Web`, como se relacionan entre si y cuales son sus archivos mas importantes.

## Vision general

La solucion esta organizada por capas:

- `MiniLenguajeLexico.Domain`: define el modelo del negocio.
- `MiniLenguajeLexico.Application`: contiene la logica de aplicacion y las interfaces.
- `MiniLenguajeLexico.Infrastructure`: implementa la persistencia en SQL Server.
- `MiniLenguajeLexico.Web`: expone la interfaz MVC y orquesta la ejecucion.
- `MiniLenguajeLexico.Tests`: valida servicios y controladores.

Flujo principal:

1. El usuario envia codigo desde `MiniLenguajeLexico.Web`.
2. `MiniLenguajeLexico.Application` ejecuta el analisis lexico y consulta catalogos.
3. `MiniLenguajeLexico.Infrastructure` lee y guarda datos en SQL Server.
4. `MiniLenguajeLexico.Domain` aporta las entidades y enums compartidos.
5. `MiniLenguajeLexico.Tests` protege el comportamiento esperado.

## 1. MiniLenguajeLexico.Domain

Ruta: [MiniLenguajeLexico.Domain](D:\Proyecto\MiniLenguajeLexico.Web\MiniLenguajeLexico.Domain)

### Responsabilidad

Es la capa base del dominio. Define las estructuras que representan el problema del negocio sin depender de ASP.NET Core ni de SQL Server.

### Atribuciones

- Modelar un analisis lexico mediante entidades.
- Representar tokens, errores y catalogos configurables.
- Exponer enums de estado y tipo de token.
- Proveer catalogos predeterminados que sirven para inicializacion de base de datos.

### Archivos clave

- [Analisis.cs](D:\Proyecto\MiniLenguajeLexico.Web\MiniLenguajeLexico.Domain\Entities\Analisis.cs): entidad agregada que contiene estado, tokens y errores de un analisis.
- [TokenAnalisis.cs](D:\Proyecto\MiniLenguajeLexico.Web\MiniLenguajeLexico.Domain\Entities\TokenAnalisis.cs): representa un token identificado.
- [ErrorAnalisis.cs](D:\Proyecto\MiniLenguajeLexico.Web\MiniLenguajeLexico.Domain\Entities\ErrorAnalisis.cs): representa un error detectado durante el analisis.
- [PalabraReservadaCatalogo.cs](D:\Proyecto\MiniLenguajeLexico.Web\MiniLenguajeLexico.Domain\Entities\PalabraReservadaCatalogo.cs): modelo de una palabra reservada configurable.
- [DelimitadorCatalogo.cs](D:\Proyecto\MiniLenguajeLexico.Web\MiniLenguajeLexico.Domain\Entities\DelimitadorCatalogo.cs): modelo de delimitador configurable.
- [ErrorCatalogo.cs](D:\Proyecto\MiniLenguajeLexico.Web\MiniLenguajeLexico.Domain\Entities\ErrorCatalogo.cs): modelo del baul de errores.
- [EstadoAnalisis.cs](D:\Proyecto\MiniLenguajeLexico.Web\MiniLenguajeLexico.Domain\Enums\EstadoAnalisis.cs): estados posibles del analisis.
- [TipoToken.cs](D:\Proyecto\MiniLenguajeLexico.Web\MiniLenguajeLexico.Domain\Enums\TipoToken.cs): clasificacion funcional de tokens.
- [PalabrasReversadas.cs](D:\Proyecto\MiniLenguajeLexico.Web\MiniLenguajeLexico.Domain\Constants\PalabrasReversadas.cs): lista base de palabras reservadas.
- [CatalogoDelimitadores.cs](D:\Proyecto\MiniLenguajeLexico.Web\MiniLenguajeLexico.Domain\Constants\CatalogoDelimitadores.cs): lista base de delimitadores.
- [BaulErroresCatalogo.cs](D:\Proyecto\MiniLenguajeLexico.Web\MiniLenguajeLexico.Domain\Constants\BaulErroresCatalogo.cs): lista base de errores del sistema.

### Dependencias

- No depende de otros proyectos de la solucion.

## 2. MiniLenguajeLexico.Application

Ruta: [MiniLenguajeLexico.Application](D:\Proyecto\MiniLenguajeLexico.Web\MiniLenguajeLexico.Application)

### Responsabilidad

Implementa los casos de uso y la logica de aplicacion. Es la capa que conecta el dominio con la infraestructura y la interfaz web.

### Atribuciones

- Ejecutar el analisis lexico del codigo fuente.
- Aplicar reglas de validacion de identificadores y errores contextuales.
- Convertir entidades del dominio a DTOs consumibles por la web.
- Definir contratos para repositorios y servicios.
- Encapsular la gestion de catalogos: palabras reservadas, delimitadores y baul de errores.

### Archivos clave

- [ServicioAnalizadorLexico.cs](D:\Proyecto\MiniLenguajeLexico.Web\MiniLenguajeLexico.Application\Services\ServicioAnalizadorLexico.cs): caso de uso principal del analizador.
- [AnalizadorLexicoMotor.cs](D:\Proyecto\MiniLenguajeLexico.Web\MiniLenguajeLexico.Application\Services\AnalizadorLexicoMotor.cs): motor interno que tokeniza y detecta errores.
- [ResultadoMotorLexico.cs](D:\Proyecto\MiniLenguajeLexico.Web\MiniLenguajeLexico.Application\Services\ResultadoMotorLexico.cs): estructura interna del resultado del motor.
- [ServicioPalabrasReservadas.cs](D:\Proyecto\MiniLenguajeLexico.Web\MiniLenguajeLexico.Application\Services\ServicioPalabrasReservadas.cs): administra palabras reservadas desde la fuente de datos.
- [ServicioDelimitadores.cs](D:\Proyecto\MiniLenguajeLexico.Web\MiniLenguajeLexico.Application\Services\ServicioDelimitadores.cs): administra delimitadores.
- [ServicioBaulErrores.cs](D:\Proyecto\MiniLenguajeLexico.Web\MiniLenguajeLexico.Application\Services\ServicioBaulErrores.cs): administra el catalogo de errores.
- [IServicioAnalizadorLexico.cs](D:\Proyecto\MiniLenguajeLexico.Web\MiniLenguajeLexico.Application\Interfaces\IServicioAnalizadorLexico.cs): contrato del caso de uso principal.
- [IRepositorioAnalisis.cs](D:\Proyecto\MiniLenguajeLexico.Web\MiniLenguajeLexico.Application\Interfaces\IRepositorioAnalisis.cs): contrato para persistir analisis.
- [IRepositorioConfiguracionLexica.cs](D:\Proyecto\MiniLenguajeLexico.Web\MiniLenguajeLexico.Application\Interfaces\IRepositorioConfiguracionLexica.cs): contrato para catalogos configurables.
- [SolicitudAnalisisDto.cs](D:\Proyecto\MiniLenguajeLexico.Web\MiniLenguajeLexico.Application\DTOs\SolicitudAnalisisDto.cs): entrada del caso de uso.
- [ResultadoAnalisisDto.cs](D:\Proyecto\MiniLenguajeLexico.Web\MiniLenguajeLexico.Application\DTOs\ResultadoAnalisisDto.cs): salida del caso de uso para la UI.

### Dependencias

- Depende de [MiniLenguajeLexico.Domain](D:\Proyecto\MiniLenguajeLexico.Web\MiniLenguajeLexico.Domain).
- No conoce detalles de SQL Server ni de MVC.

## 3. MiniLenguajeLexico.Infrastructure

Ruta: [MiniLenguajeLexico.Infrastructure](D:\Proyecto\MiniLenguajeLexico.Web\MiniLenguajeLexico.Infrastructure)

### Responsabilidad

Implementa la persistencia y la inicializacion del esquema necesario en SQL Server.

### Atribuciones

- Guardar el resultado de cada analisis en base de datos.
- Consultar y administrar catalogos persistentes.
- Crear o ajustar tablas y restricciones necesarias al arrancar la aplicacion.
- Mantener coherencia entre catalogos predeterminados del dominio y el contenido de la base.

### Archivos clave

- [RepositorioAnalisisSql.cs](D:\Proyecto\MiniLenguajeLexico.Web\MiniLenguajeLexico.Infrastructure\Repositories\RepositorioAnalisisSql.cs): guarda analisis, tokens y errores.
- [RepositorioConfiguracionLexicaSql.cs](D:\Proyecto\MiniLenguajeLexico.Web\MiniLenguajeLexico.Infrastructure\Repositories\RepositorioConfiguracionLexicaSql.cs): inicializa catalogos y gestiona palabras reservadas, delimitadores y errores.

### Dependencias

- Depende de [MiniLenguajeLexico.Application](D:\Proyecto\MiniLenguajeLexico.Web\MiniLenguajeLexico.Application).
- Depende de [MiniLenguajeLexico.Domain](D:\Proyecto\MiniLenguajeLexico.Web\MiniLenguajeLexico.Domain).
- Usa `Microsoft.Data.SqlClient` para acceder a SQL Server.

## 4. MiniLenguajeLexico.Web

Ruta: [MiniLenguajeLexico.Web](D:\Proyecto\MiniLenguajeLexico.Web\MiniLenguajeLexico.Web)

### Responsabilidad

Es la aplicacion ASP.NET Core MVC que expone la experiencia de usuario.

### Atribuciones

- Configurar el contenedor de dependencias.
- Inicializar catalogos al arranque.
- Recibir solicitudes HTTP.
- Validar formularios y coordinar llamadas a servicios.
- Renderizar vistas del analizador y de la administracion de catalogos.

### Archivos clave

- [Program.cs](D:\Proyecto\MiniLenguajeLexico.Web\MiniLenguajeLexico.Web\Program.cs): composicion de la aplicacion, rutas, DI e inicializacion de catalogos.
- [AnalizadorController.cs](D:\Proyecto\MiniLenguajeLexico.Web\MiniLenguajeLexico.Web\Controllers\AnalizadorController.cs): flujo del analizador.
- [PalabrasReservadasController.cs](D:\Proyecto\MiniLenguajeLexico.Web\MiniLenguajeLexico.Web\Controllers\PalabrasReservadasController.cs): alta, edicion y eliminacion de palabras reservadas.
- [DelimitadoresController.cs](D:\Proyecto\MiniLenguajeLexico.Web\MiniLenguajeLexico.Web\Controllers\DelimitadoresController.cs): administracion de delimitadores.
- [BaulErroresController.cs](D:\Proyecto\MiniLenguajeLexico.Web\MiniLenguajeLexico.Web\Controllers\BaulErroresController.cs): administracion del catalogo de errores.
- [AnalizadorViewModel.cs](D:\Proyecto\MiniLenguajeLexico.Web\MiniLenguajeLexico.Web\Models\AnalizadorViewModel.cs): modelo de presentacion del analizador.
- [Views\Analizador\Index.cshtml](D:\Proyecto\MiniLenguajeLexico.Web\MiniLenguajeLexico.Web\Views\Analizador\Index.cshtml): pantalla principal de analisis.
- [Views\PalabrasReservadas\Index.cshtml](D:\Proyecto\MiniLenguajeLexico.Web\MiniLenguajeLexico.Web\Views\PalabrasReservadas\Index.cshtml): vista de administracion de palabras reservadas.
- [Views\Delimitadores\Index.cshtml](D:\Proyecto\MiniLenguajeLexico.Web\MiniLenguajeLexico.Web\Views\Delimitadores\Index.cshtml): vista de administracion de delimitadores.
- [Views\BaulErrores\Index.cshtml](D:\Proyecto\MiniLenguajeLexico.Web\MiniLenguajeLexico.Web\Views\BaulErrores\Index.cshtml): vista de administracion del baul de errores.
- [wwwroot\css\site.css](D:\Proyecto\MiniLenguajeLexico.Web\MiniLenguajeLexico.Web\wwwroot\css\site.css): estilos globales.

### Dependencias

- Depende de [MiniLenguajeLexico.Application](D:\Proyecto\MiniLenguajeLexico.Web\MiniLenguajeLexico.Application).
- Depende de [MiniLenguajeLexico.Domain](D:\Proyecto\MiniLenguajeLexico.Web\MiniLenguajeLexico.Domain).
- Depende de [MiniLenguajeLexico.Infrastructure](D:\Proyecto\MiniLenguajeLexico.Web\MiniLenguajeLexico.Infrastructure).

## 5. MiniLenguajeLexico.Tests

Ruta: [MiniLenguajeLexico.Tests](D:\Proyecto\MiniLenguajeLexico.Web\MiniLenguajeLexico.Tests)

### Responsabilidad

Protege el comportamiento funcional de la solucion con pruebas automatizadas.

### Atribuciones

- Verificar el motor de analisis lexico y sus reglas.
- Validar servicios de configuracion.
- Validar controladores MVC.
- Simular repositorios con dobles de prueba para aislar escenarios.

### Archivos clave

- [ServicioAnalizadorLexicoTests.cs](D:\Proyecto\MiniLenguajeLexico.Web\MiniLenguajeLexico.Tests\ServicioAnalizadorLexicoTests.cs): cobertura del analizador y reglas de identificadores.
- [ServicioPalabrasReservadasTests.cs](D:\Proyecto\MiniLenguajeLexico.Web\MiniLenguajeLexico.Tests\ServicioPalabrasReservadasTests.cs): pruebas del servicio de palabras reservadas.
- [ServicioDelimitadoresTests.cs](D:\Proyecto\MiniLenguajeLexico.Web\MiniLenguajeLexico.Tests\ServicioDelimitadoresTests.cs): pruebas del servicio de delimitadores.
- [ServicioBaulErroresTests.cs](D:\Proyecto\MiniLenguajeLexico.Web\MiniLenguajeLexico.Tests\ServicioBaulErroresTests.cs): pruebas del baul de errores.
- [AnalizadorControllerTests.cs](D:\Proyecto\MiniLenguajeLexico.Web\MiniLenguajeLexico.Tests\AnalizadorControllerTests.cs): pruebas del controlador principal.
- [PalabrasReservadasControllerTests.cs](D:\Proyecto\MiniLenguajeLexico.Web\MiniLenguajeLexico.Tests\PalabrasReservadasControllerTests.cs): pruebas del CRUD de palabras reservadas.
- [DelimitadoresControllerTests.cs](D:\Proyecto\MiniLenguajeLexico.Web\MiniLenguajeLexico.Tests\DelimitadoresControllerTests.cs): pruebas del CRUD de delimitadores.
- [BaulErroresControllerTests.cs](D:\Proyecto\MiniLenguajeLexico.Web\MiniLenguajeLexico.Tests\BaulErroresControllerTests.cs): pruebas del CRUD del baul de errores.
- [FakeRepositorioConfiguracionLexica.cs](D:\Proyecto\MiniLenguajeLexico.Web\MiniLenguajeLexico.Tests\FakeRepositorioConfiguracionLexica.cs): doble de prueba en memoria.
- [ThrowingRepositorioConfiguracionLexica.cs](D:\Proyecto\MiniLenguajeLexico.Web\MiniLenguajeLexico.Tests\ThrowingRepositorioConfiguracionLexica.cs): doble que fuerza errores de infraestructura.

### Dependencias

- Depende de [MiniLenguajeLexico.Application](D:\Proyecto\MiniLenguajeLexico.Web\MiniLenguajeLexico.Application).
- Depende de [MiniLenguajeLexico.Domain](D:\Proyecto\MiniLenguajeLexico.Web\MiniLenguajeLexico.Domain).
- Depende de [MiniLenguajeLexico.Web](D:\Proyecto\MiniLenguajeLexico.Web\MiniLenguajeLexico.Web) para probar controladores y view models.
- Usa `xUnit` y `Microsoft.NET.Test.Sdk`.

## Carpeta `database`

Ruta: [database](D:\Proyecto\MiniLenguajeLexico.Web\database)

No es un proyecto .NET, pero si es parte importante de la solucion.

### Atribuciones

- Versionar el esquema base de SQL Server.
- Permitir instalaciones nuevas o reparaciones controladas.

### Archivos clave

- [01-create-analizador-schema.sql](D:\Proyecto\MiniLenguajeLexico.Web\database\01-create-analizador-schema.sql): crea o ajusta tablas del analisis.
- [02-create-configuracion-lexica.sql](D:\Proyecto\MiniLenguajeLexico.Web\database\02-create-configuracion-lexica.sql): crea o ajusta catalogos configurables.

## Dependencias entre proyectos

```text
MiniLenguajeLexico.Domain
        ^
        |
MiniLenguajeLexico.Application
        ^
        |
MiniLenguajeLexico.Infrastructure

MiniLenguajeLexico.Web ----> Application
MiniLenguajeLexico.Web ----> Domain
MiniLenguajeLexico.Web ----> Infrastructure

MiniLenguajeLexico.Tests ---> Web
MiniLenguajeLexico.Tests ---> Application
MiniLenguajeLexico.Tests ---> Domain
```

## Regla practica para futuras modificaciones

- Si cambias entidades, enums o catalogos base, el lugar correcto suele ser `Domain`.
- Si cambias reglas del analizador o casos de uso, el lugar correcto suele ser `Application`.
- Si cambias SQL, repositorios o bootstrap de base de datos, el lugar correcto suele ser `Infrastructure` y `database`.
- Si cambias formularios, navegacion o vistas, el lugar correcto suele ser `Web`.
- Si corriges regresiones o agregas cobertura, el lugar correcto es `Tests`.
