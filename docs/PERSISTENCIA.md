# Persistencia con EF Core + SQL Server

Guía de los cambios hechos para pasar del repositorio en memoria a SQL Server, y los
comandos necesarios para crear la base y empezar a persistir datos.

El criterio fue **hacer lo mínimo**: apoyarse en las convenciones de EF Core y escribir
configuración solo donde la convención no alcanza.

---

## 1. Paquetes: cuál va en qué capa

| Capa | Paquete | Por qué ahí |
|---|---|---|
| Domain | *ninguno* | El dominio no debe saber que existe una base de datos. Persistence ignorance. |
| Application | *ninguno* | Solo depende de `IRepositorioCanchas`, una abstracción del Domain. |
| Infrastructure | `Microsoft.EntityFrameworkCore.SqlServer` | Es la capa que implementa la persistencia. Arrastra `EntityFrameworkCore` y `EntityFrameworkCore.Relational` como dependencias transitivas. |
| Presentation | `Microsoft.EntityFrameworkCore.Design` | Lo necesita el **startup project** para que `dotnet ef` pueda construir el `DbContext` en tiempo de diseño. No se usa en runtime. |

```bash
dotnet add GestionComplejo2C.Infrastructure package Microsoft.EntityFrameworkCore.SqlServer
dotnet add GestionComplejo2C.Presentation package Microsoft.EntityFrameworkCore.Design
```

> **Punto de clase:** la regla de dependencias de Clean Architecture no cambia. Domain sigue
> sin referencias a nada. Todo lo que huele a SQL vive en Infrastructure.

---

## 2. Connection string

`GestionComplejo2C.Presentation/appsettings.json`:

```json
"ConnectionStrings": {
  "GestionComplejoDb": "Server=localhost;Database=GestionComplejo2C;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True"
}
```

Partes:

- `Server` — instancia de SQL Server.
- `Database` — la base se crea sola al correr `database update`.
- `Trusted_Connection=True` — autenticación integrada de Windows, sin usuario ni password.
- `TrustServerCertificate=True` — el certificado de SQL local es autofirmado. **Solo para desarrollo.**
- `MultipleActiveResultSets=True` — permite tener más de un `DataReader` abierto en la misma conexión.

Variantes según el motor instalado:

| Motor | Server= |
|---|---|
| Instancia default (Developer / Standard) | `localhost` o `.` |
| SQL Server Express | `localhost\\SQLEXPRESS` |
| LocalDB (viene con Visual Studio) | `(localdb)\\MSSQLLocalDB` |
| Docker / SQL auth | `localhost,1433` + `User Id=sa;Password=...` en vez de `Trusted_Connection` |

En JSON el backslash va escapado (`\\`). Si ponés uno solo, el archivo no parsea y la app no arranca.

---

## 3. `GestionComplejoDbContext`

`Infrastructure/Persistence/GestionComplejoDbContext.cs`

```csharp
public class GestionComplejoDbContext : DbContext
{
    public GestionComplejoDbContext(DbContextOptions<GestionComplejoDbContext> options)
        : base(options)
    {
    }

    public DbSet<Cancha> Canchas => Set<Cancha>();

    public DbSet<Reserva> Reservas => Set<Reserva>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Cancha>()
            .HasMany<Reserva>("reservas")
            .WithOne()
            .HasForeignKey(r => r.CanchaId);

        modelBuilder.Entity<Cancha>()
            .Navigation("reservas")
            .AutoInclude();
    }
}
```

- Recibe `DbContextOptions<GestionComplejoDbContext>` por constructor: el proveedor y la
  connection string se inyectan desde `Program.cs`, no se hardcodean acá.
- Expone un `DbSet<T>` por tabla: `Canchas` y `Reservas`.

> **Punto de clase:** el `DbContext` **es** un Unit of Work. Acumula cambios en su
> `ChangeTracker` y recién los manda a la base en `SaveChanges()`, dentro de una transacción.

---

## 4. El mapeo: dos líneas, el resto por convención

No hay clases `IEntityTypeConfiguration<>` ni Data Annotations. EF Core deduce solo:

| Qué deduce | De dónde |
|---|---|
| Nombre de tabla `Canchas` / `Reservas` | Del nombre del `DbSet`. |
| Clave primaria | La propiedad se llama `Id`. |
| `nvarchar(max)`, `int`, `bit`, `datetime2`, `decimal(18,2)` | Del tipo CLR de cada propiedad. |
| `NOT NULL` | Del nullable reference type: `string` sin `?` es requerido. |
| `Recaudacion`, `ReservasActivas`, `Fin` **no** se mapean | Son propiedades de solo lectura sin campo de respaldo: EF no puede escribirlas al materializar, así que las ignora. |
| `ON DELETE CASCADE` + índice sobre `CanchaId` | Es una relación requerida; EF elige cascade e indexa la FK. |
| El `Guid` que trae la entidad no se pisa | EF genera Guids solo cuando el valor es `Guid.Empty`. Como el constructor asigna uno real, lo respeta. |

Lo único que **no** deduce, y por eso está escrito:

1. **La relación contra el campo privado.** `Cancha` guarda sus reservas en
   `private readonly List<Reserva> reservas`, sin propiedad pública, para que nadie pueda
   agregar una reserva salteándose la validación de `Reservar()`. EF Core no descubre
   navegaciones que son solo un campo, así que hay que nombrarlo:

   ```csharp
   modelBuilder.Entity<Cancha>()
       .HasMany<Reserva>("reservas")   // el nombre del campo privado
       .WithOne()                      // Reserva no tiene navegación inversa
       .HasForeignKey(r => r.CanchaId);
   ```

2. **`AutoInclude()`.** EF no carga una relación si no se lo pedís. Sin esto,
   `Recaudacion`, `ReservasActivas` y `VerHistorial()` darían vacío aunque la tabla
   `Reservas` tenga filas. Declararlo una vez en el modelo mantiene el repositorio limpio
   de `Include(...)` repetidos.

> **Punto de clase:** el encapsulamiento del dominio no se rompe para que el ORM funcione.
> EF se adapta al modelo, no al revés.

DDL que produce este modelo:

```sql
CREATE TABLE [Canchas] (
    [Id] uniqueidentifier NOT NULL,
    [Deporte] nvarchar(max) NOT NULL,
    [TipoPiso] nvarchar(max) NOT NULL,
    [JugadoresMax] int NOT NULL,
    [PrecioPorHora] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_Canchas] PRIMARY KEY ([Id])
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
    CONSTRAINT [FK_Reservas_Canchas_CanchaId] FOREIGN KEY ([CanchaId])
        REFERENCES [Canchas] ([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_Reservas_CanchaId] ON [Reservas] ([CanchaId]);
```

---

## 5. Cambios en las entidades del Domain

### `Cancha`

| Antes | Ahora | Por qué |
|---|---|---|
| `private static int siguienteId = 1;` + `Id = siguienteId++` | `Id = Guid.NewGuid()` en el constructor | Un contador estático reinicia en 1 al reiniciar la app y colisiona con más de una instancia corriendo. Con un `Guid` el Id lo genera la entidad, es único siempre y no depende de la base. |
| `public int Id { get; }` | `public Guid Id { get; private set; }` | EF necesita poder escribir la propiedad al materializar una fila. `private set` mantiene el encapsulamiento. |
| propiedades `{ get; }` | `{ get; private set; }` | Idem. |
| — | `private Cancha() { }` | Constructor que usa EF para crear instancias al leer de la base. Es **privado** para que el resto del código siga obligado a pasar por el constructor con validaciones. |

### `Reserva`

- Mismos cambios de setters + constructor privado.
- **Nuevo:** `public Guid CanchaId { get; private set; }` — la clave foránea. EF la completa
  sola cuando la reserva entra en la colección de una `Cancha` trackeada (*relationship fixup*).
- `Id` ya era un `Guid` generado en el constructor: quedó igual.

> **Ventaja de generar el Id en el dominio:** la entidad es válida y tiene identidad desde el
> momento en que la creás, sin esperar al `SaveChanges()`. Con `IDENTITY` el Id vale 0 hasta
> que la base responde, y hay que tener cuidado de guardar antes de armar la URL del
> `CreatedAtAction`.

---

## 6. `IRepositorioCanchas` y `RepositorioCanchas`

Cambios en la interfaz (Domain):

```csharp
Cancha? ObtenerPorId(Guid id);   // antes: int id
void Eliminar(Cancha cancha);    // antes: bool Eliminar(Cancha cancha)
void GuardarCambios();           // nuevo
```

- `Eliminar` ya no devuelve `bool`: con una `List<T>`, `Remove` sabía al instante si el elemento
  estaba. Con EF solo marca la entidad como `Deleted`; el resultado real llega en `SaveChanges()`.
- `GuardarCambios()` es el punto de commit explícito. Con la lista en memoria, mutar el objeto
  ya "persistía". Con EF hay que avisarle cuándo cerrar la unidad de trabajo.

La implementación queda casi igual de corta que la versión en memoria:

```csharp
public void Agregar(Cancha cancha) => context.Canchas.Add(cancha);

public IReadOnlyList<Cancha> ObtenerTodas() => context.Canchas.ToList();

public Cancha? ObtenerPorId(Guid id) => context.Canchas.FirstOrDefault(c => c.Id == id);

public void Eliminar(Cancha cancha) => context.Canchas.Remove(cancha);

public void GuardarCambios() => context.SaveChanges();
```

Un `List<Cancha>` pasó a ser un `DbSet<Cancha>` y los métodos LINQ son los mismos, pero ahora
`FirstOrDefault` se traduce a un `SELECT ... WHERE Id = @id` en vez de recorrer memoria.

`Add` **no** ejecuta el `INSERT`: solo marca la entidad como `Added`. El SQL sale en
`GuardarCambios()`.

---

## 7. Servicios: dónde va el `GuardarCambios()`

`CanchaService` y `ReservaService` llaman a `GuardarCambios()` después de cada operación de escritura:

| Operación | Qué hace EF |
|---|---|
| `CanchaService.Crear` | `Agregar` + `GuardarCambios` → `INSERT`. |
| `CanchaService.ActualizarPrecio` | La entidad viene trackeada; alcanza con `GuardarCambios` → `UPDATE` de la columna que cambió. |
| `CanchaService.Eliminar` | `Eliminar` + `GuardarCambios` → `DELETE` (las reservas caen por cascade). |
| `ReservaService.Crear` | `cancha.Reservar(...)` agrega a la colección de una entidad trackeada → EF la ve como `Added`. |
| `ReservaService.Cancelar` | `cancha.Cancelar(id)` cambia `Cancelada` → `UPDATE`. |

Fijate que en `ActualizarPrecio` y `Cancelar` **no se llama a ningún método del repositorio para
"actualizar"**. El change tracker compara la entidad contra el snapshot que guardó al leerla y
genera el `UPDATE` solo.

---

## 8. `Program.cs`

```csharp
var connectionString = builder.Configuration.GetConnectionString("GestionComplejoDb")
    ?? throw new InvalidOperationException("Falta la connection string...");

builder.Services.AddDbContext<GestionComplejoDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<IRepositorioCanchas, RepositorioCanchas>();  // era AddSingleton
```

Dos cosas importantes:

1. **El `throw` si falta la connection string.** Preferimos fallar en el arranque y no en el
   primer request, con un mensaje claro.

2. **`AddSingleton` → `AddScoped` en el repositorio.** `AddDbContext` registra el contexto como
   **Scoped** (una instancia por request HTTP). Un `Singleton` que depende de un `Scoped` es una
   *captive dependency*: el contenedor lo detecta y tira una excepción al arrancar. Y aunque no
   lo hiciera, un `DbContext` compartido entre requests no es thread-safe y acumularía entidades
   trackeadas para siempre.

> **Punto de clase:** el lifetime del `DbContext` es *uno por unidad de trabajo*. En una API web,
> eso es *uno por request*. Todo lo que dependa de él tiene que ser Scoped o Transient.

---

## 9. Comandos para migrar

### Preparar la herramienta

```bash
# Primera vez
dotnet tool install --global dotnet-ef

# Actualizar (la instalada acá es 10.0.9 y el runtime es 10.0.11)
dotnet tool update --global dotnet-ef

dotnet ef --version
```

### Crear la primera migración

Desde la raíz de la solución:

```bash
dotnet ef migrations add InicialSqlServer \
  --project GestionComplejo2C.Infrastructure \
  --startup-project GestionComplejo2C.Presentation \
  --output-dir Persistence/Migrations
```

- `--project` → dónde vive el `DbContext` y dónde se escriben los archivos de migración.
- `--startup-project` → de dónde salen la configuración (`appsettings.json`) y el registro de DI.
- `--output-dir` → carpeta destino, relativa al `--project`.

Genera tres archivos en `Infrastructure/Persistence/Migrations/`:
`<timestamp>_InicialSqlServer.cs` (`Up`/`Down`), su `.Designer.cs` y el `ModelSnapshot`.
**Revisalos antes de aplicar.**

### Aplicar a la base

```bash
dotnet ef database update \
  --project GestionComplejo2C.Infrastructure \
  --startup-project GestionComplejo2C.Presentation
```

Crea la base `GestionComplejo2C` si no existe, crea las tablas y registra la migración en
`__EFMigrationsHistory`.

### Equivalente en Package Manager Console (Visual Studio)

```powershell
Add-Migration InicialSqlServer -Project GestionComplejo2C.Infrastructure -StartupProject GestionComplejo2C.Presentation -OutputDir Persistence\Migrations
Update-Database -Project GestionComplejo2C.Infrastructure -StartupProject GestionComplejo2C.Presentation
```

### Otros comandos útiles

```bash
# Deshacer la ÚLTIMA migración (solo si todavía no se aplicó a la base)
dotnet ef migrations remove --project GestionComplejo2C.Infrastructure --startup-project GestionComplejo2C.Presentation

# Volver a un estado anterior de la base
dotnet ef database update NombreDeLaMigracionAnterior --project ... --startup-project ...

# Revertir TODAS las migraciones
dotnet ef database update 0 --project ... --startup-project ...

# Generar el script SQL en vez de aplicarlo (lo que se usa en producción / se le pasa al DBA)
dotnet ef migrations script --idempotent --project ... --startup-project ... -o migracion.sql

# Listar migraciones y cuáles están aplicadas
dotnet ef migrations list --project ... --startup-project ...

# Borrar la base entera (desarrollo, cuando querés arrancar de cero)
dotnet ef database drop --force --project ... --startup-project ...
```

### Flujo cuando cambia el modelo

1. Modificás una entidad o el `OnModelCreating`.
2. `dotnet ef migrations add NombreDescriptivo ...`
3. Leés el archivo generado.
4. `dotnet ef database update ...`

Nunca edites una migración ya aplicada en otro entorno: generá una nueva.

---

## 10. Cosas a saber

**Antes de correr los comandos**

- Tiene que haber un SQL Server corriendo y la connection string tiene que apuntar a él.
- `database update` falla si todavía no hay ninguna migración: primero `migrations add`.
- Si `dotnet ef` dice *"Unable to create a DbContext"*, casi siempre es que falta el
  `--startup-project` o que `appsettings.json` tiene un error de sintaxis.

**Dos warnings esperables al construir el modelo**

```
warn: No store type was specified for the decimal property 'PrecioPorHora' on entity type 'Cancha'.
warn: No store type was specified for the decimal property 'Importe' on entity type 'Reserva'.
```

No rompen nada: SQL Server usa `decimal(18,2)` por defecto, que es exactamente lo que queremos
para plata. EF avisa porque un importe con más de 2 decimales se truncaría en silencio. Si querés
que desaparezcan, son dos líneas en `OnModelCreating`:

```csharp
modelBuilder.Entity<Cancha>().Property(c => c.PrecioPorHora).HasPrecision(18, 2);
modelBuilder.Entity<Reserva>().Property(r => r.Importe).HasPrecision(18, 2);
```

**Verificar que anduvo**

```sql
USE GestionComplejo2C;
SELECT * FROM __EFMigrationsHistory;   -- debe listar InicialSqlServer
SELECT * FROM Canchas;
SELECT * FROM Reservas;
```

Y probando la API: `POST /api/cancha` → reiniciás la app → `GET /api/cancha` debe seguir
devolviendo la cancha. Ese es el test de que la persistencia funciona.

Ahora los ids de las rutas son Guids: `GET /api/cancha/3f2a...-...`, no `GET /api/cancha/1`.

**Para ver el SQL que genera EF**

Ya está activado en `appsettings.Development.json`:

```json
"Microsoft.EntityFrameworkCore.Database.Command": "Information"
```

Cada consulta aparece en la consola. Muy útil para mostrar en clase el `INSERT`/`UPDATE` real.

**Credenciales fuera del repo**

En desarrollo, si la connection string lleva usuario y password, sacala del `appsettings.json`:

```bash
dotnet user-secrets init --project GestionComplejo2C.Presentation
dotnet user-secrets set "ConnectionStrings:GestionComplejoDb" "Server=...;User Id=...;Password=..." --project GestionComplejo2C.Presentation
```

En producción va por variable de entorno: `ConnectionStrings__GestionComplejoDb`.

---

## 11. Limitaciones conocidas / próximos pasos

Cosas que quedaron afuera a propósito, pero que conviene nombrar en clase:

- **Todo es síncrono.** Lo idiomático en EF Core sobre una API web es `async`/`await`
  (`ToListAsync`, `FirstOrDefaultAsync`, `SaveChangesAsync`), porque libera el thread mientras
  espera a la base. Implica volver `async` las interfaces, servicios y controllers.
- **`ObtenerTodas` trae todo.** Sin paginación y, por el `AutoInclude`, con todas las reservas
  de todas las canchas. Con volumen real hace falta paginar y volver el include explícito.
- **Las columnas de texto son `nvarchar(max)`.** Funciona, pero no se pueden indexar. Un
  `HasMaxLength(60)` sobre `Deporte` y `TipoPiso` lo arreglaría.
- **Condición de carrera en las reservas.** `Cancha.EstaLibre()` evalúa en memoria. Dos requests
  simultáneos pueden pasar la validación y crear reservas superpuestas. Se resuelve con una
  columna de concurrencia (`RowVersion`) o una constraint en la base.
- **Los controllers devuelven entidades del dominio.** Convendría devolver DTOs de respuesta,
  para no acoplar el contrato HTTP al modelo de persistencia.
- **No hay seed de datos.** Si querés datos iniciales: `modelBuilder.Entity<Cancha>().HasData(...)`
  en `OnModelCreating` (queda dentro de la migración).
