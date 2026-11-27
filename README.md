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
| [Hlibz.Results](https://www.nuget.org/packages/Hlibz.Results) | Core, Application | Every use case returns a `Result`; one that can fail declares how as an error set, `Result<T, TError>` with `TError` a `union` of error records (`stack_10xx`), next to the use case |
| [Hlibz.Paging](https://www.nuget.org/packages/Hlibz.Paging) (+ `.EntityFrameworkCore`) | Application, Infrastructure | `[Sortable]` DTOs, sort strings checked by the use case through `[SortableBy]`, `PaginateAsync` |
| [Hlibz.EntityFrameworkCore.ModelRules](https://www.nuget.org/packages/Hlibz.EntityFrameworkCore.ModelRules) | Infrastructure | snake_case, max lengths, enums as strings, no shadow properties; a test verifies the model |
| [Hlibz.MinimalApi.Endpoints](https://www.nuget.org/packages/Hlibz.MinimalApi.Endpoints) | Api | One class per endpoint, groups, the auth gate's `401`/`403` and the rate limiter's `429` documented |
| [Hlibz.Results.AspNetCore](https://www.nuget.org/packages/Hlibz.Results.AspNetCore) | Api | `Result` → HTTP, one `ErrorResponse` body for every failure (the rate limiter's too), each endpoint's declared errors documented from its handler's error set, or from `ProducesErrorSet` where the endpoint sends a file |
| [Hlibz.OpenApi.AspNetCore](https://www.nuget.org/packages/Hlibz.OpenApi.AspNetCore) + [Hlibz.OpenApi.UseCases](https://www.nuget.org/packages/Hlibz.OpenApi.UseCases) + [Hlibz.OpenApi.Paging](https://www.nuget.org/packages/Hlibz.OpenApi.Paging) + [Hlibz.OpenApi.Results](https://www.nuget.org/packages/Hlibz.OpenApi.Results) | Api | Strongly-typed ids as `uuid` components, XML docs, bearer security, the use cases' rules and `400`s, what each sort string sorts by and its default, each declared error's own body (its code and details; a `oneOf` where a status has two, with the ambient errors sent with that status) |
| [Hlibz.Redoc.AspNetCore](https://www.nuget.org/packages/Hlibz.Redoc.AspNetCore) | Api | ReDoc at `/api-docs`, an app and an admin document |
| [Hlibz.Results.Mcp](https://www.nuget.org/packages/Hlibz.Results.Mcp) | Mcp | A failed tool returns `isError` with guidance (what to do next) and `{ code, details }`; each tool lists its errors in `_meta`; the host doesn't start while an error has no guidance |
| [Hlibz.McpDocs.AspNetCore](https://www.nuget.org/packages/Hlibz.McpDocs.AspNetCore) | Mcp | The MCP server's docs page at `/mcp-docs`, with each tool's errors and their guidance |
| [Hlibz.TypeRules.Analyzers](https://www.nuget.org/packages/Hlibz.TypeRules.Analyzers) | everywhere | Endpoints, handlers, use cases, entities and ids keep their shapes (`.editorconfig`) |

## Run it

Requires the .NET 11 SDK; the Hlibz packages are previews on nuget.org (the .NET 11 majors:
Hlibz.Results 3 with Hlibz.Results.Mcp, Hlibz.UseCases 2, Hlibz.OpenApi 2,
Hlibz.MinimalApi.Endpoints 2; and Hlibz.McpDocs.AspNetCore 0.3). Each host migrates the SQLite database (`.data/stack.db`, shared by
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
HTTP endpoint ─┐                                    ┌─ validation (DataAnnotations) → ValidationFailed
               ├─ I<UseCase>Handler.HandleAsync ────┼─ AuthorizationFilter → Unauthorized, Forbidden
MCP tool ──────┘                                    ├─ ConcurrencyConflictFilter → ConcurrencyConflict
                                                    └─ the handler → Result<T, TError>
```

Clients never check anything themselves: a blank title, an unknown sort key, a user calling an
admin use case, or someone else's list fail the same way in both hosts. The caller's role is read
from the database on each call, not from the token, so a role change applies at once.

A use case's errors are its own: `UpdateTodoItemError(TodoListNotFound, TodoItemNotFound)` is
what its handler may fail with, and the compiler holds it to that. The layers around it add the
ambient errors (`validation_failed`, `unauthorized`, `forbidden`, and `concurrency_conflict` when
a save finds its rows changed meanwhile: `ConcurrencyConflictFilter` in Infrastructure turns EF
Core's `DbUpdateConcurrencyException` into it, so no handler knows EF Core). The Api's rate
limiter adds `rate_limited` before any use case runs. Each error carries the data a
client acts on (`TodoListNotFound` its `todo_list_id`), and its message is for logs, not for end
users: clients branch on the code.

## Errors to try

Every failure has the same body: `code` to branch on, `message` for logs, and `details` with what
the client needs to act on (when the error has any). The documents at `/api-docs` show each
endpoint's errors with their own bodies.

| Code | Status | When | Details |
|---|---|---|---|
| `stack_1001` | 409 | Signing up with an email address someone has | |
| `stack_1002` | 401 | Logging in with a wrong email address or password | |
| `stack_1003` | 404 | A todo list that doesn't exist, or is someone else's | `todo_list_id` |
| `stack_1004` | 404 | An item that isn't on the list | `item_id` |
| `stack_1005` | 404 | A user that doesn't exist | `user_id` |
| `stack_1006` | 400 | An admin changing their own role | |
| `stack_1007` | 409 | Creating or renaming a list to a title the owner has (ignoring case) | `todo_list_id`: the list that has it |
| `stack_1008` | 409 | Creating an 11th list | `max_todo_lists` |
| `stack_1009` | 409 | Adding a 21st item to a list | `max_items` |
| `stack_1010` | 409 | Adding or renaming an item to a title on the list (ignoring case) | `item_id`: the item that has it |
| `validation_failed` | 400 | A blank title, an unknown sort key, ... | `validation_errors` |
| `unauthorized` | 401 | No access token, or an expired one | |
| `forbidden` | 403 | A user calling an admin endpoint, or another user's lists | |
| `concurrency_conflict` | 409 | A change to a list someone deleted meanwhile | |
| `rate_limited` | 429 | An 11th call to `api/auth` within a minute from one address (`Retry-After` says when to retry) | |

`GET api/todo-lists/{id}/export` sends a list as a Markdown checklist, a file rather than the use
case's value, so its endpoint declares the use case's errors with `ProducesErrorSet`; a missing
list is the same JSON `404` as anywhere else.

Over MCP a failed tool returns `isError` with guidance instead of the message: what to do next,
in terms of the server's tools, with the details filled in (`stack_1003: No todo list with id … is
visible to you … Call list_todo_lists to find the list's id.`), and `{ code, details }` as
`structuredContent` for programs. Each tool lists the errors it can return, with their guidance, in
its `_meta` under `hlibz/errors` (Hlibz.Results.Mcp); the docs page at `/mcp-docs` lists them on
each tool under **Errors**, and `catalog.md` as a table. The guidance texts are the host's own
(`Errors/StackGuidance.cs`), and each tool's errors come from the use case it runs
(`Errors/UseCaseToolErrors.cs`).

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
- **Validation messages use C# names** (`The Title field is required.`) under snake_case keys
  (camelCase over MCP): the keys are converted, the messages DataAnnotations writes aren't.
- **MCP tools can't return a `Result`.** The MCP SDK serializes a tool's return value before any
  filter sees it, so every tool unwraps with `ValueOrThrow()` and the failure travels as an
  exception (Hlibz.Results.Mcp's filter catches it).
- **A tool's errors come from reflection over the tools.** MCP's `McpServerTool` doesn't expose the
  method, so `UseCaseToolErrors` finds each tool's method by name to match its handler parameter
  to a use case. Generic enough for a Hlibz.UseCases + MCP package, once a second app needs it.
- **The use case catalog names handler classes.** `UseCaseDescriptor.HandlerType` is the internal
  handler, not the `I…Handler` a host injects, so a host matches by `IsAssignableFrom`.
- **Errors a handler returns on its own are invisible.** `GetTodoLists` returns `Forbidden` for
  another user's lists; it's ambient, so no error set or attribute says so, and
  `admin_list_user_todo_lists` doesn't list it.
- **Ambient errors are listed by status code, not by what can happen.** A declared `409` lists
  `concurrency_conflict` even on `POST api/auth/register`, which only inserts; login's declared
  `401` lists `unauthorized` though login is anonymous. Any result accepts an ambient error, so
  the documents list every one sent with a declared status rather than guess.
- **A status code only ambient errors use stays a plain `ErrorResponse`, or isn't there.** The
  gates' `401`/`403`/`429` and a validation-only `400` aren't narrowed to their codes the way a
  declared status is; and the deletes, which can fail with `concurrency_conflict`, document no
  `409` at all (nothing declares one).
- **`403` on every signed-in endpoint.** Hlibz.MinimalApi.Endpoints documents `401`/`403` from
  `RequireAuthorization()`; only the use cases know which can be forbidden (`[AdminOnly]`, another
  user's lists), so `GET api/users/me` shows a `403` it never returns.
