Auth.API – .NET 8 CQRS + MediatR + JWT Authentication 
Overview & Key Features
•	Layered (Clean) Architecture: Domain, Application, Infrastructure, Auth.API, BuildingBlocks.
•	CQRS Implementation using MediatR (Commands & Queries).
•	PostgreSQL with EF Core and Automatic Migrations.
•	JWT Authentication with global authorization fallback.
•	Value Objects for domain modeling (UserId, Email).
•	Validation using FluentValidation.
•	Caching placeholder (In-memory / Redis).
Clean Architecture (Overview Diagram)

                +-------------------------+
                |       Auth.API          |
                |  (Controllers, JWT, DI) |
                +------------+------------+
                             |
                             v
                +-------------------------+
                |      Application        |
                | CQRS (Commands/Queries) |
                | Handlers, Validators    |
                +------------+------------+
                             |
                             v
                +-------------------------+
                |        Domain           |
                | Entities, ValueObjects  |
                +------------+------------+
                             |
                             v
                +-------------------------+
                |     Infrastructure      |
                | EF Core, Repositories   |
                | Migrations              |
                +-------------------------+

Solution Structure
Layer	Primary Responsibility	Key Contents
Domain	Business logic, entities, rules	User Entity, UserId, Email ValueObjects
Application	CQRS orchestration	Commands, Queries, Validators, Handlers
Infrastructure	Database & external services	EF Core DbContext, Repositories, Migrations
Auth.API	Entry point & presentation	Controllers, JWT Config, DI
BuildingBlocks	Common reusable code	CQRS abstractions, Exceptions, Behaviors
Key Packages
•	MediatR
•	FluentValidation
•	Microsoft.EntityFrameworkCore
•	Microsoft.AspNetCore.Authentication.JwtBearer
•	System.IdentityModel.Tokens.Jwt
•	AspNetCore.HealthChecks.NpgSql
•	Microsoft.Extensions.Caching.StackExchangeRedis
Configuration (appsettings.json)

{
  "ConnectionStrings": {
    "Database": "Host=...;Database=...;Username=...;Password=...",
    "Redis": "localhost:6379"
  },
  "JwtSettings": {
    "Issuer": "Auth.API",
    "Audience": "ClientApp",
    "Key": "SET_IN_USER_SECRETS",
    "ExpiresMinutes": 60
  }
}

Database & Migrations
•	Add Migration:
•	dotnet ef migrations add InitialCreate -p Infrastructure/Infrastructure.csproj -s Auth.API/Auth.API.csproj
•	Update Database:
•	dotnet ef database update -p Infrastructure/Infrastructure.csproj -s Auth.API/Auth.API.csproj
•	Automatic migrations also run at startup.
Authentication Flow
•	Register User → POST /api/Register (Anonymous)
•	Login → POST /api/Login returns { user, token }
•	Include JWT in Authorization header: Bearer <token>
JWT Claims
•	sub (username)
•	email
•	uid
•	jti
Controllers / Endpoints
•	POST /api/Register
{ firstName, lastName, userName, email, password }
•	POST /api/Login
{ userName, password }
•	PUT /api/UpdateUser (Requires Auth)
{ firstName, lastName, userName, email, password }
CQRS Pipeline Behaviors
•	ValidationBehavior
•	LoggingBehavior
Error Handling
•	NotFoundException
•	BadRequestException
•	ValidationException
Using Swagger (OpenAPI)
•	Open Swagger UI.
•	Click Authorize button.
•	Paste JWT token (no 'Bearer' prefix).
•	Invoke secured endpoints.
