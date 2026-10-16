# Asincronismo de punta a punta (`async` / `await`)

Guía de por qué la API es asíncrona **en todas las capas**, qué cambió en el código y qué
pasaría si no lo fuera.

| Capa | Antes | Ahora |
|---|---|---|
| Presentation (controllers) | `ActionResult<T> GetById(Guid id)` | `async Task<ActionResult<T>> GetById(Guid id, CancellationToken ct)` |
| Application (services) | `CourtResponse? GetById(Guid id)` | `Task<CourtResponse?> GetByIdAsync(Guid id, CancellationToken ct)` |
| Domain (interfaces) | `Court? GetById(Guid id)` | `Task<Court?> GetByIdAsync(Guid id, CancellationToken ct)` |
| Infrastructure (repositorios) | `.ToList()`, `.FirstOrDefault()`, `SaveChanges()` | `.ToListAsync()`, `.FirstOrDefaultAsync()`, `SaveChangesAsync()` |
| Infrastructure (API del clima) | ya era `async`, pero sin cancelación | recibe y propaga el `CancellationToken` |

---

## 1. El problema: los hilos son un recurso limitado

ASP.NET Core atiende cada request con un **hilo del *thread pool***. El pool tiene una
cantidad acotada de hilos y crea nuevos **lentamente** (del orden de uno o dos por segundo
cuando se queda corto).

Casi todo lo que hace esta API es **esperar**:

- esperar a que SQL Server devuelva las canchas,
- esperar a que SQL Server confirme el `INSERT` de una reserva,
- esperar a que Open-Meteo responda si llueve (puede tardar hasta **10 segundos**, que es el
  `Timeout` configurado en `Program.cs`).

Durante esas esperas la CPU no hace nada útil. La pregunta es: **¿qué hace el hilo mientras
tanto?**

### Versión sincrónica: el hilo se queda bloqueado

```csharp
public Court? GetById(Guid id) =>
    context.Courts.FirstOrDefault(c => c.Id == id);   // el hilo queda "dormido" esperando a SQL Server
```

El hilo queda **retenido sin trabajar** hasta que llega la respuesta. No puede atender a
nadie más.

### Versión asíncrona: el hilo se libera

```csharp
public Task<Court?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
    context.Courts.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
```

Cuando el código llega a un `await` sobre una operación de E/S (red, disco, base de datos):

1. Se registra "cuando llegue la respuesta, seguí desde acá".
2. El hilo **vuelve al pool** y atiende otros requests.
3. Cuando la base (o la API externa) responde, *cualquier* hilo libre retoma el método
   justo después del `await`.

> **Analogía:** un mozo (hilo) que toma el pedido, lo deja en la cocina y sigue atendiendo
> otras mesas, en vez de quedarse parado al lado de la cocina hasta que el plato esté listo.
> La cocina (SQL Server / Open-Meteo) no cocina más rápido, pero el mismo mozo atiende
> muchas más mesas.

---

## 2. ¿Qué pasaría si no lo tuviéramos?

### 2.1. Con poca carga: nada visible

Con un solo usuario probando desde Swagger, la versión sincrónica y la asíncrona tardan
**lo mismo**. `async` **no hace más rápido un request individual**; incluso agrega un costo
mínimo (una máquina de estados que genera el compilador).

### 2.2. Con carga: *thread pool starvation*

Ejemplo concreto con este proyecto. Es sábado a la mañana y 200 clientes intentan reservar
cancha al mismo tiempo. Cada `POST /api/court/{courtId}/bookings` consulta Open-Meteo, que
hoy responde lento (≈ 5 s):

| | Sincrónico | Asincrónico |
|---|---|---|
| Hilos ocupados durante la espera | 200 (todos bloqueados) | ≈ 0 (solo mientras hay CPU real) |
| El pool alcanza | No: crea hilos de a 1–2 por segundo | Sí |
| Un `GET /api/court` que llega en ese momento | Hace **cola** detrás de las reservas aunque la base responda en 5 ms | Se atiende enseguida |
| Síntoma | Latencias de decenas de segundos, timeouts, errores 503, CPU **baja** | Latencia normal |

Ese cuadro —**la API "colgada" con la CPU casi en cero**— es la firma típica del
*thread pool starvation*. El servidor no está trabajando: está **esperando** con todos los
hilos tomados. Agregar más CPU o más memoria no lo arregla.

Además, cada hilo bloqueado reserva memoria para su pila (≈ 1 MB), así que escalar
"agregando hilos" también es caro.

### 2.3. ¿Y si solo una parte es asíncrona? (*sync-over-async*)

Antes de este cambio, `BookingService.CreateAsync` era `async` pero llamaba a
`userRepository.GetCustomerById(...)` y `courtRepository.SaveChanges()` en forma sincrónica.
Funcionaba, pero esas esperas a la base seguían bloqueando el hilo: se perdía buena parte
del beneficio.

El caso peor es el inverso, "forzar" un método async desde código sincrónico:

```csharp
// NO HACER
var isRaining = weatherService.IsRainingAsync(date).Result;
var court     = courtRepository.GetByIdAsync(id).GetAwaiter().GetResult();
```

Eso **bloquea un hilo para esperar a una tarea que necesita otro hilo para terminar**:
consume el doble de recursos, empeora el *starvation* y en otros entornos (WinForms, WPF,
ASP.NET clásico) produce **deadlocks**. Por eso la regla es **"async all the way"**: si
una capa de abajo es asíncrona, todas las de arriba también lo son, hasta el controller.

---

## 3. `CancellationToken`: dejar de trabajar para nadie

Todos los métodos asíncronos reciben un `CancellationToken`, que se origina en el
controller:

```csharp
[HttpGet("{id}")]
public async Task<ActionResult<CourtResponse>> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
```

ASP.NET Core completa ese parámetro solo (no hace falta `[FromXxx]`; no aparece en Swagger)
con `HttpContext.RequestAborted`, que se **cancela si el cliente corta la conexión**
(cerró la pestaña, el front-end canceló el fetch, se cortó la red).

El token viaja hacia abajo:

```
Controller ──▶ Service ──▶ Repository ──▶ EF Core ──▶ SQL Server
                       └─▶ OpenMeteoWeatherService ──▶ HttpClient ──▶ Open-Meteo
```

Si el cliente se va, EF Core cancela la consulta en SQL Server y `HttpClient` aborta la
llamada a Open-Meteo, en lugar de terminar un trabajo cuyo resultado nadie va a leer.

En las interfaces se declara con `= default` para que pasarlo sea opcional (por ejemplo, en
un test o en un *seed* donde no hay request).

---

## 4. Qué quedó sincrónico y por qué

`async` sirve para **E/S** (esperar algo externo). No todo método tiene que serlo:

| Método | Por qué sigue siendo sincrónico |
|---|---|
| `Add`, `AddBooking`, `Remove` del repositorio | Solo marcan la entidad en el *change tracker* de EF, en memoria. No tocan la base: eso pasa en `SaveChangesAsync`. EF tiene `AddAsync`, pero la documentación indica usarlo solo con generadores de claves especiales (HiLo); no es nuestro caso. EF no tiene `RemoveAsync`. |
| `IPasswordHasher.Verify` (BCrypt) | Es trabajo de **CPU** (calcular un hash). No hay espera que liberar; envolverlo en `Task.Run` solo movería el trabajo a otro hilo del mismo pool. |
| `ITokenService.GenerateToken` (JWT) | Igual: arma y firma un string en memoria. |
| Entidades de dominio (`Court.Book`, `CancelBooking`, ...) | Reglas de negocio puras, en memoria. El dominio no sabe de E/S. |
| `.Select(...).ToList()` en los servicios | Recorren una lista que **ya está en memoria** (mapeo a DTO). El `ToListAsync` que importa es el del repositorio, que es el que ejecuta la consulta SQL. |

---

## 5. Convenciones aplicadas

- **Sufijo `Async`** en servicios, repositorios y servicios externos (`GetByIdAsync`,
  `SaveChangesAsync`, `IsRainingAsync`): avisa a quien llama que tiene que hacer `await`.
- **Las acciones de los controllers NO llevan el sufijo** (`GetById`, no `GetByIdAsync`).
  ASP.NET Core recorta "Async" del nombre de la acción al armar las rutas, así que
  `CreatedAtAction(nameof(GetByIdAsync), ...)` fallaría en runtime con
  *"No route matches the supplied values"*. Además, el nombre de una acción describe el
  endpoint, no la implementación.
- **Se devuelve `Task<T>`, nunca `async void`**: un `async void` no se puede esperar y sus
  excepciones tiran abajo el proceso.
- **Cuando el método solo reenvía**, se devuelve la `Task` directamente sin
  `async`/`await` (ej. `GetByIdAsync` y `SaveChangesAsync` en los repositorios). Cuando hace
  falta convertir el resultado —`List<T>` a `IReadOnlyList<T>`— se usa `async` + `await`.
- **No se lanzan consultas en paralelo sobre el mismo `DbContext`** (por ejemplo con
  `Task.WhenAll`). El `DbContext` no es *thread-safe*: dentro de un request las consultas
  se hacen una detrás de otra, cada una con su `await`.

---

## 6. Resumen

- La API pasa la mayor parte del tiempo **esperando** a SQL Server y a Open-Meteo.
- Con código sincrónico, cada espera **retiene un hilo**; bajo carga se agotan y la API
  se cuelga con la CPU ociosa (*thread pool starvation*).
- Con `async`/`await`, el hilo **vuelve al pool** durante la espera y el mismo servidor
  atiende muchos más requests concurrentes.
- Tiene que ser **de punta a punta**: una sola capa sincrónica en el medio bloquea igual,
  y forzar `.Result` / `.Wait()` es todavía peor.
- El `CancellationToken` evita seguir trabajando cuando el cliente ya se fue.
- `async` **no** acelera un request aislado ni sirve para trabajo de CPU: es para
  **escalar** cuando hay E/S.
