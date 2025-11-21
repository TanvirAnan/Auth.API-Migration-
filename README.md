Auth.API – .NET 8 CQRS + MediatR + JWT Authentication Sample
Overview

Layered (Clean) architecture: Domain, Application, Infrastructure, API, BuildingBlocks

Implements CQRS with MediatR (commands/queries + pipeline behaviors)

PostgreSQL (EF Core) with automatic migrations

JWT authentication + global authorization fallback (all endpoints require auth unless [AllowAnonymous])

Value Objects: UserId, Email

Validation: FluentValidation

Caching: In-memory distributed placeholder

Solution Structure
Domain

Entities (User)

Value Objects (UserId, Email)

Interfaces

Application

Commands (CreateUser, UpdateUser)

Queries (Login)

Validators, Handlers

Pipeline Behaviors

Infrastructure

EF Core DbContext

Entity Configurations

Repository Implementations

Migrations

Auth.API

ASP.NET Core entrypoint

Dependency Injection wiring

JWT configuration

Controllers (Register, Login, UpdateUser)

BuildingBlocks

CQRS abstractions

Common exception types

Pipeline behaviors

Key Packages

MediatR

FluentValidation

Microsoft.EntityFrameworkCore

Microsoft.AspNetCore.Authentication.JwtBearer

System.IdentityModel.Tokens.Jwt

Configuration (appsettings.json)
{
  "ConnectionStrings": {
    "Database": "",
    "Redis": ""
  },
  "JwtSettings": {
    "Issuer": "",
    "Audience": "",
    "Key": "SET_IN_USER_SECRETS",
    "ExpiresMinutes": 60
  }
}


Note: Use a strong key in User Secrets or environment variables for production.

Database & Migrations
# Add migration
dotnet ef migrations add InitialCreate \
  -p Infrastructure/Infrastructure.csproj \
  -s Auth.API/Auth.API/Auth.API.csproj

# Update database
dotnet ef database update \
  -p Infrastructure/Infrastructure.csproj \
  -s Auth.API/Auth.API/Auth.API.csproj


Automatic migration also runs at startup.

Authentication Flow

Register user
POST /api/Register (anonymous)

Login
POST /api/Login → returns { user, token }

Use JWT token
Add header:

Authorization: Bearer <token>


Protected endpoints require a valid JWT (enforced by fallback policy).

JWT Claims

sub: username

email

uid

jti

Controllers / Endpoints
POST /api/Register

Body:

{
  "firstName": "",
  "lastName": "",
  "userName": "",
  "email": "",
  "password": ""
}

POST /api/Login

Body:

{
  "userName": "",
  "password": ""
}

PUT /api/UpdateUser

Requires Authentication

Uses sub claim (username) to enforce self-update only

Body:

{
  "firstName": "",
  "lastName": "",
  "userName": "",
  "email": "",
  "password": "current password"
}

User Entity

Id (value object UserId mapped to UUID)

FirstName, LastName

UserName

Email (value object + EF conversion)

Password (currently plaintext, needs hashing → PBKDF2/Argon2 recommended)

CQRS Pipeline Behaviors

ValidationBehavior → runs FluentValidation before executing handlers

LoggingBehavior → logs start/end and execution time

Error Handling

Custom exception handler returns ProblemDetails with traceId

Throws:

NotFoundException

BadRequestException

ValidationException

Example Login Request / Response
Request
POST /api/Login
{
  "userName": "alice",
  "password": "Alice@2025"
}

Response
{
  "user": { ... },
  "token": "eyJ..."
}

Security Notes

Passwords are stored plaintext → MUST be hashed (PBKDF2, Argon2)

JWT key should be rotated & secured (User Secrets / Key Vault)

Consider:

Refresh tokens

Role-based or policy-based authorization

Swagger Usage

Click Authorize

Paste token (no Bearer prefix needed)

Call protected endpoints
