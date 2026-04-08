# Library Management Platform (Fullstack Monorepo)

Entrega fullstack enfocada en calidad “enterprise”: **Clean Architecture**, validaciones robustas, manejo global de errores, Swagger cuidado, EF Core migrations, pruebas unitarias con valor y un frontend Angular simple/profesional.

## Quickstart (ejecución local)

Requisitos:
- Docker Desktop
- .NET SDK 9.x+
- Node 20+ / npm

```bash
# 1) Levantar SQL Server (Docker)
./scripts/dev-up-db.sh

# 2) Aplicar migraciones EF Core
dotnet restore backend/Library.sln
./scripts/dev-migrate-db.sh

# 3) Correr API (Swagger en /swagger)
./scripts/dev-run-backend.sh

# 4) Correr Frontend
./scripts/dev-run-frontend.sh
```

## Stack

- **Backend**: .NET (SDK local 9.x; código migrable a .NET 10), ASP.NET Core Web API, EF Core, Swagger
- **DB**: SQL Server (Docker en macOS/Linux; LocalDB en Windows si aplica), migrations EF Core
- **Frontend**: Angular 21 (standalone, routing, reactive forms)
- **Testing**: xUnit + Moq + FluentAssertions (backend), pruebas unitarias básicas (frontend)

## Estructura del monorepo

```
library-management-platform/
  backend/
    src/
      Library.Api/
      Library.Application/
      Library.Domain/
      Library.Infrastructure/
    tests/
      Library.UnitTests/
  frontend/
    library-app/
  database/
    docker-compose.yml
    scripts/
  docs/
    architecture.md
  scripts/
```

## Reglas de negocio (enunciado)

- **Campos obligatorios** validados (request DTO + validaciones de negocio).
- **Integridad**: FK `Books.AuthorId -> Authors.Id`, índice único para `Authors.Email`.
- **Máximo de libros** configurable (`LibraryConstraints:MaxBooksAllowed` en `backend/src/Library.Api/appsettings.json`).
  - Si se supera: lanza excepción con mensaje exacto:
    - `"No es posible registrar el libro, se alcanzó el máximo permitido."`
- **Autor inexistente al crear/editar libro**:
  - `"El autor no está registrado"`

## Backend (cómo ejecutar)

### 1) Base de datos (Docker)

Requisito: tener Docker Desktop instalado.

```bash
./scripts/dev-up-db.sh
```

Esto expone SQL Server en `localhost:14333` (usuario `sa`, password `Your_password123`).

### 2) Restaurar, migrar y correr API

```bash
dotnet restore backend/Library.sln
./scripts/dev-migrate-db.sh
./scripts/dev-run-backend.sh
```

- Swagger: `http://localhost:5045/swagger`
- Health: `http://localhost:5045/health`

### Connection string

El backend usa `ConnectionStrings:LibraryDb` en `backend/src/Library.Api/appsettings.json`.

En Windows (opcional LocalDB), ejemplo:

```json
{
  "ConnectionStrings": {
    "LibraryDb": "Server=(localdb)\\\\MSSQLLocalDB;Database=LibraryDb;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

## Frontend (cómo ejecutar)

```bash
cd frontend/library-app
npm install
npm start
```

App: `http://localhost:4200`

> Si cambias el puerto del backend, ajusta `apiBaseUrl` en `frontend/library-app/src/environments/environment.ts`.

## Endpoints principales

- **Autores**
  - `GET /api/authors`
  - `GET /api/authors/{id}`
  - `POST /api/authors`
  - `PUT /api/authors/{id}`
  - `DELETE /api/authors/{id}` (bloquea si tiene libros)
- **Libros**
  - `GET /api/books`
  - `GET /api/books/{id}`
  - `POST /api/books`
  - `PUT /api/books/{id}`
  - `DELETE /api/books/{id}`

## Manejo global de errores (backend)

Formato consistente:

```json
{
  "success": false,
  "message": "mensaje claro",
  "errors": [],
  "traceId": "..."
}
```

Mapeo HTTP:

- 400: validación y excepciones de dominio (incluye mensajes exactos del enunciado)
- 404: no encontrado
- 409: conflicto (por ejemplo, email duplicado; o eliminar autor con libros)
- 500: inesperado

## Testing

### Backend

```bash
cd backend
dotnet test Library.sln -c Release
```

Incluye pruebas reales para:

- creación exitosa de autor
- creación exitosa de libro
- error si autor no existe (mensaje exacto)
- error si se supera máximo de libros (mensaje exacto)

### Frontend

```bash
cd frontend/library-app
npm test -- --watch=false
```

## Documentación de arquitectura

Ver `docs/architecture.md`.

## Mejoras futuras (si se evoluciona a “prod”)

- Paginación/filtrado en listados
- Soft-delete (si el dominio lo requiere)
- Observabilidad (OpenTelemetry), logging estructurado
- Autenticación/autorización si aplica
- Tests de integración API con Testcontainers

