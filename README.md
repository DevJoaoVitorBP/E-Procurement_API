# E-Procurement API

REST API for procurement workflow management, built with `.NET 10` and layered architecture.

> ⚠️ **Important**
>
> This repository is a **personal study project** for learning and portfolio purposes.
> It is **not intended for production use**.

---

## 📚 Table of Contents

- [Overview](#-overview)
- [Current Status](#-current-status)
- [Tech Stack](#-tech-stack)
- [Project Architecture](#-project-architecture)
- [Getting Started](#-getting-started)
- [Configuration](#-configuration)
- [Run the API](#-run-the-api)
- [Authentication and Roles](#-authentication-and-roles)
- [Main Endpoints](#-main-endpoints)
- [Validation and Error Handling](#-validation-and-error-handling)
- [Swagger and Postman](#-swagger-and-postman)
- [Testing](#-testing)
- [CI Pipeline](#-ci-pipeline)
- [Troubleshooting](#-troubleshooting)
- [Roadmap](#-roadmap)
- [Author](#-author)

---

## 🎯 Overview

The goal of this project is to practice modern API development concepts with clear separation of concerns.

Implemented business flows include:

- User registration and authentication
- JWT-based authorization
- Supplier management
- Purchase request lifecycle:
  - create
  - approve/reject
  - move to procurement
  - history tracking
- Purchase order lifecycle:
  - create
  - send
  - complete
  - cancel

---

## 📊 Current Status

| Item | Status |
|---|---|
| Project Type | Personal study project |
| API Stability | Experimental / educational |
| Swagger | Implemented |
| Postman Collection | Implemented (English) |
| Unit Tests | Implemented |
| API Controller Tests | Implemented |
| HTTP Integration Tests | Implemented |
| CI (GitHub Actions) | Implemented |

---

## 🛠 Tech Stack

- `.NET 10`
- `C# 14`
- `ASP.NET Core Web API`
- `Entity Framework Core`
- `FluentValidation`
- `JWT` (`System.IdentityModel.Tokens.Jwt`)
- `BCrypt.Net-Next`
- `xUnit`

---

## 🏗 Project Architecture

The solution uses a layered approach:

```text
Eprocurement.Domain            -> Entities, enums, interfaces (business core)
Eprocurement.Application       -> Services, contracts, validators, abstractions
Eprocurement.Infraestructure   -> DbContext, repositories, identity implementations
EprocurementApi                -> Controllers and API startup/pipeline
Eprocurement.Tests             -> Unit, API, and integration tests
```

### Main Patterns

- Repository Pattern
- Unit of Work
- Dependency Injection
- DTOs/Contracts
- Layered architecture

---

## 🚀 Getting Started

### Prerequisites

- Windows 10/11 (or compatible environment for `.NET 10`)
- `.NET 10 SDK`
- SQL Server / LocalDB
- Visual Studio 2026 (optional, but recommended)

### Clone repository

```powershell
git clone https://github.com/DevJoaoVitorBP/E-Procurement_API.git
cd C:\Projeto\EprocuremmentApi
```

### Restore dependencies

```powershell
dotnet restore
```

### Create / update database

```powershell
dotnet ef database update --project Eprocurement.Infraestructure/Eprocurement.Infraestructure.csproj --startup-project EprocurementApi/EprocurementApi.csproj
```

---

## ⚙️ Configuration

Set required values in `EprocurementApi/appsettings.json` (or user secrets / environment variables):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=EprocurementDb;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "Jwt": {
    "Key": "your-strong-key-here",
    "Issuer": "EprocurementApi",
    "Audience": "EprocurementApiClient"
  }
}
```

> `Program.cs` requires `Jwt:Key`, `Jwt:Issuer`, and `Jwt:Audience`.

---

## ▶️ Run the API

```powershell
dotnet run --project EprocurementApi/EprocurementApi.csproj
```

Default local URL is typically:

- `https://localhost:7287`

(may vary by local launch profile)

---

## 🔐 Authentication and Roles

JWT Bearer authentication is enabled.

### Supported roles

- `Employee`
- `Manager`
- `Procurement`
- `Admin`

### Basic auth flow

1. Register user (`/api/user/register`)
2. Login (`/api/auth/login`)
3. Use token in header:

```http
Authorization: Bearer <token>
```

---

## 🔌 Main Endpoints

### Auth

- `POST /api/auth/login`
- `GET /api/auth/me`

### User

- `POST /api/user/register`

### Supplier

- `POST /api/supplier/register`
- `GET /api/supplier`
- `GET /api/supplier/{id}`
- `PUT /api/supplier/{id}`
- `PATCH /api/supplier/{id}/status`
- `DELETE /api/supplier/{id}`

### Purchase Request

- `POST /api/purchaserequest`
- `GET /api/purchaserequest`
- `GET /api/purchaserequest/{id}`
- `POST /api/purchaserequest/{id}/approve`
- `POST /api/purchaserequest/{id}/reject`
- `POST /api/purchaserequest/{id}/move-to-procurement`
- `GET /api/purchaserequest/{id}/history`

### Purchase Order

- `POST /api/purchaseorder`
- `GET /api/purchaseorder`
- `GET /api/purchaseorder/{id}`
- `PATCH /api/purchaseorder/{id}/send`
- `PATCH /api/purchaseorder/{id}/complete`
- `PATCH /api/purchaseorder/{id}/cancel`

---

## ✔️ Validation and Error Handling

- Request validation is implemented with `FluentValidation`
- Controller actions map business exceptions to HTTP responses:
  - `KeyNotFoundException` → `404 Not Found`
  - `InvalidOperationException` → `400 Bad Request`

Typical error payload format:

```json
{
  "message": "Descriptive error message"
}
```

---

## 🧭 Swagger and Postman

### Swagger UI

Available at:

- `/swagger`

Includes:

- endpoint documentation
- request/response contracts
- JWT Bearer scheme

### Postman

Collection file:

- `postman/EprocurementApi.postman_collection.json`

It contains the full recommended execution flow (Auth, User, Supplier, Purchase Request, Purchase Order).

---

## 🧪 Testing

Test project:

- `Eprocurement.Tests`

Test categories implemented:

- Domain unit tests
- Application service unit tests
- Validator tests
- API controller tests
- Integration tests (`WebApplicationFactory`)

Run tests:

```powershell
dotnet test Eprocurement.Tests/Eprocurement.Tests.csproj
```

---

## ⚙️ CI Pipeline

GitHub Actions workflow:

- `.github/workflows/ci.yml`

Pipeline steps:

1. Restore
2. Build (Release)
3. Test

---

## 🐛 Troubleshooting

### 1) `Jwt:Key is not configured`

Set JWT values in `appsettings.json` or environment variables.

### 2) Database errors / missing schema

Run migrations update command again:

```powershell
dotnet ef database update --project Eprocurement.Infraestructure/Eprocurement.Infraestructure.csproj --startup-project EprocurementApi/EprocurementApi.csproj
```

### 3) `FileLoadException (0x800711C7)` / App Control blocking DLL

If Windows policy blocks `EprocurementApi.dll` during run/tests:

```powershell
Get-ChildItem "C:\Projeto\EprocuremmentApi" -Recurse | Unblock-File
```

If needed, close running API process before rebuild/test.

---

## 🗺 Roadmap

Planned next study steps:

- Rate limiting
- Structured logging
- Docker support
- Additional integration scenarios
- Documentation refinements

---

## ✍️ Author

**João Vitor Batalha Pereira**

- GitHub: [DevJoaoVitorBP](https://github.com/DevJoaoVitorBP)
- LinkedIn: [devjoaopereira](https://www.linkedin.com/in/devjoaopereira)

---

## 📌 Final Note

This repository is maintained as a **learning project** to practice backend architecture, clean layering, testing, and API design with modern `.NET`.
