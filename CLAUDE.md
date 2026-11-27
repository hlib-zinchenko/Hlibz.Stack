# CLAUDE.md

Guidance for Claude Code in this repository.

## What this is

A playground where the Hlibz NuGet packages are used together in a small app (todo lists, users,
admins) with two hosts: an HTTP API and an MCP server. The point is to find how the packages fit
and what should change in them; record friction in README.md's "Friction found so far".

## Commands

```bash
dotnet build                                   # also regenerates src/Hlibz.Stack.Api/openapi/*.json
dotnet test                                    # Microsoft Testing Platform (global.json)
dotnet run --project src/Hlibz.Stack.Api       # http://localhost:5400/api-docs
dotnet run --project src/Hlibz.Stack.Mcp       # http://localhost:5401/mcp-docs, MCP at /mcp
dotnet tool restore
dotnet ef migrations add <Name> -p src/Hlibz.Stack.Infrastructure -s src/Hlibz.Stack.Infrastructure -o Database/Migrations
```

Stop any host you start. Build before committing a change to endpoints, use cases or DTOs, and
commit the changed OpenAPI documents; CI diffs them.

## Layers

- **Core**: entities (`Entity<TId>`: private setters, read-only collections, static factories,
  private constructors EF Core binds), strongly-typed ids (`[StronglyTypedId(Template.Guid)]`,
  `IEntityId<TSelf>` with `From(Guid)`, new ids from `EntityId.New<TId>()`, a version 7 Guid),
  repository contracts and `IUnitOfWork`. No EF Core. Core owns the Hlibz.Results reference:
  `Result` is part of the domain's vocabulary, and every layer above gets it from here.
- **Application**: one folder per use case, `UseCases/<Area>/<UseCase>/` (namespace to match),
  holding the use case, its handler interface, its handler and its body records; what an area's
  use cases share sits at the area's root (`UseCases/TodoLists/TodoListAccess`). DTOs are in
  `Dto/`, read repositories in `ReadRepositories/`. Each use case is a record (`IUseCase`) with its
  DataAnnotations, an `I<Name>Handler` interface (public, what hosts inject) and an internal
  sealed handler returning `Result<T, TError>` (`Unit` for no value), `TError` the use case's
  error set: `<UseCase>Error.cs`, a `union` of error records, beside it. A use case that can't
  fail on its own (only validation and authorization, which are ambient) returns `Result<T>`. Bodies that sit beside a route id are their own
  records (`TodoListChanges`). Everything needs a signed-in user unless marked `[Anonymous]`;
  `[AdminOnly]` needs an admin; `AuthorizationFilter` enforces both, reading the role from the
  database through `ICallerResolver`, and fails with the ambient `Unauthorized`/`Forbidden`. A list
  owner or an admin may manage a list (`Caller.CanManage`); for anyone else it's `TodoListNotFound`
  (`404 stack_1003`). List queries take `Page`, `PerPage` (`[Range(1, 100)]`)
  and `Sort` (Hlibz.Paging's `[SortableBy(typeof(Dto), NamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower)]`);
  handlers parse sorting with `SortableDto.For<TDto>().Parse(sort)`.
  Errors are `[Error("stack_10xx", ...)]` sealed records with the data a client acts on (ids),
  in the use case's folder, or at the area's root when several share one (`TodoListNotFound`);
  the Results generator rejects a reused code. Their messages are for logs: clients branch on
  codes. Read repositories are internal interfaces
  here, implemented in Infrastructure. No ASP.NET Core or EF Core (CI checks).
- **Infrastructure**: `StackDbContext` (SQLite, snake_case, ModelRules, ids through
  `EntityIdConverter<TId>`, `DateTimeOffset` stored as a number so SQLite can sort it),
  `TimestampInterceptor` (created/updated times, and a list's `updated_at` when its items change),
  repositories, `JwtAccessTokenIssuer`, `StackDatabase.InitializeAsync` (migrate, then seed from
  `Seed:Users` when there are no users). Migrations in `Database/Migrations` (no analyzers).
- **Api**: `Hlibz.MinimalApi.Endpoints` classes under `Endpoints/<Area>/`, one per route, calling
  one handler and returning its `Result<T, TError>`: `WithResultResponses()` documents the error
  set and `AddResults()` (Hlibz.OpenApi.Results) each error's body, so no `ProducesErrors`. The use case is the body where it can be; route ids plus a body record otherwise;
  list query strings go through a snake_case `ListParameters` subclass. Admin groups implement
  `IAdminEndpointGroup` (admin document, signed-in only). Two OpenAPI documents, `stack` and
  `admin`, written to `openapi/` on build.
- **Mcp**: static tool classes under `Tools/`, one tool per use case, ids as `Guid` parameters,
  results unwrapped with Hlibz.Results.Mcp's `ValueOrThrow()`/`ThrowIfFailed()`.
  `WithToolErrors(StackGuidance.Configure)` turns a failure into `isError` with the error's
  guidance as text and `{ code, details }` (camelCase) as structured content, and writes each
  tool's errors into its `_meta` (`hlibz/errors`, the docs page's Errors block). `Errors/`:
  `StackGuidance`, a template per error type whose `{placeholders}` are the error's details, and
  `UseCaseToolErrors`, the `IToolErrorSource` that gives each tool its use case's errors (the
  handler's error set from `IUseCaseCatalog`, plus `validation_failed`/`forbidden`), so the tools
  declare none. The host doesn't start when an error has no guidance or a placeholder isn't a
  detail: a new error in a tool's error set needs guidance first; `McpToolErrorTests` start it.
  Stateless HTTP transport, so each tool call reads its own request's token.

## Packages

The Hlibz .NET 11 majors (Hlibz.Results 3, Hlibz.UseCases 2, Hlibz.OpenApi 2,
Hlibz.MinimalApi.Endpoints 2) and Hlibz.McpDocs.AspNetCore 0.3 are previews on nuget.org; move
to each stable release when it ships.

## Code style

`.editorconfig` comes from Lighthouse: StyleCop, Sonar and `AnalysisMode=All`, warnings as errors,
explicit types (no `var`), file-scoped namespaces, one type per file, XML `<summary>`s on what the
API or MCP docs show (they become descriptions), `// !` justification for any `!`. TypeRules rule
sets at the end of `.editorconfig`. Tests are `Subject_WithCondition_ExpectedOutcome`, through the
Api (`StackApiFactory`, a SQLite file per test class).
