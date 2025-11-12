<!-- Copilot instructions tailored to the capstone multi-service .NET workspace -->
# Notes for AI coding agents — capstone services (concise)

Overview
- This repository contains multiple small ASP.NET Core services under `src/services/` (authentication, catalog, crm, cart, ...).
- Each service follows a 3-project pattern: `Models`, `DataAccess`, `Services` (example: `cart/Models/Cart.Models.csproj`, `cart/DataAccess/Cart.DataAccess.csproj`, `cart/Services/Cart.Services.csproj`).

Key patterns and conventions
- Namespace and project naming: use PascalCase. Example projects use `Crm.Models`, `Crm.DataAccess`, `Crm.Services` (watch for collisions where a type name equals a namespace, e.g. `Cart.Models.Cart`).
- Repository pattern: each `DataAccess` has `IRepository.cs`, `MongoRepository.cs` and a domain repository (e.g. `CustomerRepository.cs`, `CartRepository.cs`). Expect MongoDB usage and dependency injection of `IMongoClient`.
- Services project contains Program.cs (minimal API / WebApplication), controllers under `Services/Controllers`, and DI wiring. JWT auth is configured in `Services/Program.cs` and often uses an env var or inline secret (search for `YourSuperSecretKey`).
- Swagger/OpenAPI is enabled with `AddSwaggerGen` / `AddOpenApi` and `app.UseSwagger(); app.UseSwaggerUI(...)`.

Build and developer workflows
- Main workspace build (from services folder):
  - cd src/services
  - dotnet restore
  - dotnet build capstone.sln -c Release
- Per-service build: run `dotnet build <ServicePath>/Services/<Project>.csproj -c Release` or `dotnet run --project <ServicePath>/Services/<Project>.csproj` for local runtime.
- Many edits to project names require updating solution files (`*.sln`) and `ProjectReference` entries inside `.csproj` files. Use `dotnet sln <sln> add` to add renamed projects.

Integration points & environment
- MongoDB: connection string is read from the environment variable `MONGO_CONNECTION_STRING` (see e.g. `catalog/Services/Program.cs`).
- JWT: token validation parameters are configured directly in `Program.cs`; change secrets in `appsettings.json` or environment variables rather than leaving hard-coded values.
- Docker: individual services have `Dockerfile`s in service root; use `docker build` or `docker-compose` at the repo level if provided.

Project-specific pitfalls that matter to AI edits
- Avoid renaming or moving .csproj files without updating corresponding `.sln` entries and all `ProjectReference` paths. Many recent changes renamed `Models.csproj` → `Cart.Models.csproj` etc.
- Be explicit with types when a namespace equals a type name (e.g., use `Cart.Models.Cart` and `Cart.Models.CartItem` in code to avoid ambiguity).
- Do not assume test projects exist — add tests in a new `tests/` folder if needed; there are no existing test harnesses.

Useful file references (examples to open before edits)
- `src/services/*/Services/Program.cs` — DI, JWT, Swagger wiring.
- `src/services/*/DataAccess/MongoRepository.cs` and `IRepository.cs` — data access patterns.
- `src/services/*/Models/*.cs` — domain models (Item, Customer, Cart, UserAccount).
- `src/services/*/Services/Controllers/*Controller.cs` — API surface.

How to be productive quickly
- Read a service's `Program.cs` and its `DataAccess` and `Models` projects before changing behavior. That shows DI, external deps, and env-vars.
- When adding a new project, update the service's `.sln` (or `capstone.sln`) and adjust `ProjectReference` paths.
- For build failures, run `dotnet build capstone.sln -v minimal` and paste the error block (not whole logs) — it contains the failing file and line numbers.

If you modify CI, builds, or secrets
- Keep secrets out of code. Prefer `appsettings.Development.json` for local defaults and environment variables for CI.

Feedback
- After applying edits, run a build of the affected service and paste the failing compiler error(s) if any. I will iterate only on the failing files to minimize repo churn.

---
_End of concise instructions — update this file when service-level layouts or naming conventions change._
