# ChatService

ASP.NET Core 10 backend for a real-time chat application. Users register and log in with JWT authentication, create and join password-protected rooms, and exchange messages over a SignalR hub. Data is split across SQL Server (users and rooms), MongoDB (conversations and messages) and Redis (SignalR connection state and recent room messages). The matching React frontend is the `ChatClient` repo.

## Tech stack

- .NET 10, ASP.NET Core Web API, SignalR
- EF Core 10 with SQL Server
- MongoDB (official .NET driver)
- Redis (`IDistributedCache`)
- JWT bearer authentication
- FluentValidation, Scrutor, Swashbuckle (Swagger)

## Architecture

Clean Architecture with CQRS (no MediatR), organized as folders inside a single project (`ChatService/ChatService.csproj`, solution `ChatService.slnx`). The layer boundaries are not enforced by separate assemblies.

| Folder | Role |
|---|---|
| `Domain/` | Entities, repository interfaces, `Result`/`Error` types |
| `Application/` | Command and query handlers, one folder per use case |
| `Infrastructure/` | EF Core, MongoDB, Redis, JWT, password hashing |
| `Controllers/`, `Hubs/` | REST controllers and the SignalR hub |

Handlers are discovered by an assembly scan and injected directly into controller actions. Logging and validation are applied as decorators.

| Store | Entities |
|---|---|
| SQL Server | `User`, `Room`, `Member` |
| MongoDB | `Conversation`, `Message` |
| Redis | SignalR connection state, recent messages per room |

## Prerequisites

- .NET 10 SDK
- SQL Server (the Development settings use LocalDB), MongoDB and Redis running locally
- The `dotnet-ef` tool if you want to add migrations

## Running locally

```bash
dotnet build ChatService.slnx
dotnet run --project ChatService                        # http://localhost:5223, Swagger at /swagger
dotnet run --project ChatService --launch-profile https # https://localhost:7000
```

In the Development environment, pending EF Core migrations are applied automatically at startup. MongoDB has no migrations; indexes are created when the context is constructed.

Migrations (SQL Server only):

```bash
dotnet ef migrations add <Name> --project ChatService
dotnet ef database update --project ChatService
```

## Configuration

`appsettings.json` holds only environment-agnostic settings. Connection strings and the `Jwt` section live in `appsettings.Development.json`, which points at local services:

| Setting | Development default |
|---|---|
| `ConnectionStrings:SQLServer` | LocalDB, database `ChatDB` |
| `ConnectionStrings:Redis` | `localhost:6379` |
| `MongoDatabaseOptions:ConnectionString` / `DatabaseName` | `mongodb://localhost:27017` / `ChatDB` |
| `Jwt:Issuer`, `Jwt:Audience`, `Jwt:SecretKey` | See the file |

In any other environment supply these as environment variables using the `Section__Key` form, for example `ConnectionStrings__SQLServer`, `ConnectionStrings__Redis`, `MongoDatabaseOptions__ConnectionString` and `Jwt__SecretKey`.

## API overview

All REST routes are under `/api`. Controllers require a bearer token unless noted.

| Method and route | Description |
|---|---|
| `POST /api/users/register` | Register a user (`name`, `email`, `password`); anonymous |
| `POST /api/users/login` | Log in (`email`, `password`) and receive an access token; anonymous |
| `POST /api/rooms` | Create a room (`name`, `password`) |
| `GET /api/rooms/{id}` | Get a room |
| `PUT /api/rooms` | Join a room (`roomId`) |
| `POST /api/conversations` | Create a conversation (`roomId`, `participants`) |
| `GET /api/conversations/{roomId}` | Get messages for a room's conversation (paged with `page` and `pageSize`); marked anonymous |

Creating a room does not create its conversation; the client calls `POST /api/conversations` separately, and sending a message fails until the conversation exists.

### SignalR hub

The hub is at `/chat`. The client passes the JWT as the `access_token` query-string parameter; the hub validates it itself and aborts the connection if it is invalid.

- Methods: `JoinSpecificChatRoom`, `SendMessage(roomId, senderId, content)`, `Typing(roomId, username)`
- Client events: `UserJoined`, `UserLeft`, `ReceiveMessage`, `UserTyping`, `Error`

## Known limitations

- CORS allows only `http://localhost:5173` (the ChatClient dev server).
- The hub validates token lifetime, but the REST JWT bearer setup has `ValidateLifetime = false`.
- No FluentValidation validators are defined yet, so the validation decorator has nothing to run.
- There is no test project.
