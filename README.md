# MermaidFlow

A .NET backend for creating, managing, and rendering Mermaid diagram documents. Write Markdown with embedded Mermaid diagrams — MermaidFlow handles storage, server-side rendering, and export.

---

## Tech Stack

| Layer          | Technology                               |
| -------------- | ---------------------------------------- |
| Framework      | ASP.NET Core 10 (Web API)                |
| Architecture   | Clean Architecture + CQRS (MediatR)      |
| ORM            | Entity Framework Core 9 (SQL Server)     |
| Authentication | JWT Bearer _(planned)_                   |
| Rendering      | PuppeteerSharp / Mermaid CLI _(planned)_ |
| Validation     | FluentValidation _(planned)_             |
| API Docs       | Scalar (OpenAPI)                         |

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

### Documents

| Method   | Endpoint          | Description                   |
| -------- | ----------------- | ----------------------------- |
| `POST`   | `/Documents`      | Create a document             |
| `GET`    | `/Documents/{id}` | Get a document by ID          |
| `PUT`    | `/Documents/{id}` | Update a document _(planned)_ |
| `DELETE` | `/Documents/{id}` | Delete a document _(planned)_ |

### Users

| Method   | Endpoint      | Description     |
| -------- | ------------- | --------------- |
| `POST`   | `/Users`      | Register a user |
| `GET`    | `/Users`      | List all users  |
| `GET`    | `/Users/{id}` | Get user by ID  |
| `PUT`    | `/Users/{id}` | Update user     |
| `DELETE` | `/Users/{id}` | Delete user     |

### Auth _(planned)_

| Method | Endpoint        | Description         |
| ------ | --------------- | ------------------- |
| `POST` | `/auth/login`   | Login → returns JWT |
| `POST` | `/auth/refresh` | Refresh token       |
| `POST` | `/auth/logout`  | Revoke token        |

### Mermaid _(planned)_

| Method | Endpoint            | Description               |
| ------ | ------------------- | ------------------------- |
| `POST` | `/mermaid/render`   | Render Mermaid code → SVG |
| `POST` | `/mermaid/validate` | Validate Mermaid syntax   |
| `POST` | `/mermaid/export`   | Export as SVG or PNG      |

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

Open **http://localhost:5209/scalar/v1** for the interactive API explorer.

### Connection String

Update `appsettings.json` in `MermaidFlow.Api`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MermaidFlowDb;Trusted_Connection=True;"
}
```

---

## Data Models

**Document** — `Id`, `Title`, `Content` (raw Markdown), `UserId` (FK), `CreatedAt`, `UpdatedAt`, `IsPublic`, `Tags`

**User** — `Id`, `Email` (unique), `PasswordHash` (PBKDF2/SHA-256), `DisplayName`, `CreatedAt`

---

## Roadmap

- [ ] JWT authentication
- [ ] Server-side Mermaid rendering (PuppeteerSharp)
- [ ] Diagram caching (SHA-256 hash → SVG cache)
- [ ] Document export (HTML / PDF via QuestPDF)
- [ ] FluentValidation pipeline
- [ ] Rate limiting on render endpoint
- [ ] Unit & integration tests (xUnit + Moq)
