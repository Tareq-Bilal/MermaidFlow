# Prompt: Create a .NET Clean Architecture Project

> **Usage:** Replace every `{ProjectName}` placeholder with your actual project name (e.g., `GymManagement`, `OrderSystem`, `BudgetTracker`). Replace `{Entity}` with your primary domain entity (e.g., `Subscription`, `Order`, `Product`). Replace `{EntityPlural}` with the plural form (e.g., `Subscriptions`, `Orders`, `Products`).

## Overview

Create a **.NET 7 Web API** project following **Clean Architecture** principles. The solution uses **CQRS** with **MediatR**, **ErrorOr** for result handling, and a clear separation into 5 layers. This structure is applicable to **any domain**.

---

## Solution Structure

The solution is named `{ProjectName}.sln` and contains **5 projects** all under a `src/` folder. There is also a `requests/` folder at the root for `.http` test files and a `global.json` pinning the SDK version.

```
Root/
├── global.json
├── {ProjectName}.sln
├── requests/
│   └── {EntityPlural}/
│       ├── Create{Entity}.http
│       └── Get{Entity}.http
└── src/
    ├── {ProjectName}.Api/              # Presentation layer (ASP.NET Core Web API)
    ├── {ProjectName}.Application/      # Application layer (use cases, CQRS handlers)
    ├── {ProjectName}.Contracts/        # Shared DTOs/request-response contracts
    ├── {ProjectName}.Domain/           # Domain layer (entities, value objects, enums)
    └── {ProjectName}.Infrastructure/   # Infrastructure layer (persistence, external services)
```

---

## Layer Dependency Rules (Critical)

The dependency flow follows Clean Architecture — **inner layers never depend on outer layers**:

```
Api  →  Application  →  Domain
 │           ↑
 │      Infrastructure
 │           ↑
 ├──→  Contracts (shared DTOs, no business logic)
 └──→  Infrastructure
```

| Project                          | References                             | NuGet Packages                                                                |
| -------------------------------- | -------------------------------------- | ----------------------------------------------------------------------------- |
| **{ProjectName}.Domain**         | _(none)_                               | _(none)_                                                                      |
| **{ProjectName}.Contracts**      | _(none)_                               | _(none)_                                                                      |
| **{ProjectName}.Application**    | Domain                                 | `MediatR`, `ErrorOr`, `Microsoft.Extensions.DependencyInjection.Abstractions` |
| **{ProjectName}.Infrastructure** | Application                            | _(none initially — add EF Core, etc. as needed)_                              |
| **{ProjectName}.Api**            | Application, Infrastructure, Contracts | `Microsoft.AspNetCore.OpenApi`, `Swashbuckle.AspNetCore`                      |

### Key rules

- **Domain** has zero dependencies — no NuGet packages, no project references.
- **Application** depends only on **Domain**. It defines interfaces (e.g., `I{EntityPlural}Repository`, `IUnitOfWork`) that **Infrastructure** implements.
- **Infrastructure** depends on **Application** (to implement its interfaces). It never depends on Api or Contracts.
- **Api** is the composition root. It wires up DI for all layers and references Application, Infrastructure, and Contracts.
- **Contracts** is a standalone project with no dependencies — contains only request/response DTOs and enums shared between Api and consumers.

---

## global.json

Pin the .NET SDK version:

```json
{
  "sdk": {
    "rollForward": "latestMinor",
    "version": "7.0.400"
  }
}
```

---

## Project Details

### 1. {ProjectName}.Domain

**Purpose:** Core business entities. Zero dependencies.

**Folder structure:**

```
{ProjectName}.Domain/
├── {ProjectName}.Domain.csproj
└── {EntityPlural}/
    └── {Entity}.cs
```

**csproj:**

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net7.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
</Project>
```

**{Entity}.cs:**

```csharp
namespace {ProjectName}.Domain.{EntityPlural};

public class {Entity}
{
    public Guid Id { get; set; }
    // Add domain properties here
}
```

---

### 2. {ProjectName}.Contracts

**Purpose:** Shared DTOs (request/response records) and enums. No business logic. No dependencies.

**Folder structure:**

```
{ProjectName}.Contracts/
├── {ProjectName}.Contracts.csproj
└── {EntityPlural}/
    ├── Create{Entity}Request.cs
    ├── {Entity}Response.cs
    └── {Entity}Type.cs          (optional — only if your entity has a type/category enum)
```

**csproj:**

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net7.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
</Project>
```

**Create{Entity}Request.cs:**

```csharp
namespace {ProjectName}.Contracts.{EntityPlural};

public record Create{Entity}Request(
    // Add request properties matching your domain needs
    string Name,
    Guid OwnerId);
```

**{Entity}Response.cs:**

```csharp
namespace {ProjectName}.Contracts.{EntityPlural};

public record {Entity}Response(
    Guid Id,
    // Add response properties matching what consumers need
    string Name);
```

**{Entity}Type.cs (optional):**

```csharp
using System.Text.Json.Serialization;

namespace {ProjectName}.Contracts.{EntityPlural};

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum {Entity}Type
{
    // Define enum values matching your domain
    TypeA,
    TypeB,
    TypeC
}
```

---

### 3. {ProjectName}.Application

**Purpose:** Application use cases implemented via CQRS (Commands/Queries) with MediatR. Defines interfaces for infrastructure concerns (Repository, Unit of Work). Uses `ErrorOr` for result types instead of exceptions.

**Folder structure:**

```
{ProjectName}.Application/
├── {ProjectName}.Application.csproj
├── DependencyInjection.cs
├── Common/
│   └── Interfaces/
│       ├── I{EntityPlural}Repository.cs
│       └── IUnitOfWork.cs
└── {EntityPlural}/
    └── Commands/
        └── Create{Entity}/
            ├── Create{Entity}Command.cs
            └── Create{Entity}CommandHandler.cs
```

**Folder convention:** Features are organized by domain aggregate (e.g., `{EntityPlural}/`) and then by CQRS type (`Commands/`, `Queries/`). Each command/query has its own folder containing the request record and its handler.

**csproj:**

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <ItemGroup>
    <ProjectReference Include="..\\{ProjectName}.Domain\\{ProjectName}.Domain.csproj" />
  </ItemGroup>
  <ItemGroup>
    <PackageReference Include="ErrorOr" Version="1.2.1" />
    <PackageReference Include="MediatR" Version="12.1.1" />
    <PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" Version="7.0.0" />
  </ItemGroup>
  <PropertyGroup>
    <TargetFramework>net7.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
</Project>
```

**DependencyInjection.cs:**

```csharp
using Microsoft.Extensions.DependencyInjection;

namespace {ProjectName}.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(options =>
        {
            options.RegisterServicesFromAssemblyContaining(typeof(DependencyInjection));
        });

        return services;
    }
}
```

**I{EntityPlural}Repository.cs:**

```csharp
using {ProjectName}.Domain.{EntityPlural};

namespace {ProjectName}.Application.Common.Interfaces;

public interface I{EntityPlural}Repository
{
    Task Add{Entity}Async({Entity} entity);
    Task<{Entity}?> GetByIdAsync(Guid id);
}
```

**IUnitOfWork.cs:**

```csharp
namespace {ProjectName}.Application.Common.Interfaces;

public interface IUnitOfWork
{
    Task CommitChangesAsync();
}
```

**Create{Entity}Command.cs:**

```csharp
using ErrorOr;
using {ProjectName}.Domain.{EntityPlural};
using MediatR;

namespace {ProjectName}.Application.{EntityPlural}.Commands.Create{Entity};

public record Create{Entity}Command(
    // Add command properties matching what the use case needs
    string Name,
    Guid OwnerId) : IRequest<ErrorOr<{Entity}>>;
```

**Create{Entity}CommandHandler.cs:**

```csharp
using ErrorOr;
using {ProjectName}.Application.Common.Interfaces;
using {ProjectName}.Domain.{EntityPlural};
using MediatR;

namespace {ProjectName}.Application.{EntityPlural}.Commands.Create{Entity};

public class Create{Entity}CommandHandler : IRequestHandler<Create{Entity}Command, ErrorOr<{Entity}>>
{
    private readonly I{EntityPlural}Repository _{entityPlural}Repository;

    public Create{Entity}CommandHandler(I{EntityPlural}Repository {entityPlural}Repository)
    {
        _{entityPlural}Repository = {entityPlural}Repository;
    }

    public async Task<ErrorOr<{Entity}>> Handle(Create{Entity}Command request, CancellationToken cancellationToken)
    {
        // 1. Create the domain entity
        var entity = new {Entity}
        {
            Id = Guid.NewGuid(),
            // Map command properties to entity
        };

        // 2. Persist to database
        await _{entityPlural}Repository.Add{Entity}Async(entity);

        // 3. Return result
        return entity;
    }
}
```

---

### 4. {ProjectName}.Infrastructure

**Purpose:** Implements interfaces defined in Application (repositories, unit of work, external services). This is where EF Core, database contexts, and third-party integrations live.

**Folder structure:**

```
{ProjectName}.Infrastructure/
├── {ProjectName}.Infrastructure.csproj
└── DependencyInjection.cs
```

**csproj:**

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <ItemGroup>
    <ProjectReference Include="..\\{ProjectName}.Application\\{ProjectName}.Application.csproj" />
  </ItemGroup>
  <PropertyGroup>
    <TargetFramework>net7.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
</Project>
```

**DependencyInjection.cs:**

```csharp
using Microsoft.Extensions.DependencyInjection;

namespace {ProjectName}.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        return services;
    }
}
```

> **Note:** This is the starting point. As the project evolves, add repository implementations, EF Core `DbContext`, and register them here. For example:
>
> ```csharp
> services.AddScoped<I{EntityPlural}Repository, {EntityPlural}Repository>();
> services.AddScoped<IUnitOfWork, UnitOfWork>();
> ```

---

### 5. {ProjectName}.Api

**Purpose:** Composition root and presentation layer. ASP.NET Core Web API with controllers. Wires up DI from Application and Infrastructure layers.

**Folder structure:**

```
{ProjectName}.Api/
├── {ProjectName}.Api.csproj
├── Program.cs
├── appsettings.json
├── appsettings.Development.json
├── Properties/
│   └── launchSettings.json
└── Controllers/
    └── {EntityPlural}Controller.cs
```

**csproj:**

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net7.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="7.0.10" />
    <PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.0" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\\{ProjectName}.Application\\{ProjectName}.Application.csproj" />
    <ProjectReference Include="..\\{ProjectName}.Contracts\\{ProjectName}.Contracts.csproj" />
    <ProjectReference Include="..\\{ProjectName}.Infrastructure\\{ProjectName}.Infrastructure.csproj" />
  </ItemGroup>
</Project>
```

**Program.cs:**

```csharp
using {ProjectName}.Application;
using {ProjectName}.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services
        .AddApplication()
        .AddInfrastructure();
}

var app = builder.Build();
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
```

**{EntityPlural}Controller.cs:**

```csharp
using {ProjectName}.Application.{EntityPlural}.Commands.Create{Entity};
using {ProjectName}.Contracts.{EntityPlural};
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace {ProjectName}.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class {EntityPlural}Controller : ControllerBase
{
    private readonly ISender _mediator;

    public {EntityPlural}Controller(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create{Entity}(Create{Entity}Request request)
    {
        var command = new Create{Entity}Command(
            // Map request properties to command properties
            request.Name,
            request.OwnerId);

        var result = await _mediator.Send(command);

        return result.MatchFirst(
            entity => Ok(new {Entity}Response(entity.Id, /* map properties */)),
            error => Problem());
    }
}
```

---

## HTTP Test Requests

Place these in a `requests/` folder at the solution root for API testing.

**requests/{EntityPlural}/Create{Entity}.http:**

```http
@host=http://localhost:5209

POST {{host}}/{EntityPlural}
Content-Type: application/json

{
    "Name": "Example",
    "OwnerId": "2150e333-8fdc-42a3-9474-1a3956d46de8"
}
```

**requests/{EntityPlural}/Get{Entity}.http:**

```http
@host=http://localhost:5209
@id=6c3cd790-7503-417a-91ee-9bd5adc9e61b

GET {{host}}/{EntityPlural}/{{id}}
```

---

## Design Patterns & Conventions

| Pattern                  | Implementation                                                                                                                 |
| ------------------------ | ------------------------------------------------------------------------------------------------------------------------------ |
| **CQRS**                 | Commands and Queries separated under `Application/{Feature}/Commands/` and `Application/{Feature}/Queries/`                    |
| **MediatR**              | Commands implement `IRequest<ErrorOr<T>>`. Handlers implement `IRequestHandler<TCommand, ErrorOr<T>>`.                         |
| **ErrorOr**              | All command/query handlers return `ErrorOr<T>` instead of throwing exceptions. Controllers use `.MatchFirst()` to map results. |
| **Repository Pattern**   | Interfaces defined in `Application/Common/Interfaces/`. Implementations in `Infrastructure/`.                                  |
| **Unit of Work**         | `IUnitOfWork` interface in Application. Implementation in Infrastructure (typically wrapping EF Core `SaveChangesAsync`).      |
| **Dependency Injection** | Each layer has a `DependencyInjection.cs` with an `Add{Layer}()` extension method on `IServiceCollection`.                     |
| **DTOs as Records**      | Request/Response types are C# `record` types in the Contracts project.                                                         |
| **Feature Folders**      | Code is organized by feature/aggregate (e.g., `{EntityPlural}/`) rather than by technical concern.                             |

### Naming Conventions

- **Commands:** `{Action}{Entity}Command` → e.g., `CreateOrderCommand`
- **Command Handlers:** `{Action}{Entity}CommandHandler` → e.g., `CreateOrderCommandHandler`
- **Queries:** `{Action}{Entity}Query` → e.g., `GetOrderQuery`
- **Query Handlers:** `{Action}{Entity}QueryHandler` → e.g., `GetOrderQueryHandler`
- **Interfaces:** `I{EntityPlural}Repository` → e.g., `IOrdersRepository`
- **Requests:** `{Action}{Entity}Request` → e.g., `CreateOrderRequest`
- **Responses:** `{Entity}Response` → e.g., `OrderResponse`
- **DI Extensions:** `Add{LayerName}()` → `AddApplication()`, `AddInfrastructure()`
- **Namespaces:** Match folder structure → `{ProjectName}.Application.{EntityPlural}.Commands.Create{Entity}`

---

## Install & Run Instructions

### Prerequisites

- **.NET 7 SDK** (7.0.400 or later) — [Download](https://dotnet.microsoft.com/download/dotnet/7.0)
- A code editor (Visual Studio 2022, VS Code with C# extension, or JetBrains Rider)

### Step 1: Verify .NET SDK

```bash
dotnet --version
```

Ensure the output is `7.0.400` or a later 7.x minor version.

### Step 2: Clone / Create the project

If building from scratch, create the solution and projects using the .NET CLI:

```bash
# Create solution directory
mkdir {ProjectName} && cd {ProjectName}

# Create global.json
dotnet new globaljson --sdk-version 7.0.400 --roll-forward latestMinor

# Create the solution
dotnet new sln -n {ProjectName}

# Create src directory
mkdir src

# Create projects
dotnet new webapi -n {ProjectName}.Api -o src/{ProjectName}.Api
dotnet new classlib -n {ProjectName}.Application -o src/{ProjectName}.Application
dotnet new classlib -n {ProjectName}.Domain -o src/{ProjectName}.Domain
dotnet new classlib -n {ProjectName}.Infrastructure -o src/{ProjectName}.Infrastructure
dotnet new classlib -n {ProjectName}.Contracts -o src/{ProjectName}.Contracts

# Add projects to solution
dotnet sln add src/{ProjectName}.Api
dotnet sln add src/{ProjectName}.Application
dotnet sln add src/{ProjectName}.Domain
dotnet sln add src/{ProjectName}.Infrastructure
dotnet sln add src/{ProjectName}.Contracts

# Set up project references (dependency rules)
dotnet add src/{ProjectName}.Api reference src/{ProjectName}.Application
dotnet add src/{ProjectName}.Api reference src/{ProjectName}.Contracts
dotnet add src/{ProjectName}.Api reference src/{ProjectName}.Infrastructure
dotnet add src/{ProjectName}.Application reference src/{ProjectName}.Domain
dotnet add src/{ProjectName}.Infrastructure reference src/{ProjectName}.Application

# Install NuGet packages
dotnet add src/{ProjectName}.Application package MediatR --version 12.1.1
dotnet add src/{ProjectName}.Application package ErrorOr --version 1.2.1
dotnet add src/{ProjectName}.Application package Microsoft.Extensions.DependencyInjection.Abstractions --version 7.0.0
dotnet add src/{ProjectName}.Api package Microsoft.AspNetCore.OpenApi --version 7.0.10
dotnet add src/{ProjectName}.Api package Swashbuckle.AspNetCore --version 6.5.0
```

### Step 3: Restore dependencies

```bash
dotnet restore {ProjectName}.sln
```

### Step 4: Build the solution

```bash
dotnet build {ProjectName}.sln
```

Ensure zero errors and zero warnings.

### Step 5: Run the API

```bash
dotnet run --project src/{ProjectName}.Api
```

The API will start at:

- **HTTP:** `http://localhost:5209`
- **HTTPS:** `https://localhost:7091`
- **Swagger UI:** `http://localhost:5209/swagger` (in Development mode)

### Step 6: Test the API

Use the `.http` files in the `requests/` folder (VS Code REST Client extension or JetBrains HTTP Client), or use curl:

```bash
curl -X POST http://localhost:5209/{EntityPlural} \
  -H "Content-Type: application/json" \
  -d '{"Name": "Example", "OwnerId": "2150e333-8fdc-42a3-9474-1a3956d46de8"}'
```

Or open `http://localhost:5209/swagger` in your browser to test via the Swagger UI.

---

## Architecture Diagram

```
┌─────────────────────────────────────────────────────────┐
│                   {ProjectName}.Api                     │
│              (Controllers, Program.cs, DI)              │
│         Composition Root — wires everything up          │
└──────────┬──────────────┬───────────────┬───────────────┘
           │              │               │
           ▼              ▼               ▼
┌──────────────────┐ ┌────────────┐ ┌─────────────────────┐
│  {ProjectName}   │ │{ProjectName│ │   {ProjectName}     │
│  .Application    │ │.Contracts  │ │   .Infrastructure   │
│                  │ │            │ │                     │
│  Commands/       │ │ DTOs       │ │  Implements         │
│  Queries/        │ │ Enums      │ │  Application        │
│  Interfaces/     │ │ Records    │ │  interfaces         │
│  (MediatR +      │ └────────────┘ │  (Repos, UoW,       │
│   ErrorOr)       │                │   DbContext, etc.)  │
└────────┬─────────┘                └──────────┬──────────┘
         │                                     │
         │         ┌──────────────────┐        │
         └────────►│  {ProjectName}   │◄───────┘
                   │  .Domain         │  (via Application)
                   │                  │
                   │  Entities        │
                   │  Value Objects   │
                   │  Enums           │
                   │  (Zero deps)     │
                   └──────────────────┘
```

---

## Extending the Project

When adding a **new feature** (e.g., a new entity like `Product`), follow this pattern:

1. **Domain:** Add `Domain/{NewEntityPlural}/{NewEntity}.cs` entity
2. **Contracts:** Add `Contracts/{NewEntityPlural}/Create{NewEntity}Request.cs`, `{NewEntity}Response.cs`
3. **Application:**
   - Add `Application/Common/Interfaces/I{NewEntityPlural}Repository.cs`
   - Add `Application/{NewEntityPlural}/Commands/Create{NewEntity}/Create{NewEntity}Command.cs`
   - Add `Application/{NewEntityPlural}/Commands/Create{NewEntity}/Create{NewEntity}CommandHandler.cs`
   - (Optional) Add `Application/{NewEntityPlural}/Queries/Get{NewEntity}/Get{NewEntity}Query.cs` + handler
4. **Infrastructure:** Implement `I{NewEntityPlural}Repository` in `Infrastructure/{NewEntityPlural}/{NewEntityPlural}Repository.cs` and register in `DependencyInjection.cs`
5. **Api:** Add `Api/Controllers/{NewEntityPlural}Controller.cs`
6. **Requests:** Add `requests/{NewEntityPlural}/Create{NewEntity}.http`
