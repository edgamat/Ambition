# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Project Ambition is a .NET 9.0 example application demonstrating OpenTelemetry distributed tracing. It consists of multiple services communicating via RabbitMQ/MassTransit with SQL Server persistence.

## Build & Test Commands

```bash
make check      # Full CI: build → test → security → format
make build      # dotnet build
make test       # dotnet test --no-build (requires build first)
make security   # Check for vulnerable NuGet packages
make format     # dotnet format --verify-no-changes --no-restore
```

Run a single test project:
```bash
dotnet test test/UnitTests.Ambition.Api/UnitTests.Ambition.Api.csproj
dotnet test test/UnitTests.Ambition.Domain/UnitTests.Ambition.Domain.csproj
```

Run a single test by name:
```bash
dotnet test --filter "FullyQualifiedName~TestMethodName"
```

## Architecture

**Layered/Clean Architecture with event-driven messaging:**

- **Ambition.Domain** — Core business logic, models (`Customer`, `Product`, `MaintenancePlan`), interfaces (`IMaintenancePlanRepository`, `IMaintenancePlanService`, `IEventPublisher`), and OpenTelemetry `DiagnosticsConfig` with custom `ActivitySource`. No framework dependencies.
- **Ambition.Infrastructure** — EF Core `AmbitionDbContext` (SQL Server), repository implementations, event publishing via Edgamat.Messaging, and EF migrations.
- **Ambition.Api** — ASP.NET Core Minimal API. Endpoints for maintenance plans (GET/POST). Configures OpenTelemetry (OTLP exporters), Serilog logging, health checks, and Swagger.
- **Ambition.Accounting** — Background worker service (hosted service). Consumes `MaintenancePlanCreated` events via MassTransit/RabbitMQ. Has its own `AccountingDbContext` for ledger data.
- **Mercury.Email** — Stub email service (Minimal API, .NET 8.0). POST `/send` endpoint with simulated delay.

## Key Patterns

- **Minimal APIs** — no controllers; endpoints mapped via extension methods (e.g., `MapMaintenancePlanEndpoints`)
- **Repository pattern** — `IMaintenancePlanRepository` abstracts data access
- **Event-driven** — domain events published via `IEventPublisher`, consumed by Accounting worker through MassTransit
- **OpenTelemetry instrumentation** — custom `ActivitySource` ("Ambition") in `DiagnosticsConfig`, span enrichment via `ActivityExtensions`

## Local Infrastructure (Docker)

Docker Compose files in `docker/` for: SQL Server, RabbitMQ (with management UI), Seq (centralized logging), .NET Aspire Dashboard.

## Code Style

Enforced via `.editorconfig` and `dotnet format`:
- 4-space indentation, CRLF line endings
- Explicit types preferred over `var`
- Private fields: `_camelCase`, private static fields: `s_camelCase`
- Interfaces: `I` prefix, PascalCase throughout for public members
- Separate and sort import directive groups
- Allman-style braces (open brace on new line)

## Test Framework

xUnit with `coverlet.collector` for code coverage. Test projects mirror source structure under `test/`.
