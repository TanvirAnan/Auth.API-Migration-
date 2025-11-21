🚀 Auth.API – .NET 8 CQRS + MediatR + JWT Authentication Sample
📌 Overview

Layered (Clean) Architecture: Domain, Application, Infrastructure, API, BuildingBlocks

CQRS with MediatR (Commands, Queries, Pipeline Behaviors)

PostgreSQL (EF Core) with automatic migrations

JWT Authentication with global authorization fallback

Value Objects: UserId, Email

Validation: FluentValidation

Caching: In-memory distributed placeholder

📂 Solution Structure
🧩 Domain

Entities: User

Value Objects: UserId, Email

Interfaces

⚙️ Application

Commands: CreateUser, UpdateUser

Queries: Login

Validators & Handlers

Pipeline Behaviors

🗄️ Infrastructure

EF Core DbContext

Entity Configurations

Repository Implementations

Migrations

🌐 Auth.API

ASP.NET Core entrypoint

Dependency Injection wiring

JWT configuration

Controllers: Register, Login, UpdateUser

🧱 BuildingBlocks

CQRS abstractions

Common exception types

Pipeline behaviors

📦 Key Packages

MediatR

FluentValidation

Microsoft.EntityFrameworkCore

Microsoft.AspNetCore.Authentication.JwtBearer

System.IdentityModel.Tokens.Jwt

🛠️ Configuration (appsettings.json)
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


🔐 Important: Store JwtSettings.Key in User Secrets or environment variables in production.

🗄️ Database & Migrations
Add Migration
dotnet ef migrations add InitialCreate \
  -p Infrastructure/Infrastructure.csproj \
  -s Auth.API/Auth.API/Auth.API.csproj

Update Database
dotnet ef database update \
  -p Infrastructure/Infrastructure.csproj \
  -s Auth.API/Auth.API/Auth.API.csproj


⚙️ Automatic migrations also run at startup.

🔐 Authentication Flow
1️⃣ Register User

POST /api/Register (Anonymous)

2️⃣ Login

POST /api/Login
Returns:

{
  "user": { ... },
  "token": "eyJ..."
}

3️⃣ Use JWT Token

Add header:

Authorization: Bearer <token>


🔒 Protected endpoints require a valid JWT via fallback policy.

🔑 JWT Claims

sub — username

email

uid

jti

🧩 Controllers / Endpoints
POST /api/Register
{
  "firstName": "",
  "lastName": "",
  "userName": "",
  "email": "",
  "password": ""
}

POST /api/Login
{
  "userName": "",
  "password": ""
}

PUT /api/UpdateUser

🔐 Requires Authentication
Uses sub claim (username) to enforce self-update only.

{
  "firstName": "",
  "lastName": "",
  "userName": "",
  "email": "",
  "password": "current password"
}

👤 User Entity

Id (value object UserId, UUID)

FirstName, LastName

UserName

Email (value object with EF conversion)

Password

❗ Stored plaintext → MUST be hashed (PBKDF2, Argon2)

⚡ CQRS Pipeline Behaviors

ValidationBehavior — Runs FluentValidation

LoggingBehavior — Logs execution start/end + duration

❗ Error Handling

Returns ProblemDetails with traceId
Throws:

NotFoundException

BadRequestException

ValidationException

📥 Example Login Request / Response
Request
{
  "userName": "alice",
  "password": "Alice@2025"
}

Response
{
  "user": { ... },
  "token": "eyJ..."
}

🔒 Security Notes

Passwords MUST be hashed (PBKDF2/Argon2)

JWT key should be rotated & stored securely

Consider adding:

Refresh tokens

Role/claim-based authorization

Rate limiting

📘 Swagger Usage

Open Swagger

Click Authorize

Paste JWT token (⚠️ no Bearer prefix)

Call secured endpoints
