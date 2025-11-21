# 🚀 Auth.API — .NET 8 CQRS + MediatR + JWT Authentication Sample

A modern, clean, and scalable authentication microservice built using **.NET 8**, following **Clean Architecture**, **CQRS**, **MediatR**, and **JWT Authentication** best practices.

---

## 📌 Overview & Key Features

- **Clean Architecture** (Domain → Application → Infrastructure → API → BuildingBlocks)
- **CQRS** with MediatR (Commands + Queries + Pipeline Behaviors)
- **PostgreSQL** with EF Core + Automatic Migrations
- **JWT Authentication** with global authorization fallback
- **Value Objects**: `UserId`, `Email`
- **Validation** using FluentValidation
- **Caching** (In-memory / Redis placeholder)
- **Pipeline Behaviors**: Validation, Logging
- **Centralized Error Handling** with ProblemDetails

---

## 🧱 Clean Architecture Overview

```
            +-------------------------+
            |        Auth.API         |
            | Controllers, JWT, DI    |
            +------------+------------+
                         |
                         v
            +-------------------------+
            |      Application        |
            | Commands, Queries, CQRS |
            | Handlers, Validators    |
            +------------+------------+
                         |
                         v
            +-------------------------+
            |         Domain          |
            | Entities, ValueObjects  |
            +------------+------------+
                         |
                         v
            +-------------------------+
            |     Infrastructure      |
            | EF Core, Repos, DB      |
            +-------------------------+
```

---

## 📂 Solution Structure

| Layer           | Responsibility                       | Key Contents |
|-----------------|--------------------------------------|--------------|
| **Domain**      | Core business rules & entities       | User, UserId, Email |
| **Application** | CQRS orchestration                   | Commands, Queries, Handlers, Validators |
| **Infrastructure** | Database & services               | EF Core, DbContext, Migrations, Repositories |
| **Auth.API**    | API layer + Startup configuration    | Controllers, JWT, DI |
| **BuildingBlocks** | Shared reusable components        | CQRS abstractions, Behaviors, Exceptions |

---

## 📦 Key Packages Used

- `MediatR`
- `FluentValidation`
- `Microsoft.EntityFrameworkCore`
- `Microsoft.AspNetCore.Authentication.JwtBearer`
- `System.IdentityModel.Tokens.Jwt`
- `AspNetCore.HealthChecks.NpgSql`
- `Microsoft.Extensions.Caching.StackExchangeRedis`

---

## 🛠️ Configuration (appsettings.json)

```json
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
```

---

## 🗄️ Database & Migrations

### Add Migration

```bash
dotnet ef migrations add InitialCreate \
  -p Infrastructure/Infrastructure.csproj \
  -s Auth.API/Auth.API.csproj
```

### Update Database

```bash
dotnet ef database update \
  -p Infrastructure/Infrastructure.csproj \
  -s Auth.API/Auth.API.csproj
```

Automatic migrations also run on startup (development only).

---

## 🔐 Authentication Flow

### 1️⃣ Register

**POST** `/api/Register`

```json
{
  "firstName": "John",
  "lastName": "Doe",
  "userName": "johndoe",
  "email": "john@example.com",
  "password": "SecurePass@123"
}
```

### 2️⃣ Login

**POST** `/api/Login`

```json
{
  "userName": "alice",
  "password": "Alice@2025"
}
```

**Response:**

```json
{
  "user": { "..." },
  "token": "eyJ..."
}
```

### 3️⃣ Access Protected Endpoints

Include JWT in headers:

```
Authorization: Bearer <your-token>
```

---

## 🔑 JWT Claims

- `sub` — Username
- `email` — User email
- `uid` — User ID
- `jti` — Unique Token ID

---

## 🧩 Controllers / Endpoints

- **POST** `/api/Register` (Anonymous) → create user
- **POST** `/api/Login` (Anonymous) → authenticate user
- **PUT** `/api/UpdateUser` (Requires Auth) → self-update only

```json
{
  "firstName": "Updated",
  "lastName": "Name",
  "userName": "newusername",
  "email": "newemail@example.com",
  "password": "current-password"
}
```

---

## 👤 User Entity

- `Id` (Value Object → UUID)
- `FirstName`, `LastName`
- `UserName`
- `Email` (Value Object + EF conversion)
- `Password` ⚠️ plaintext → **must hash in production** (PBKDF2/Argon2)

---

## ⚡ CQRS Pipeline Behaviors

- **ValidationBehavior** → FluentValidation
- **LoggingBehavior** → Logs start/end time

---

## ❗ Error Handling

- Global exception handler returns **ProblemDetails** with `traceId`
- Handled exceptions: `NotFoundException`, `BadRequestException`, `ValidationException`

---

## 📘 Using Swagger (OpenAPI)

1. Open Swagger UI
2. Click **Authorize**
3. Paste JWT token (❌ no "Bearer" prefix)
4. Call secured endpoints

---

## 🚀 Getting Started

1. Clone the repository
2. Update `appsettings.json` with your connection strings
3. Run migrations: `dotnet ef database update`
4. Run the API: `dotnet run --project Auth.API`
5. Navigate to `https://localhost:5274`

---

## 📝 License

This project is licensed under the MIT License.

---

## 🤝 Contributing

Contributions, issues, and feature requests are welcome!
