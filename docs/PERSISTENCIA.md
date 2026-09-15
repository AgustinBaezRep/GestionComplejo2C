# Persistencia con EF Core + SQL Server

Guía de cómo está armada la persistencia del proyecto, con foco en las **tres relaciones**
tomadas del diagrama de clases (`docs/Class.Diagram/GestionComplejoUML.png`):

| Relación | Entidades | Cómo la infiere EF Core |
|---|---|---|
| **1 : \*** | `Cancha` → `Reserva` | Una colección de un lado + una FK del otro. |
| **1 : 1** | `Cancha` → `Vestuario` (0..1) | Una referencia de cada lado + la FK en el dependiente. |
| **\* : \*** | `Cancha` ↔ `Servicio` | Una colección de cada lado. EF crea la tabla intermedia sola. |

El criterio fue **no configurar nada en `OnModelCreating`** ni con atributos: EF Core deduce
el esquema entero a partir de las entidades.

---

## 1. Paquetes: cuál va en qué capa

| Capa | Paquete | Por qué ahí |
|---|---|---|
| Domain | *ninguno* | El dominio no debe saber que existe una base de datos. |
| Application | *ninguno* | Solo depende de las interfaces de repositorio del Domain. |
| Infrastructure | `Microsoft.EntityFrameworkCore.SqlServer` | Es la capa que implementa la persistencia. |
| Presentation | `Microsoft.EntityFrameworkCore.Design` | Lo necesita el **startup project** para que `dotnet ef` funcione. No se usa en runtime. |

```bash
dotnet add GestionComplejo2C.Infrastructure package Microsoft.EntityFrameworkCore.SqlServer
dotnet add GestionComplejo2C.Presentation package Microsoft.EntityFrameworkCore.Design
```

---

## 2. Connection string

`GestionComplejo2C.Presentation/appsettings.json`:

```json
"ConnectionStrings": {
  "GestionComplejoDb": "Server=localhost;Database=GestionComplejo2C;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True"
}
```

- `Server` — instancia de SQL Server. Si el nombre lleva backslash, en JSON va escapado: `localhost\\SQLEXPRESS`.
- `Database` — la base se crea sola al correr `database update`.
- `Trusted_Connection=True` — autenticación de Windows.
- `TrustServerCertificate=True` — certificado autofirmado del SQL local. **Solo desarrollo.**

---

## 3. `GestionComplejoDbContext`: cuatro `DbSet` y nada más

```csharp
public class GestionComplejoDbContext : DbContext
{
    public GestionComplejoDbContext(DbContextOptions<GestionComplejoDbContext> options)
        : base(options)
    {
    }

    public DbSet<Cancha> Canchas => Set<Cancha>();
    public DbSet<Reserva> Reservas => Set<Reserva>();
    public DbSet<Vestuario> Vestuarios => Set<Vestuario>();
    public DbSet<Servicio> Servicios => Set<Servicio>();
}
```

No hay `OnModelCreating`. Cada `DbSet` le dice a EF "esta clase es una tabla" y le da el nombre
(`Canchas`, `Reservas`, ...). Todo lo demás lo lee de las propiedades de las clases.

> **Punto de clase:** el `DbContext` es el Unit of Work. Acumula cambios en su `ChangeTracker`
> y recién los manda a la base en `SaveChanges()`, dentro de una transacción.

---

## 4. Las tres relaciones, tal como están en las entidades

### 1 : \*  —  `Cancha` tiene muchas `Reserva`

```csharp
// Cancha
public List<Reserva> Reservas { get; private set; } = new List<Reserva>();

// Reserva
public Guid CanchaId { get; private set; }
```

EF ve una colección de `Reserva` en `Cancha` y una propiedad `CanchaId` en `Reserva`
(`<NombreDeLaEntidad>Id`). Con eso arma la FK. Como `CanchaId` es `Guid` y no `Guid?`, la
relación es **requerida**, y para relaciones requeridas EF elige `ON DELETE CASCADE`.

No hace falta navegación inversa (`Reserva.Cancha`). Una relación puede ser unidireccional.

### 1 : 1  —  `Cancha` tiene un `Vestuario` opcional

```csharp
// Cancha
public Guid? VestuarioId { get; private set; }
public Vestuario? Vestuario { get; private set; }

// Vestuario
public Cancha? Cancha { get; private set; }
```

Acá hay tres piezas y las tres importan:

1. **Referencia de cada lado** (`Cancha.Vestuario` y `Vestuario.Cancha`). Es lo que le dice a
   EF que es 1:1. Con una sola referencia, EF asumiría 1:\* (muchas canchas para un vestuario).
2. **`VestuarioId` en `Cancha`.** Decide quién es el *dependiente* (el que lleva la FK). Sin una
   FK en ninguno de los dos lados EF no sabe elegir y tira excepción al construir el modelo.
3. **`Guid?` nullable.** Hace la relación *opcional*: una cancha puede no tener vestuario (el
   `0..1` del diagrama).

Para garantizar el "1" del lado de la cancha, EF crea un **índice único** sobre `VestuarioId`
(filtrado para ignorar los `NULL`). Dos canchas no pueden apuntar al mismo vestuario ni aunque
alguien inserte a mano.

### \* : \*  —  `Cancha` ofrece muchos `Servicio`, un `Servicio` está en muchas `Cancha`

```csharp
// Cancha
public List<Servicio> Servicios { get; private set; } = new List<Servicio>();

// Servicio
public List<Cancha> Canchas { get; private set; } = new List<Cancha>();
```

Una colección de cada lado. EF Core (desde la versión 5) crea la tabla intermedia sola:
`CanchaServicio`, con columnas `CanchasId` y `ServiciosId` (nombre de la navegación + `Id`), PK
compuesta y cascade en las dos FK. El diagrama la llama `CANCHA_SERVICIO` con `IdCancha` /
`IdServicio`; los nombres difieren pero la estructura es la misma.

**No existe una clase `CanchaServicio` en el código.** Agregar un servicio es `cancha.Servicios.Add(servicio)`;
EF traduce eso a un `INSERT` en la tabla intermedia.

### Todo lo que EF dedujo sin que se lo digamos

| Qué | De dónde |
|---|---|
| Nombres de tabla `Canchas`, `Reservas`, `Vestuarios`, `Servicios` | Del nombre de cada `DbSet`. |
| Clave primaria | La propiedad se llama `Id`. |
| `uniqueidentifier`, `int`, `bit`, `datetime2`, `decimal(18,2)`, `nvarchar(max)` | Del tipo CLR de cada propiedad. |
| `NOT NULL` vs `NULL` | `Guid` vs `Guid?`, `string` vs `string?`. |
| `Recaudacion`, `ReservasActivas`, `Fin` **no** son columnas | Son propiedades de solo lectura sin campo de respaldo: EF no puede escribirlas, así que las ignora. |
| FK `Reservas.CanchaId` + `ON DELETE CASCADE` + índice | Colección + propiedad `CanchaId` no nullable. |
| FK `Canchas.VestuarioId` + **índice único filtrado** | Referencias de ambos lados + `VestuarioId` nullable. |
| Tabla `CanchaServicio` con PK compuesta y doble cascade | Colecciones de ambos lados. |

### El DDL que sale de esto

```sql
CREATE TABLE [Servicios] (
    [Id] uniqueidentifier NOT NULL,
    [Nombre] nvarchar(max) NOT NULL,
    [Descripcion] nvarchar(max) NOT NULL,
    [Costo] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_Servicios] PRIMARY KEY ([Id])
);

CREATE TABLE [Vestuarios] (
    [Id] uniqueidentifier NOT NULL,
    [Disponible] bit NOT NULL,
    [Duchas] int NOT NULL,
    [Capacidad] int NOT NULL,
    CONSTRAINT [PK_Vestuarios] PRIMARY KEY ([Id])
);

CREATE TABLE [Canchas] (
    [Id] uniqueidentifier NOT NULL,
    [Deporte] nvarchar(max) NOT NULL,
    [TipoPiso] nvarchar(max) NOT NULL,
    [JugadoresMax] int NOT NULL,
    [PrecioPorHora] decimal(18,2) NOT NULL,
    [VestuarioId] uniqueidentifier NULL,
    CONSTRAINT [PK_Canchas] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Canchas_Vestuarios_VestuarioId] FOREIGN KEY ([VestuarioId]) REFERENCES [Vestuarios] ([Id])
);

CREATE TABLE [CanchaServicio] (
    [CanchasId] uniqueidentifier NOT NULL,
    [ServiciosId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_CanchaServicio] PRIMARY KEY ([CanchasId], [ServiciosId]),
    CONSTRAINT [FK_CanchaServicio_Canchas_CanchasId] FOREIGN KEY ([CanchasId]) REFERENCES [Canchas] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_CanchaServicio_Servicios_ServiciosId] FOREIGN KEY ([ServiciosId]) REFERENCES [Servicios] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Reservas] (
    [Id] uniqueidentifier NOT NULL,
    [CanchaId] uniqueidentifier NOT NULL,
    [Cliente] nvarchar(max) NOT NULL,
    [Inicio] datetime2 NOT NULL,
    [Horas] int NOT NULL,
    [Importe] decimal(18,2) NOT NULL,
    [Cancelada] bit NOT NULL,
    CONSTRAINT [PK_Reservas] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Reservas_Canchas_CanchaId] FOREIGN KEY ([CanchaId]) REFERENCES [Canchas] ([Id]) ON DELETE CASCADE
);

CREATE UNIQUE INDEX [IX_Canchas_VestuarioId] ON [Canchas] ([VestuarioId]) WHERE [VestuarioId] IS NOT NULL;
CREATE INDEX [IX_CanchaServicio_ServiciosId] ON [CanchaServicio] ([ServiciosId]);
CREATE INDEX [IX_Reservas_CanchaId] ON [Reservas] ([CanchaId]);
```

---

## 5. Por qué la reserva se agrega al contexto explícitamente

`ReservaService.Crear` tiene una línea que las otras operaciones no tienen:

```csharp
var reserva = cancha.Reservar(request.Cliente, request.Inicio, request.Horas);

repositorioCanchas.AgregarReserva(reserva);   // context.Reservas.Add(reserva)
repositorioCanchas.GuardarCambios();
```

`cancha.Reservar()` ya hace `Reservas.Add(reserva)` sobre una cancha trackeada. Uno esperaría que
EF detecte la reserva nueva solo, como detecta el `INSERT` en `CanchaServicio` cuando se hace
`Servicios.Add(servicio)`. No es así, y la razón es una convención:

- Para EF, una clave `Guid` **la genera él** (`ValueGeneratedOnAdd`), salvo que se le diga lo contrario.
- Cuando EF *descubre* una entidad nueva a través de una navegación, en vez de por un `Add`
  explícito, mira la clave para decidir su estado. Si la clave está vacía, la marca `Added`. Si
  **ya tiene valor**, asume que la entidad existe en la base y la marca `Modified`.
- Nuestro constructor hace `Id = Guid.NewGuid()`. La clave nunca está vacía.

Resultado sin el `AgregarReserva`: EF manda `UPDATE Reservas ... WHERE Id = @id`, no encuentra la
fila, y tira `DbUpdateConcurrencyException: expected to affect 1 row(s), but actually affected 0 row(s)`.
Un error que no dice nada sobre la causa real.

El `Add` explícito le saca la duda: una entidad que entra por `Add` es `Added`, tenga o no clave.

**La alternativa** es declarar en el modelo que la clave la genera el dominio, con un atributo sobre
el `Id` (`[DatabaseGenerated(DatabaseGeneratedOption.None)]`) o con Fluent API
(`ValueGeneratedNever()`). Con eso, EF trataría como `Added` toda entidad nueva que encuentre, sin
`Add` explícito. Se eligió el `Add` para mantener las entidades sin ninguna anotación.

Es la única entidad del proyecto que se crea "colgando" de otra. `Cancha`, `Vestuario` y `Servicio`
entran siempre por `Add`, y el servicio que se agrega a una cancha ya existe en la base.

> **Punto de clase:** la convención asume que las claves las genera la base. Si el constructor las
> genera, hay que compensarlo: o se lo decís al modelo, o hacés el `Add` explícito.

---

## 6. Cambios en las entidades del Domain

| Cambio | Por qué |
|---|---|
| `Id = Guid.NewGuid()` en el constructor | La entidad tiene identidad desde que nace, sin esperar a la base. |
| Propiedades `{ get; private set; }` | EF necesita escribirlas al materializar una fila. `private` mantiene cerrado el acceso desde afuera. |
| Constructor privado **vacío** | EF lo usa para crear instancias al leer, y después llena las propiedades una por una. No tiene que hacer nada. Es privado para que el resto del código siga obligado a pasar por el constructor con validaciones. |
| `string` inicializados con `= string.Empty` en la declaración | Con nullable reference types activado, un `string` no nullable que ningún constructor asigna dispara el warning CS8618. Inicializarlo en la propiedad deja el constructor vacío y sin warning. |
| `Reservas` y `Servicios` son propiedades **públicas** (`List<T> { get; private set; }`) | EF Core no descubre por convención navegaciones que son solo un campo privado. Para que la relación exista sin `OnModelCreating`, tiene que ser una propiedad pública. |
| `Cancha.AsignarVestuario`, `QuitarVestuario`, `AgregarServicio`, `QuitarServicio` | Las relaciones se modifican a través de métodos del agregado, no tocando las colecciones desde afuera. |

---

## 7. Repositorios: ahora el `Include` es obligatorio

```csharp
public Cancha? ObtenerPorId(Guid id) =>
    context.Canchas
        .Include(c => c.Reservas)
        .Include(c => c.Vestuario)
        .Include(c => c.Servicios)
        .FirstOrDefault(c => c.Id == id);
```

EF no carga una relación si no se lo pedís. Sin los `Include`, `cancha.Reservas` vendría vacío,
`cancha.Vestuario` sería `null` y `cancha.Servicios` estaría vacío, aunque la base tenga datos.

En la versión anterior esto se resolvía con `AutoInclude()` en `OnModelCreating`. Al sacar el
`OnModelCreating`, la carga vuelve a ser explícita en cada consulta. Es el precio de no configurar.

`RepositorioVestuarios` incluye `v.Cancha` (para saber si el vestuario ya está asignado).
`RepositorioServicios` no incluye nada: para listar o borrar un servicio no hacen falta sus canchas.

**Los tres repositorios comparten el mismo `DbContext`** (los tres lo reciben por DI, y es Scoped).
Por eso `CanchaService.AsignarVestuario` carga el vestuario con `repositorioVestuarios`, lo asigna a
la cancha y guarda con `repositorioCanchas.GuardarCambios()`: es la misma unidad de trabajo.

---

## 8. DTOs de respuesta: por qué los controllers ya no devuelven entidades

Con `Cancha.Vestuario` ↔ `Vestuario.Cancha` y `Cancha.Servicios` ↔ `Servicio.Canchas`, las
entidades forman **ciclos**. Si el controller devuelve una `Cancha`, `System.Text.Json` entra en
`Cancha → Vestuario → Cancha → Vestuario → ...` y explota con
`JsonException: A possible object cycle was detected`.

La solución correcta es devolver DTOs: `CanchaResponse`, `ReservaResponse`, `VestuarioResponse`,
`ServicioResponse`. Son `record` con un método estático `Desde(entidad)` que hace el mapeo:

```csharp
public record CanchaResponse(Guid Id, string Deporte, /* ... */ VestuarioResponse? Vestuario, IReadOnlyList<ServicioResponse> Servicios)
{
    public static CanchaResponse Desde(Cancha cancha) => new(
        cancha.Id, cancha.Deporte, /* ... */
        cancha.Vestuario is null ? null : VestuarioResponse.Desde(cancha.Vestuario),
        cancha.Servicios.Select(ServicioResponse.Desde).ToList());
}
```

`CanchaResponse` incluye el vestuario y los servicios; `VestuarioResponse` solo el `CanchaId`.
Cada DTO decide hasta dónde llega, y el ciclo se corta.

Beneficio extra: el contrato HTTP queda desacoplado del modelo de persistencia. Se puede cambiar
la entidad sin romper a los clientes de la API.

---

## 9. Endpoints

| Relación | Método | Ruta | Qué hace |
|---|---|---|---|
| — | `POST` | `/api/cancha` | Crea una cancha. |
| — | `GET` | `/api/cancha`, `/api/cancha/{id}` | Lista / detalle (con vestuario y servicios). |
| — | `PATCH` | `/api/cancha/{id}/precio` | Actualiza el precio. |
| — | `DELETE` | `/api/cancha/{id}` | Borra (409 si tiene reservas activas). |
| **1:\*** | `POST` | `/api/cancha/{canchaId}/reservas` | Reserva (409 si se solapa). |
| **1:\*** | `GET` | `/api/cancha/{canchaId}/reservas[/{id}]` | Historial / detalle. |
| **1:\*** | `DELETE` | `/api/cancha/{canchaId}/reservas/{id}` | Cancela. |
| **1:1** | `PUT` | `/api/cancha/{id}/vestuario/{vestuarioId}` | Asigna (409 si el vestuario ya es de otra cancha). |
| **1:1** | `DELETE` | `/api/cancha/{id}/vestuario` | Quita el vestuario. |
| **\*:\*** | `POST` | `/api/cancha/{id}/servicios/{servicioId}` | Agrega (409 si ya lo tiene). |
| **\*:\*** | `DELETE` | `/api/cancha/{id}/servicios/{servicioId}` | Quita. |
| — | `POST` `GET` `DELETE` | `/api/vestuario[/{id}]` | CRUD de vestuarios. |
| — | `POST` `GET` `DELETE` | `/api/servicio[/{id}]` | CRUD de servicios. |

Los ids de las rutas son Guids: `GET /api/cancha/3f2a9c14-...`.

---

## 10. `Program.cs`

```csharp
builder.Services.AddDbContext<GestionComplejoDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<IRepositorioCanchas, RepositorioCanchas>();
builder.Services.AddScoped<IRepositorioVestuarios, RepositorioVestuarios>();
builder.Services.AddScoped<IRepositorioServicios, RepositorioServicios>();

builder.Services.AddScoped<ICanchaService, CanchaService>();
builder.Services.AddScoped<IReservaService, ReservaService>();
builder.Services.AddScoped<IVestuarioService, VestuarioService>();
builder.Services.AddScoped<IServicioService, ServicioService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
```

**Swagger.** Paquete `Swashbuckle.AspNetCore` en Presentation. `AddSwaggerGen` genera la
especificación OpenAPI a partir de los controllers; `UseSwagger` la sirve en
`/swagger/v1/swagger.json` y `UseSwaggerUI` monta la interfaz en `/swagger`. Solo en Development.
`launchSettings.json` abre el navegador en `/swagger` al hacer `dotnet run` o F5.

`AddDbContext` registra el contexto como **Scoped** (una instancia por request). Todo lo que
dependa de él tiene que ser Scoped o Transient. Un `Singleton` que dependa de un Scoped es una
*captive dependency* y el contenedor lo rechaza al arrancar.

---

## 11. Comandos para migrar

### Preparar la herramienta

```bash
dotnet tool update --global dotnet-ef
dotnet ef --version
```

### Si ya existía una base con el modelo anterior

La carpeta `Migrations` se borró, pero la base `GestionComplejo2C` puede seguir existiendo con la
tabla `__EFMigrationsHistory` y las tablas viejas. Con el modelo nuevo y una migración inicial nueva,
`database update` va a intentar crear `Canchas` y va a fallar porque ya existe. Borrala primero:

```bash
dotnet ef database drop --force \
  --project GestionComplejo2C.Infrastructure \
  --startup-project GestionComplejo2C.Presentation
```

### Crear la migración

```bash
dotnet ef migrations add InicialSqlServer \
  --project GestionComplejo2C.Infrastructure \
  --startup-project GestionComplejo2C.Presentation \
  --output-dir Persistence/Migrations
```

Revisá el archivo generado: tiene que crear las cinco tablas del DDL de arriba (las cuatro entidades
más `CanchaServicio`).

### Aplicarla

```bash
dotnet ef database update \
  --project GestionComplejo2C.Infrastructure \
  --startup-project GestionComplejo2C.Presentation
```

### Equivalente en Package Manager Console (Visual Studio)

```powershell
Add-Migration InicialSqlServer -Project GestionComplejo2C.Infrastructure -StartupProject GestionComplejo2C.Presentation -OutputDir Persistence\Migrations
Update-Database -Project GestionComplejo2C.Infrastructure -StartupProject GestionComplejo2C.Presentation
```

### Otros comandos útiles

```bash
# Deshacer la ÚLTIMA migración (solo si todavía no se aplicó)
dotnet ef migrations remove --project ... --startup-project ...

# Volver la base a una migración anterior / a cero
dotnet ef database update NombreMigracion --project ... --startup-project ...
dotnet ef database update 0 --project ... --startup-project ...

# Script SQL para producción o para el DBA
dotnet ef migrations script --idempotent --project ... --startup-project ... -o migracion.sql

# Ver migraciones y cuáles están aplicadas
dotnet ef migrations list --project ... --startup-project ...
```

### Flujo cuando cambia el modelo

1. Modificás una entidad.
2. `dotnet ef migrations add NombreDescriptivo ...`
3. Leés el archivo generado.
4. `dotnet ef database update ...`

Nunca edites una migración ya aplicada en otro entorno: generá una nueva.

---

## 12. Cosas a saber

**Probar el circuito en orden.** Levantá la API con `dotnet run --project GestionComplejo2C.Presentation`
(o F5); abre Swagger en `https://localhost:7003/swagger`. Para ver las tres relaciones funcionando:

```
POST /api/cancha                                   -> guardá el id
POST /api/vestuario                                -> guardá el id
POST /api/servicio                                 -> guardá el id
PUT  /api/cancha/{cancha}/vestuario/{vestuario}    (1:1)
POST /api/cancha/{cancha}/servicios/{servicio}     (*:*)
POST /api/cancha/{cancha}/reservas                 (1:*)
GET  /api/cancha/{cancha}                          -> viene todo junto
```

Y en SQL: `SELECT * FROM CanchaServicio` muestra la fila de la relación \*:\*.

**Qué pasa al borrar, según la relación.**

| Borrás | Pasa |
|---|---|
| Una cancha | Se borran sus reservas y sus filas en `CanchaServicio` (cascade). El vestuario queda. |
| Un vestuario asignado | La relación es opcional, así que EF pone `VestuarioId = NULL` en la cancha antes de borrar (`ClientSetNull`). Funciona porque `RepositorioVestuarios` carga la cancha con `Include`; si no la cargara, SQL Server rechazaría el `DELETE` por la FK. |
| Un servicio | Se borran sus filas en `CanchaServicio` (cascade). Las canchas quedan. |

**Dos warnings esperables al arrancar.**

```
warn: No store type was specified for the decimal property 'PrecioPorHora' on entity type 'Cancha'.
warn: No store type was specified for the decimal property 'Importe' on entity type 'Reserva'.
warn: No store type was specified for the decimal property 'Costo' on entity type 'Servicio'.
```

No rompen nada: SQL Server usa `decimal(18,2)` por defecto. EF avisa porque un importe con más
de dos decimales se truncaría en silencio. Si querés que desaparezcan sin tocar `OnModelCreating`,
es un atributo por propiedad, de `System.ComponentModel.DataAnnotations.Schema` (parte de .NET,
no de EF):

```csharp
[Column(TypeName = "decimal(18,2)")]
public decimal PrecioPorHora { get; private set; }
```

**Para ver el SQL que genera EF.** Ya está activado en `appsettings.Development.json`
(`"Microsoft.EntityFrameworkCore.Database.Command": "Information"`). Cada consulta aparece en
la consola: es la mejor forma de mostrar en clase el `INSERT` en `CanchaServicio` o el `UPDATE`
que pone `VestuarioId`.

**Si algo falla.**

| Síntoma | Casi siempre es |
|---|---|
| *Unable to create a DbContext* | Falta `--startup-project`, o `appsettings.json` tiene un error de sintaxis. |
| *There is already an object named 'Canchas'* | La base vieja sigue ahí. `database drop --force` y de nuevo. |
| *DbUpdateConcurrencyException ... affected 0 row(s)* al crear algo | Una entidad nueva con `Guid` seteado entró por navegación sin `Add` explícito. Ver sección 5. |
| *A possible object cycle was detected* | Un controller está devolviendo una entidad en vez de un DTO. |
| `vestuario: null` o `servicios: []` con datos en la base | Falta un `Include` en la consulta del repositorio. |
| *Cannot consume scoped service ... from singleton* | Quedó un `AddSingleton` apuntando a algo que depende del `DbContext`. |

**Verificar que anduvo.**

```sql
USE GestionComplejo2C;
SELECT * FROM __EFMigrationsHistory;
SELECT * FROM Canchas;
SELECT * FROM CanchaServicio;
```

---

## 13. Limitaciones conocidas / próximos pasos

- **Todo es síncrono.** Lo idiomático sobre una API web es `ToListAsync`, `FirstOrDefaultAsync`,
  `SaveChangesAsync`. Implica volver `async` interfaces, servicios y controllers.
- **`ObtenerTodas` carga todo.** Tres `Include` por cancha, sin paginar. Con volumen real hace
  falta paginar y cargar las relaciones solo cuando se necesitan.
- **Texto sin longitud.** `nvarchar(max)` no se puede indexar. `[MaxLength(60)]` sobre la
  propiedad lo arregla sin tocar `OnModelCreating`.
- **Herencia `Usuario` → `Cliente` / `Administrador`** del diagrama quedó afuera. Es otro tema
  (TPH / TPT) y merece su propia clase.
- **Enums.** El diagrama tiene `DeporteEnum` y `TipoPisoEnum`; el código usa `string`. EF mapea
  enums a `int` por convención, así que el cambio sería solo en el Domain.
- **Condición de carrera en las reservas.** `EstaLibre()` evalúa en memoria. Dos requests
  simultáneos pueden solaparse. Se resuelve con `RowVersion` o una constraint en la base.
