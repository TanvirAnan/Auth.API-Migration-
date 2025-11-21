Title Auth.API – .NET 8 CQRS + MediatR + JWT Authentication Sample

Overview
•	Layered (Clean) architecture: Domain, Application, Infrastructure, API, BuildingBlocks.

•	Implements CQRS with MediatR (commands/queries + pipeline behaviors).

•	PostgreSQL (EF Core) with automatic migrations.

•	JWT authentication + global authorization fallback (all endpoints require auth unless [AllowAnonymous]).

•	Value objects: UserId, Email.

•	Validation: FluentValidation.

•	Caching (in-memory distributed placeholder).

Solution Structure

•	Domain: Entities (User), Value Objects (UserId, Email), Interfaces.
•	Application: Commands (CreateUser, UpdateUser), Queries (Login), Validators, Handlers, Behaviors.
•	Infrastructure: EF Core DbContext, Entity configurations, Repository implementations, Migrations.
•	Auth.API: ASP.NET Core host, DI wiring, JWT configuration, Controllers (Register, Login, UpdateUser).
•	BuildingBlocks: CQRS abstractions, common exception types, pipeline behaviors.

Key Packages
•	MediatR
•	FluentValidation
•	EF Core
•	Microsoft.AspNetCore.Authentication.JwtBearer
•	System.IdentityModel.Tokens.Jwt
Configuration (appsettings.json)
•	ConnectionStrings: Database, Redis.
•	JwtSettings: Issuer, Audience, Key, ExpiresMinutes. Set a strong Key in user secrets or environment for production.

Database & Migrations Add migration: dotnet ef migrations add InitialCreate -p Infrastructure/Infrastructure.csproj -s Auth.API/Auth.API/Auth.API.csproj Update database: dotnet ef database update -p Infrastructure/Infrastructure.csproj -s Auth.API/Auth.API/Auth.API.csproj Automatic migration runs at startup.

Authentication Flow
1.	Register user: POST /api/Register (anonymous).
2.	Login: POST /api/Login returns { user, token }.
3.	Use token in Authorization header: Bearer <jwt>.
4.	Protected endpoints require valid token; fallback policy enforces authentication.
   
JWT Claims
•	sub: username
•	email
•	uid (not used in update now; update relies on sub)
•	jti

Controllers / Endpoints
•	POST /api/Register Body: { firstName, lastName, userName, email, password } Returns: created user (with Id) and metadata.
•	POST /api/Login Body: { userName, password } Returns: { user, token }
•	PUT /api/UpdateUser Auth: required Body: { firstName, lastName, userName, email, password } (password = current password) Uses token sub claim to ensure self-update. Returns { id }.
User Entity Fields: Id (UserId value object mapped to uuid), FirstName, LastName, UserName, Email (value object + conversion), Password (plain text – requires hashing improvement).

CQRS Behaviors
•	ValidationBehavior: runs FluentValidation before handlers.
•	LoggingBehavior: logs start/end and performance threshold.

Error Handling
•	CustomExceptionHandler returns ProblemDetails with traceId.
•	Throws NotFoundException, BadRequestException, ValidationException.
Typical Request/Response (Login) Request: POST /api/Login { "userName": "alice", "password": "Alice@2025" } Response: { "user": { ... }, "token": "eyJ..." }

Security Notes
•	Passwords stored plaintext (improve by hashing with PBKDF2/Argon2).
•	JWT Key should be rotated and stored securely (secrets manager / Key Vault).
•	Consider refresh tokens for longer sessions.
•	Consider adding roles/claims/policies.

Swagger Usage
•	Click Authorize.
•	Paste JWT (without Bearer prefix).
•	Invoke protected endpoints.
