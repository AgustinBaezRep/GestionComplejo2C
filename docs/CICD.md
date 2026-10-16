# CI/CD: deploy automático a Azure (App Service + Azure SQL)

Guía para publicar la API en **Azure App Service** y la base en **Azure SQL Database**, y para que
cada push (o merge de un PR) a `master` haga solo el deploy y la actualización de la base con
**GitHub Actions**.

| Pieza | Servicio de Azure | Plan gratuito |
|---|---|---|
| Contenedor lógico de todo | Resource Group | Gratis siempre |
| API (.NET 10) | App Service (Linux) | Plan **F1 Free** |
| Base de datos | Azure SQL Database | **Free offer** (100.000 vCore-segundos + 32 GB por mes) |
| Pipeline | GitHub Actions | Gratis en repos públicos (2.000 min/mes en privados) |

```
 git push master ──► GitHub Actions ──┬─► dotnet ef database update ──► Azure SQL Database
                                      └─► azure/webapps-deploy ───────► Azure App Service
```

> **Cuenta:** con **Azure for Students** (azure.microsoft.com/free/students) se entra con el mail de
> la facultad, sin tarjeta. Con los planes de esta guía no se consume crédito.

---

## 1. Resource Group

Un **Resource Group** es una carpeta lógica que agrupa recursos de Azure relacionados. No cuesta
nada y sirve para tres cosas:

- **Organizar:** todo lo del proyecto (App Service, plan, servidor SQL, base) queda junto.
- **Permisos:** se le puede dar acceso a un compañero sobre el grupo entero.
- **Borrar todo de una:** eliminar el grupo elimina todo lo que contiene.

Cada recurso pertenece a **un solo** grupo, y el grupo tiene una **región** (dónde se guarda su
metadata). Conviene crear los recursos en la misma región que el grupo.

### Crearlo desde el portal

1. Entrar a [portal.azure.com](https://portal.azure.com) → buscar **Resource groups** → **Create**.
2. **Subscription:** la de estudiante.
3. **Resource group:** `rg-gestioncomplejo2c`.
4. **Region:** `East US 2` (o la que use el resto de la guía; ver nota de regiones en §3).
5. **Review + create** → **Create**.

---

## 2. App Service (la API)

Un App Service necesita dos recursos:

- **App Service Plan:** la "máquina" (CPU, RAM, precio). El plan **F1** es gratuito: 1 GB RAM,
  60 minutos de CPU por día, sin *Always On* (la app se duerme sin tráfico y el primer request
  tarda unos segundos).
- **Web App:** la aplicación en sí, que corre dentro del plan.

### Crearlo

1. Buscar **App Services** → **Create** → **Web App**.
2. Pestaña **Basics**:
   - **Resource group:** `rg-gestioncomplejo2c`.
   - **Name:** `gestioncomplejo2c-api` (es global, si está tomado agregale algo). La URL queda
     `https://<name>.azurewebsites.net` (o con un sufijo regional, el portal la muestra).
   - **Publish:** `Code`.
   - **Runtime stack:** `.NET 10 (LTS)`.
   - **Operating System:** `Linux`.
   - **Region:** la misma del grupo.
   - **Pricing plan:** **Create new** → elegir **Free F1**.
3. Pestaña **Deployment**: dejar *Continuous deployment* en **Disable** (el pipeline lo hacemos
   nosotros en el §5).
4. **Review + create** → **Create**.

> Si el nombre del App Service no es `gestioncomplejo2c-api`, cambialo en
> `.github/workflows/deploy.yml` → `env.AZURE_WEBAPP_NAME`.

### Habilitar el publish profile

El pipeline se autentica con el **publish profile** (un XML con credenciales de deploy). Azure lo
trae deshabilitado por defecto en apps nuevas:

1. En la Web App → **Settings → Configuration → General settings**.
2. **SCM Basic Auth Publishing Credentials:** `On` → **Save**.

---

## 3. Azure SQL Database (la base)

Azure SQL tiene dos niveles:

- **SQL Server (lógico):** el "servidor" con su nombre `xxx.database.windows.net`, el usuario admin
  y el **firewall**. No cobra nada por sí solo.
- **SQL Database:** la base en sí, que vive dentro del servidor.

### Crearla con la oferta gratuita

1. Buscar **Azure SQL** → **Create** → **SQL databases / Single database** → **Create**.
2. Arriba aparece el banner **"Want to try Azure SQL Database for free?"** → **Apply offer**.
   Si no aparece, la suscripción ya usó sus bases gratis o la región no la ofrece.
3. Pestaña **Basics**:
   - **Resource group:** `rg-gestioncomplejo2c`.
   - **Database name:** `GestionComplejo2C`.
   - **Server:** **Create new**:
     - **Server name:** `sql-gestioncomplejo2c` (global, agregale algo si está tomado).
     - **Location:** la misma región.
     - **Authentication method:** **Use SQL authentication**.
     - **Server admin login / Password:** anotalos, van en la connection string.
   - **Behavior when free limit reached:** **Auto-pause the database until next month**
     (así nunca cobra; si se agota el cupo la base queda pausada hasta el mes siguiente).
4. Pestaña **Networking**:
   - **Connectivity method:** **Public endpoint**.
   - **Allow Azure services and resources to access this server:** **Yes**. Esto deja entrar al
     App Service *y* a los runners de GitHub Actions (que corren en Azure).
   - **Add current client IP address:** **Yes**, para poder conectarte desde tu PC con SSMS/Azure
     Data Studio.
5. **Review + create** → **Create**.

> **Regiones:** si la región elegida no ofrece la base gratuita o el plan F1 (mensaje de "no
> disponible" o cuota), probá con otra (`East US 2`, `West US 2`, `Brazil South`...). El
> Resource Group puede quedar en una región y los recursos en otra.

### Obtener la connection string

En la base → **Settings → Connection strings → ADO.NET (SQL authentication)**. Queda así
(reemplazá `{your_password}`):

```
Server=tcp:sql-gestioncomplejo2c.database.windows.net,1433;Initial Catalog=GestionComplejo2C;Persist Security Info=False;User ID=<admin>;Password=<password>;MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=False;Connection Timeout=60;
```

Diferencias con la local (`appsettings.json`):

| Local | Azure | Por qué |
|---|---|---|
| `Trusted_Connection=True` | `User ID` + `Password` | En Azure no hay autenticación de Windows. |
| `TrustServerCertificate=True` | `Encrypt=True;TrustServerCertificate=False` | Azure tiene un certificado válido; se valida. |
| (timeout por defecto 15 s) | `Connection Timeout=60` | La base gratuita es *serverless* y se pausa sola; despertarla tarda hasta ~1 minuto. |

---

## 4. Configurar la app en Azure (secretos de producción)

El `appsettings.json` que se publica apunta a `localhost` y tiene la clave JWT de desarrollo. En
Azure **no se edita el archivo**: se sobreescriben valores con variables de entorno. ASP.NET Core
las lee automáticamente y pisan al `appsettings.json` (el `:` de las secciones se escribe `__`).

En la Web App → **Settings → Environment variables**:

**Pestaña App settings:**

| Name | Value |
|---|---|
| `Jwt__Key` | Una clave larga y aleatoria (mínimo 32 caracteres), distinta a la de desarrollo. |

**Pestaña Connection strings:**

| Name | Value | Type |
|---|---|---|
| `SportsComplexDb` | La connection string del §3. | `SQLAzure` |

**Apply** → **Confirm**. La app se reinicia.

> El nombre `SportsComplexDb` tiene que ser exactamente el que lee `Program.cs`
> (`GetConnectionString("SportsComplexDb")`).

> **Swagger:** en `Program.cs` solo se habilita si el entorno es `Development`. En Azure el entorno
> es `Production`, así que `/swagger` devuelve 404. Si lo querés ver para una demo, agregá el app
> setting `ASPNETCORE_ENVIRONMENT` = `Development` (y sacalo después).

---

## 5. El pipeline (GitHub Actions)

### Cuándo corre

Está en `.github/workflows/deploy.yml`:

```yaml
on:
  push:
    branches: [ master ]   # push directo o merge de un PR -> deploy
  pull_request:
    branches: [ master ]   # PR abierto -> solo compila
  workflow_dispatch:       # botón "Run workflow" en la pestaña Actions
```

Aprobar un PR no cambia `master`; lo que dispara el deploy es el **merge**, porque un merge es un
push a `master`. Por eso alcanza con el trigger `push`. El trigger `pull_request` es la parte de
**CI**: mientras el PR está abierto, GitHub compila el código y muestra ✅ o ❌ en el PR, sin tocar
Azure.

### Qué hace

| # | Paso | PR | Push a master |
|---|---|:---:|:---:|
| 1 | Checkout del código | ✅ | ✅ |
| 2 | Instalar .NET 10 | ✅ | ✅ |
| 3 | `dotnet restore` / `dotnet build` (Release) | ✅ | ✅ |
| 4 | `dotnet publish` a `./publish` | ✅ | ✅ |
| 5 | Instalar `dotnet-ef` | — | ✅ |
| 6 | `dotnet ef database update --connection ...` contra Azure SQL | — | ✅ |
| 7 | Subir `./publish` al App Service | — | ✅ |

Detalles:

- **Las migraciones van antes del deploy.** Así, cuando arranca la versión nueva del código, la
  base ya tiene las columnas/tablas que espera. Si la migración falla, el pipeline se corta y en
  Azure queda la versión anterior funcionando.
- **`database update` es incremental:** EF mira la tabla `__EFMigrationsHistory` y aplica solo las
  migraciones que faltan. Si no hay nuevas, no hace nada. La primera vez crea todas las tablas.
- **`--connection`** reemplaza la connection string del `appsettings.json` solo para ese comando,
  así no hace falta tocar archivos. Es el mismo comando del `docs/PERSISTENCIA.md` con un
  parámetro más.
- **`--no-build`** reutiliza lo compilado en el paso 3, en vez de compilar de nuevo.

### Secretos del repositorio

El pipeline necesita dos datos sensibles que **nunca** se commitean. En GitHub → repo →
**Settings → Secrets and variables → Actions → New repository secret**:

| Secret | De dónde sale |
|---|---|
| `AZURE_WEBAPP_PUBLISH_PROFILE` | Web App → **Overview → Download publish profile**. Abrir el archivo `.PublishSettings` y pegar **todo** su contenido. |
| `DB_CONNECTION_STRING` | La connection string del §3, con el password real. |

En el YAML se usan como `${{ secrets.NOMBRE }}`. GitHub los oculta en los logs (`***`).

> El publish profile descargado es una credencial: no lo dejes en el repo (el `.gitignore` ya
> ignora `*.publishsettings`) y borralo de Descargas después de cargarlo.

### Primer deploy

1. Commitear y pushear `.github/workflows/deploy.yml` a `master` (o mergear el PR que lo agrega).
2. GitHub → pestaña **Actions** → ver el run **CI/CD - Azure App Service**. Cada paso se puede
   expandir para ver el log.
3. Si está todo verde, probar la API:

```bash
curl -X POST https://<tu-app>.azurewebsites.net/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"x@x.com","password":"x"}'
```

Un `401 Usuario o password incorrectos.` significa que la API está levantada **y** pudo consultar
la base (la tabla de usuarios existe). Un `500` suele ser la connection string mal cargada en el §4.

### Recomendado: proteger `master`

Para que todo cambio pase por PR y el CI tenga que dar verde antes del merge: repo → **Settings →
Branches → Add branch ruleset** (o *branch protection rule*) sobre `master`:

- **Require a pull request before merging** (y opcionalmente *Require approvals: 1*).
- **Require status checks to pass** → elegir `build-and-deploy`.

---

## 6. Problemas comunes

| Síntoma | Causa / solución |
|---|---|
| `Client with IP address 'x.x.x.x' is not allowed to access the server` en el paso de migraciones | El firewall del servidor SQL. Revisar que *Allow Azure services* esté en **Yes** (servidor SQL → **Security → Networking**). Como último recurso, agregar esa IP como regla. |
| Timeout en el paso de migraciones o en el primer request | La base estaba pausada (serverless). Volver a correr el job (**Re-run jobs**); con `Connection Timeout=60` normalmente alcanza. |
| `Deployment Failed, Error: Failed to get app runtime OS` / `401 Unauthorized` en el deploy | Publish profile incorrecto o *SCM Basic Auth* apagado (§2). Volver a descargarlo y actualizar el secret. |
| La app responde `HTTP Error 500.30` | La app no arrancó. Web App → **Monitoring → Log stream**. Suele ser la connection string o `Jwt__Key` faltante. |
| `Database 'GestionComplejo2C' on server ... is not currently available` | Se agotó el cupo gratuito del mes y la base quedó pausada. Se reactiva el mes siguiente. |
| La app tarda en responder después de un rato sin uso | Normal en F1: no tiene *Always On*, la app se duerme. |

---

## 7. Borrar todo

Al terminar la cursada: **Resource groups → `rg-gestioncomplejo2c` → Delete resource group**.
Elimina el App Service, el plan, el servidor SQL y la base de una sola vez.
