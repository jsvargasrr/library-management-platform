# Arquitectura

## Objetivo

Este repositorio implementa una plataforma de gestión de biblioteca (Autores y Libros) como **monorepo fullstack** con:

- Backend: ASP.NET Core Web API + EF Core + SQL Server
- Frontend: Angular (standalone + routing + reactive forms)
- Testing: xUnit + Moq + FluentAssertions (backend) y pruebas unitarias básicas (frontend)

El foco está en **mantenibilidad**, **separación de responsabilidades** y **reglas de negocio explícitas**.

## Backend (Clean Architecture)

Estructura:

```
backend/src/
  Library.Domain
  Library.Application
  Library.Infrastructure
  Library.Api
```

### `Library.Domain`

- **Entidades**: `Author`, `Book`
- **Excepciones de dominio**: mensajes de negocio obligatorios y exactos
  - `AuthorNotRegisteredException` → `"El autor no está registrado"`
  - `MaxBooksReachedException` → `"No es posible registrar el libro, se alcanzó el máximo permitido."`

### `Library.Application`

- **Casos de uso / servicios**: `AuthorService`, `BookService`
- **DTOs**: request/response para endpoints
- **Validaciones**: FluentValidation a nivel de request DTO
- **Abstracciones**: repositorios `IAuthorRepository`, `IBookRepository`
- **Excepciones de aplicación**:
  - `ValidationException` (400) con lista de errores
  - `NotFoundException` (404)
  - `ConflictException` (409)
- **Configuración de reglas**: `LibraryConstraintsOptions` con `MaxBooksAllowed` desde `appsettings.json`

### `Library.Infrastructure`

- EF Core:
  - `LibraryDbContext`
  - Configuraciones Fluent API (`AuthorConfiguration`, `BookConfiguration`)
  - Migraciones en `Persistence/Migrations`
- Repositorios concretos:
  - `AuthorRepository`
  - `BookRepository`
- `DateTimeProvider` para testabilidad (validación de fechas)

### `Library.Api`

- **Controladores**: `AuthorsController`, `BooksController`
- **Middleware global de errores**: `ExceptionHandlingMiddleware`
- **Contrato de respuesta**:
  - `ApiResponse<T>` para respuestas exitosas
  - `ApiErrorResponse` para errores
- Swagger/OpenAPI + health check endpoint `/health`

## Frontend (Angular)

Estructura (feature-first):

```
frontend/library-app/src/app/
  core/
    api/
    notifications/
  layout/
    shell/
  features/
    authors/
    books/
```

- **Features**: Authors y Books con pages de listado y formulario (create/edit)
- **Servicios HTTP** tipados (`AuthorsApi`, `BooksApi`)
- **Interceptor** de errores (`apiErrorInterceptor`) para mostrar mensajes del backend
- **UX**: layout consistente, navegación simple, estados de loading, confirmaciones de delete, validaciones visuales

## Base de datos

- SQL Server para desarrollo local:
  - En macOS: vía Docker (`database/docker-compose.yml`)
  - En Windows: puedes reemplazar el connection string por LocalDB si lo prefieres
- Migraciones EF Core + script idempotente:
  - `database/scripts/001-init-idempotent.sql`

