# ClinicaMedica

Web API en .NET 8 para gestionar **profesionales de una clínica médica**, con Clean Architecture, CQRS (MediatR), patrón `Result` (solo en Application), EF Core (SQLite) y Swagger.

## Estructura

- `Domain`: entidad `Profesional`, enum `TipoProfesional`, `IProfesionalRepository` (sin dependencias).
- `Application`: `Result`, comandos/consultas, handlers, DTOs.
- `Infrastructure`: `AppDbContext` (EF Core) y repositorio.
- `Presentation`: controllers, `GlobalExceptionHandler`, `Program.cs`.
- `Tests`: pruebas unitarias del handler (xUnit + Moq).

## Ejecutar

```bash
dotnet restore
dotnet build
dotnet test
dotnet run --project src/ClinicaMedica.Presentation
```

Swagger: `https://localhost:7080/swagger`

## Endpoints

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/profesionales` | Lista los profesionales **activos** |
| GET | `/api/profesionales/{id}` | Obtiene un profesional (activo o dado de baja) |
| POST | `/api/profesionales` | Crea un profesional |
| PUT | `/api/profesionales/{id}` | Modifica un profesional activo |
| DELETE | `/api/profesionales/{id}` | Baja lógica (`Activo = false`, `FechaBaja`) |

> Si ya habías ejecutado la API antes, borrá `clinica.db` (se recrea al iniciar) porque se agregó la columna `FechaBaja`.
