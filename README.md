# MermaidFlow

A .NET backend for creating, managing, and rendering Mermaid diagram documents. Write Markdown with embedded Mermaid diagrams — MermaidFlow handles storage, server-side rendering, and export.

---

## Tech Stack

| Layer          | Technology                              |
| -------------- | --------------------------------------- |
| Framework      | ASP.NET Core 10 (Web API)               |
| Architecture   | Clean Architecture + CQRS (MediatR)     |
| ORM            | Entity Framework Core 9 (SQL Server)    |
| Authentication | JWT Bearer*(planned)*                   |
| Rendering      | PuppeteerSharp / Mermaid CLI*(planned)* |
| Validation     | FluentValidation*(planned)*             |
| API Docs       | Scalar (OpenAPI)                        |

---

## Project Structure

```
src/
├── MermaidFlow.Api/            # Controllers, Program.cs
├── MermaidFlow.Application/    # CQRS commands/queries, interfaces
├── MermaidFlow.Domain/         # Entities (no dependencies)
├── MermaidFlow.Infrastructure/ # EF Core, repositories, services
└── MermaidFlow.Contracts/      # Request/response DTOs
```

---

## API Endpoints

```mermaid
graph LR
    Client(["🖥️ Client"])

    subgraph Auth ["🔐 Auth (planned)"]
        A1["POST /auth/login\nLogin → JWT"]
        A2["POST /auth/refresh\nRefresh token"]
        A3["POST /auth/logout\nRevoke token"]
    end

    subgraph Users ["👤 Users"]
        U1["POST /Users\nCreate user"]
        U2["GET /Users\nList all users"]
        U3["GET /Users/{id}\nGet user"]
        U4["PUT /Users/{id}\nUpdate user"]
        U5["DELETE /Users/{id}\nDelete user"]
    end

    subgraph Documents ["📄 Documents"]
        D1["POST /Documents\nCreate document"]
        D2["GET /Documents/{id}\nGet document"]
        D3["PUT /Documents/{id}\nUpdate document (planned)"]
        D4["DELETE /Documents/{id}\nDelete document (planned)"]
    end

    subgraph Mermaid ["🔷 Mermaid (planned)"]
        M1["POST /mermaid/render\nRender → SVG"]
        M2["POST /mermaid/validate\nValidate syntax"]
        M3["POST /mermaid/export\nExport SVG / PNG"]
    end

    Client --> A1 & A2 & A3
    Client --> U1 & U2 & U3 & U4 & U5
    Client --> D1 & D2 & D3 & D4
    Client --> M1 & M2 & M3

    A1 -- "JWT token" --> U3
    A1 -- "JWT token" --> D1 & D2 & D3 & D4
    A1 -- "JWT token" --> M1 & M2 & M3

    U1 -- "creates" --> U3
    D1 -- "owned by" --> U3
    D2 -- "content fed to" --> M1
    M1 -- "cached SVG" --> D2
```

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server or LocalDB

### Run

```bash
# Apply database migrations
dotnet ef database update --project src/MermaidFlow.Infrastructure --startup-project src/MermaidFlow.Api

# Start the API
dotnet run --project src/MermaidFlow.Api --urls "http://localhost:5209"
```

## Data Models

### `Document`

```
- Id (Guid)
- Title (string, required, max 200)
- Content (string, required)  // Raw markdown
- UserId (Guid, FK)
- CreatedAt (DateTime)
- UpdatedAt (DateTime)
- IsPublic (bool)
- Tags (List<string>)
```

### `User`

```
- Id (Guid)
- Email (string, unique)
- PasswordHash (string)
- DisplayName (string)
- CreatedAt (DateTime)
```

### `DiagramCache`

```
- Id (Guid)
- MermaidHash (string, indexed)  // SHA256 of mermaid code
- RenderedSvg (string)           // Cached SVG output
- Theme (string)
- CreatedAt (DateTime)
- ExpiresAt (DateTime)
```

---

## Roadmap

- [ ] JWT authentication
- [ ] Server-side Mermaid rendering (PuppeteerSharp)
- [ ] Diagram caching (SHA-256 hash → SVG cache)
- [ ] Document export (HTML / PDF via QuestPDF)
- [ ] FluentValidation pipeline
- [ ] Rate limiting on render endpoint
- [ ] Unit & integration tests (xUnit + Moq)
