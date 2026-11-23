# Hlibz.Stack

Where the Hlibz packages meet: a small, real app built the way a new project would start, so the
packages can be tried together, and a new one dropped in to see how it fits.

The app is **todo lists**: users sign up and keep their own lists; admins see every user and
manage anyone's lists. One set of use cases, two clients: an HTTP API and an MCP server.

```text
Hlibz.Stack.Core            entities, strongly-typed ids, repository contracts
Hlibz.Stack.Application     use cases: validation, authorization, the rules (no ASP.NET Core, no EF Core)
Hlibz.Stack.Infrastructure  SQLite through EF Core, repositories, access tokens
Hlibz.Stack.Api             HTTP: endpoints, OpenAPI documents, ReDoc          http://localhost:5400
Hlibz.Stack.Mcp             MCP: the same use cases as tools, a docs page      http://localhost:5401
```

## The packages here

| Package | Where | What it does here |
|---|---|---|
| [Hlibz.UseCases](https://www.nuget.org/packages/Hlibz.UseCases) | Application | Use case contracts, handlers registered without scanning, a pipeline that validates DataAnnotations and runs the authorization filter in both hosts |
| [Hlibz.Results](https://www.nuget.org/packages/Hlibz.Results) | Core | Every use case returns a `Result`; failures carry error codes (`stack_10xx`) |
| [Hlibz.Paging](https://www.nuget.org/packages/Hlibz.Paging) (+ `.EntityFrameworkCore`) | Application, Infrastructure | `[Sortable]` DTOs, sort strings checked by the use case through `[SortableBy]`, `PaginateAsync` |
| [Hlibz.EntityFrameworkCore.ModelRules](https://www.nuget.org/packages/Hlibz.EntityFrameworkCore.ModelRules) | Infrastructure | snake_case, max lengths, enums as strings, no shadow properties; a test verifies the model |
| [Hlibz.MinimalApi.Endpoints](https://www.nuget.org/packages/Hlibz.MinimalApi.Endpoints) | Api | One class per endpoint, groups, documented errors |
| [Hlibz.Results.AspNetCore](https://www.nuget.org/packages/Hlibz.Results.AspNetCore) | Api | `Result` → HTTP, one `ErrorResponse` body for every failure |
| [Hlibz.OpenApi.AspNetCore](https://www.nuget.org/packages/Hlibz.OpenApi.AspNetCore) + [Hlibz.OpenApi.UseCases](https://www.nuget.org/packages/Hlibz.OpenApi.UseCases) + [Hlibz.OpenApi.Paging](https://www.nuget.org/packages/Hlibz.OpenApi.Paging) | Api | Strongly-typed ids as `uuid` components, XML docs, bearer security, the use cases' rules and `400`s, what each sort string sorts by and its default |
| [Hlibz.Redoc.AspNetCore](https://www.nuget.org/packages/Hlibz.Redoc.AspNetCore) | Api | ReDoc at `/api-docs`, an app and an admin document |
| [Hlibz.McpDocs.AspNetCore](https://www.nuget.org/packages/Hlibz.McpDocs.AspNetCore) | Mcp | The MCP server's docs page at `/mcp-docs` |
| [Hlibz.TypeRules.Analyzers](https://www.nuget.org/packages/Hlibz.TypeRules.Analyzers) | everywhere | Endpoints, handlers, use cases, entities and ids keep their shapes (`.editorconfig`) |

## Run it

Requires the .NET 11 SDK. Each host migrates the SQLite database (`.data/stack.db`, shared by
both) when it starts, and seeds two users in Development:

| Email | Password | Role |
|---|---|---|
| `admin@stack.test` | `Passw0rd!` | Admin |
| `user@stack.test` | `Passw0rd!` | User |

```bash
dotnet run --project src/Hlibz.Stack.Api
```

```bash
dotnet run --project src/Hlibz.Stack.Mcp
```

Log in with `POST api/auth/login` (from ReDoc at http://localhost:5400/api-docs, or `curl`), then
send the `access_token` as `Authorization: Bearer {token}` to either host. To connect Claude Code:

```bash
claude mcp add --transport http hlibz-stack http://localhost:5401/mcp --header "Authorization: Bearer <token>"
```

Delete `.data/stack.db` to start over.

## How a call goes

```text
HTTP endpoint ─┐                                    ┌─ validation (DataAnnotations on the use case)
               ├─ I<UseCase>Handler.HandleAsync ────┼─ AuthorizationFilter ([Anonymous], [AdminOnly])
MCP tool ──────┘                                    └─ the handler → Result
```

Clients never check anything themselves: a blank title, an unknown sort key, a user calling an
admin use case, or someone else's list fail the same way in both hosts. The caller's role is read
from the database on each call, not from the token, so a role change applies at once.

## Working on it

```bash
dotnet build      # also writes src/Hlibz.Stack.Api/openapi/*.json: commit them
dotnet test
dotnet tool restore
dotnet ef migrations add <Name> -p src/Hlibz.Stack.Infrastructure -s src/Hlibz.Stack.Infrastructure -o Database/Migrations
```

CI builds, tests, and fails when the OpenAPI documents or migrations are stale, or when the
Application project picks up ASP.NET Core or EF Core.

## Friction found so far

What this repository is for: places where the packages don't yet fit together smoothly.

- **Query parameter names.** A list use case bound with `[AsParameters]` would show its PascalCase
  names (`PerPage`) in the query string and the document, so each list endpoint has a snake_case
  parameter class in the Api that repeats the rules for the document. A naming policy for
  `[AsParameters]` (binding and OpenAPI) would remove it.
- **Two JSON styles.** HTTP is snake_case; MCP tool results use the MCP SDK's camelCase.
- **Host wiring twice.** Both hosts repeat the JWT bearer setup and `HttpCurrentUser`.
- **Admin tools are listed for everyone.** Roles live in the database, not in token claims, so
  `[Authorize(Roles = ...)]` (and McpDocs' per-viewer filtering) can't hide them; the use cases
  answer non-admins with Forbidden.
- **Validation messages use C# names** (`The Title field is required.`) under snake_case keys.
