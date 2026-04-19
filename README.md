# MermaidFlow

A .NET backend for creating, managing, and rendering Mermaid diagram documents. Write Markdown with embedded Mermaid diagrams — MermaidFlow handles storage, server-side rendering, and export.

---

## Tech Stack

| Layer          | Technology                           |
| -------------- | ------------------------------------ |
| Framework      | ASP.NET Core 10 (Web API)            |
| Architecture   | Clean Architecture + CQRS (MediatR)  |
| ORM            | Entity Framework Core 9 (SQL Server) |
| Authentication | JWT Bearer                           |
| Rendering      | Playwright (headless Chromium)       |
| Validation     | FluentValidation                     |
| API Docs       | Scalar (OpenAPI)                     |

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

    subgraph Auth ["🔐 Auth"]
        A1["POST /auth/register\nRegister user"]
        A2["POST /auth/login\nLogin → JWT"]
        A3["POST /auth/refresh\nRefresh token"]
        A4["POST /auth/logout\nRevoke token"]
    end

    subgraph Users ["👤 Users"]
        U1["POST /users\nCreate user"]
        U2["GET /users\nList all users"]
        U3["GET /users/{id}\nGet user"]
        U4["PUT /users/{id}\nUpdate user"]
        U5["DELETE /users/{id}\nDelete user"]
        U6["PATCH /users/{id}/email\nUpdate email"]
        U7["PATCH /users/{id}/display-name\nUpdate display name"]
    end

    subgraph Documents ["📄 Documents"]
        D1["POST /documents\nCreate document"]
        D2["GET /documents\nList user documents"]
        D3["GET /documents/{id}\nGet document"]
        D4["PUT /documents/{id}\nUpdate document"]
        D5["DELETE /documents/{id}\nDelete document"]
        D6["GET /documents/public\nGet public documents"]
        D7["GET /documents/{id}/export?format=html\nExport document"]
    end

    subgraph Mermaid ["🔷 Mermaid"]
        M1["POST /mermaid/render\nRender → SVG"]
        M2["POST /mermaid/validate\nValidate syntax"]
        M3["POST /mermaid/export\nExport SVG / PNG"]
        M4["GET /mermaid/themes\nList themes"]
    end

    Client --> A1 & A2 & A3 & A4
    Client --> U1 & U2 & U3 & U4 & U5 & U6 & U7
    Client --> D1 & D2 & D3 & D4 & D5 & D6 & D7
    Client --> M1 & M2 & M3 & M4

    A2 -- "JWT token" --> U3 & D1 & D2 & D3 & D4 & D5
    A2 -- "JWT token" --> M1 & M2 & M3

    U1 -- "creates" --> U3
    D1 -- "owned by" --> U3
    D2 -- "content fed to" --> M1
    M1 -- "cached SVG" --> D2
```

---

## Implemented Features

### Authentication & Authorization

- JWT Bearer token authentication with refresh token flow
- PBKDF2 password hashing (SHA256, 100k iterations)
- Token revocation on logout
- Role-based authorization policies

### Mermaid Rendering

- Server-side rendering using Playwright (headless Chromium)
- Support for both `application/json` and `text/plain` content types
- SVG output with theme support
- Syntax validation endpoint

### Diagram Caching

- SHA-256 hash-based caching of rendered SVGs
- Configurable cache expiration
- Reduces server load for repeated renders

### Document Management

- Full CRUD operations for documents
- Public/private document visibility
- Document ownership and access control

### API Features

- OpenAPI documentation via Scalar
- FluentValidation request/response validation
- ErrorOr pattern for consistent error responses

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

- [x] JWT authentication
- [x] Server-side Mermaid rendering (Playwright)
- [x] Diagram caching (SHA-256 hash → SVG cache)
- [x] Document export (HTML)
- [x] FluentValidation pipeline
- [x] Unit & integration tests (xUnit + Moq)
- [ ] Real-time preview (SignalR)
