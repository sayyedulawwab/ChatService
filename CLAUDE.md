# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

Single ASP.NET Core 10 Web API project (`ChatService/ChatService.csproj`) in `ChatService.slnx`. There is no test project.

`System.Security.Cryptography.Pkcs` is referenced directly only to override the vulnerable 9.0.x version that `Microsoft.Data.SqlClient` pulls in. Swashbuckle 10 uses Microsoft.OpenApi 2.x (namespace `Microsoft.OpenApi`, `OpenApiSecuritySchemeReference`).

```bash
dotnet build ChatService.slnx
dotnet run --project ChatService                       # http://localhost:5223, Swagger at /swagger
dotnet run --project ChatService --launch-profile https # https://localhost:7000

# EF Core migrations (SQL Server only; Mongo has no migrations)
dotnet ef migrations add <Name> --project ChatService
dotnet ef database update --project ChatService
```

In the Development environment, pending EF migrations are applied automatically at startup (`ApplyMigrations()` in [Program.cs](ChatService/Program.cs)).

Running locally requires three backing services. Their connection strings and the `Jwt` section (issuer, audience, secret key) live in [appsettings.Development.json](ChatService/appsettings.Development.json); [appsettings.json](ChatService/appsettings.json) holds only environment-agnostic settings, so any other environment must supply them through environment variables (e.g. `ConnectionStrings__SQLServer`, `ConnectionStrings__Redis`, `MongoDatabaseOptions__ConnectionString`, `Jwt__SecretKey`). Development defaults: SQL Server LocalDB (`(localdb)\mssqllocaldb`, database `ChatDB`), MongoDB on `localhost:27017`, and Redis on `localhost:6379`. The csproj references `..\docker-compose.dcproj`, but that file is not in the repo.

## Architecture

The project uses Clean Architecture with CQRS. The layers are folders inside one project, not separate assemblies, so the compiler does not enforce layer boundaries:

- `Domain/`: entities, repository interfaces, the `Result`/`Error` types, and per-aggregate `*Errors` classes.
- `Application/`: commands and queries (no MediatR), one folder per use case (e.g. `Rooms/JoinRoom/`). The `Abstractions/` folder holds interfaces that Infrastructure implements.
- `Infrastructure/`: EF Core, MongoDB, Redis, JWT, and password hashing.
- `Controllers/` and `Hubs/`: the presentation layer.
- [DependencyInjection.cs](ChatService/DependencyInjection.cs): all service registration (`AddApplication`, `AddInfrastructure`).

### Three data stores, split by entity

| Store | Entities | Access |
|---|---|---|
| SQL Server (EF Core, `EfDbContext`) | `User`, `Room`, `Member` | Repositories plus `IUnitOfWork` (which is `EfDbContext`). Mapping is in `Infrastructure/EntityConfigurations/`. |
| MongoDB (`MongoDbContext`, singleton) | `Conversation`, `Message` | Repositories write directly, with no unit of work. The entities carry `[BsonId]` attributes in Domain. Indexes are created in the `MongoDbContext` constructor. |
| Redis (`ICacheRepository` over `IDistributedCache`) | SignalR connection state, recent room messages | Keys are `connection-{ConnectionId}` → `UserConnection` and `room-{roomId}` → message list. |

SQL entities use `long` ids. Mongo entities use string ObjectIds. A group `Conversation` links to a room through `RoomId` and must have an empty `Participants` list. A direct conversation has a null `RoomId` and exactly two participants. `Conversation.Create` enforces both rules by throwing `ArgumentException`. Creating a room does **not** create its conversation: the client calls `POST /api/conversations` separately. `AddMessage` fails if the room's conversation does not exist yet.

### Request flow and conventions

- Commands implement `ICommand`/`ICommand<T>` and queries implement `IQuery<T>`. Handlers return `Result`/`Result<T>`, and handlers are `internal sealed`.
- Handlers are registered by a Scrutor assembly scan in `AddApplication` ([DependencyInjection.cs](ChatService/DependencyInjection.cs)), so a new handler is picked up automatically. There is no dispatcher: controllers inject the handler into the action method (e.g. `ICommandHandler<CreateRoomCommand, long> handler`, inferred as `[FromServices]`), and `ChatHub` takes `ICommandHandler<AddMessageCommand, string>` in its constructor.
- When `result.IsFailure`, a controller returns `result.Error.ToActionResult()` ([ErrorMappingExtensions.cs](ChatService/Extensions/ErrorMappingExtensions.cs)), which maps the `HttpResponseStatusCodes` value carried on the `Error` to an HTTP result. To add a new failure mode, define it in the aggregate's `*Errors` class with `Error.NotFound/BadRequest/Conflict(...)`.
- Cross-cutting behavior is applied with Scrutor `TryDecorate` (`Application/Abstractions/Decorators/`). `LoggingDecorator` wraps commands and queries and is the outermost layer. `ValidationDecorator` wraps **commands only**: it runs every FluentValidation `IValidator<TCommand>` and, on failure, returns `Result.Failure` with a `ValidationError` (an `Error` subtype carrying per-property `Errors`), which `ToActionResult()` turns into a 400. Validators are auto-registered from the assembly, but none exist yet. A new decorator must be registered in `AddApplication`: each `TryDecorate` call wraps the previous one, so the last one registered runs first.
- Controllers read the current user id from the `ClaimTypes.NameIdentifier` claim.
- `AddMessageCommand` lives in `Application/Conversations/AddMessage/` but is declared in the `...Conversations.CreateConversation` namespace. Some Application handlers also import `ChatService.Infrastructure.Repositories`.

### SignalR hub (`/chat`)

[ChatHub.cs](ChatService/Hubs/ChatHub.cs) does **not** use `[Authorize]` or the JWT bearer middleware. In `OnConnectedAsync` it reads `access_token` from the query string, validates it manually with `JwtOptions`, and aborts the connection if validation fails. Hub methods: `JoinSpecificChatRoom(UserConnection)`, `SendMessage(roomId, senderId, content)`, `Typing(roomId, username)`. Client events: `UserJoined`, `UserLeft`, `ReceiveMessage`, `UserTyping`, `Error`. `SendMessage` appends the message to the Redis room list, persists it to Mongo via `AddMessageCommand`, and broadcasts it to the room group.

The two JWT paths disagree on lifetime: the hub validates token lifetime, but `JwtBearerOptionsSetup` (used by the REST controllers) sets `ValidateLifetime = false`. The JWT settings come from the `Jwt` config section (`JwtOptionsSetup`).

CORS policy `ChatClient` allows only `http://localhost:5173`, the frontend dev server, with credentials enabled.
