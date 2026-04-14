# E-Procurement API

A robust and scalable API for managing procurement processes (e-procurement), developed with .NET 10, layered architecture and enterprise patterns.

## 📋 Table of Contents

- [Overview](#overview)
- [Project Status](#project-status)
- [Technologies and Dependencies](#technologies-and-dependencies)
- [System Requirements](#system-requirements)
- [Architecture](#architecture)
- [Installation and Setup](#installation-and-setup)
- [Project Structure](#project-structure)
- [Features](#features)
- [API Endpoints](#api-endpoints)
- [Authentication and Authorization](#authentication-and-authorization)
- [Database](#database)
- [Dependency Injection](#dependency-injection)
- [Validation](#validation)
- [Error Handling](#error-handling)
- [Security](#security)
- [How to Use](#how-to-use)
- [Roadmap](#roadmap)

## 🎯 Overview

The **E-Procurement API** is a personal project under development for automating and managing corporate procurement processes. The goal is to build a robust solution with the following features:

- **User Management**: Registration, authentication and role control
- **Secure Authentication**: JWT (JSON Web Tokens) implementation with encryption
- **Purchase Orders**: Creation, listing and management of purchase orders
- **Suppliers**: Registration and management of suppliers
- **Purchase History**: Complete transaction tracking
- **Approval Control**: Multi-step approval system with status

## 📊 Project Status

| Status | Description |
|--------|-----------|
| **Project** | 🔄 Under Development |
| **Version** | v1.0.0 (Beta) |
| **Last Updated** | March 2026 |
| **Stability** | ⚠️ Experimental |

### Implemented Features ✅
- ✅ JWT Authentication
- ✅ User management with BCrypt
- ✅ Layered architecture foundation
- ✅ Role control (Admin, Procurement, Supplier, Finance)

### Features Under Development 🚧
- 🚧 Complete CRUD for Purchase Orders
- 🚧 Supplier Management
- 🚧 Approval System
- 🚧 Purchase History

### Planned Features 📝
- 📝 Unit and integration tests
- 📝 Swagger/OpenAPI documentation
- 📝 Rate Limiting
- 📝 Structured logging
- 📝 Docker containerization
- 📝 CI/CD Pipeline

## 🛠️ Technologies and Dependencies

### Main Stack
- **.NET**: 10.0
- **C#**: 14.0
- **Visual Studio**: 2026 (Community Edition)
- **SQL Server**: LocalDB

### Libraries and Frameworks
| Package | Function |
|---------|----------|
| **AspNetCore** | Web framework for APIs |
| **Entity Framework Core** | ORM for data access |
| **FluentValidation** | Model and request validation |
| **BCrypt.Net** | Secure password hashing |
| **JWT (System.IdentityModel.Tokens.Jwt)** | Token generation and validation |
| **Migrations** | Database version control |

## ✅ System Requirements

### Development
- Windows 10/11
- Visual Studio Community 2026 or higher
- .NET 10 SDK
- SQL Server LocalDB (installed with Visual Studio)

### Runtime
- .NET 10 Runtime
- SQL Server (compatible with the used database)

## 🏗️ Architecture

The project follows a **well-defined layered architecture**:

```
EprocuremmentApi/
├── Domain/                          # Domain Layer (Entities)
│   ├── Entities/                   # Business classes
│   │   ├── User.cs
│   │   ├── BaseEntity.cs
│   │   └── ApprovalStep.cs
│   ├── Enums/                      # Enumerations
│   │   └── ApprovalStatusEnum.cs
│   └── Interfaces/                 # Contracts
│       ├── IUserRepository.cs
│       ├── ISupplierRepository.cs
│       ├── IPurchaseOrderRepository.cs
│       └── IUnitOfWork.cs
│
├── Application/                     # Application Layer (Logic)
│   ├── Services/                   # Business services
│   │   ├── UserService.cs
│   │   ├── AuthService.cs
│   │   ├── SupplierService.cs
│   │   ├── PurchaseRequestService.cs
│   │   └── PurchaseOrderService.cs
│   ├── Abstractions/               # Service interfaces
│   │   ├── IUserService.cs
│   │   ├── IAuthService.cs
│   │   └── ...
│   ├── Contracts/                  # DTOs and contracts
│   │   ├── UserContracts.cs
│   │   ├── AuthContracts.cs
│   │   └── ...
│   ├── Validators/                 # FluentValidation validators
│   │   └── AuthValidators.cs
│   └── DependencyInjection.cs      # DI configuration
│
├── Infrastructure/                  # Infrastructure Layer
│   ├── Persistence/
│   │   ├── Repositories/          # Repository implementation
│   │   ├── ProcuremmentDbContext.cs # EF Core context
│   │   └── Migrations/            # Migration history
│   ├── Identity/
│   │   ├── BcryptPasswordHasher.cs # Password encryption
│   │   └── JwtTokenService.cs     # JWT token generation
│   └── DependencyInjection.cs      # DI configuration
│
└── EprocuremmentApi/               # Presentation Layer (API)
    ├── Controllers/                # Controllers
    │   ├── UserControllers.cs
    │   ├── AuthController.cs
    │  
    ├── Program.cs                  # Application setup
    ├── appsettings.json           # Configuration
    └── EprocuremmentApi.http      # Test requests
```

### Patterns Used

1. **Repository Pattern**: Data access abstraction
2. **Unit of Work Pattern**: Transaction coordination
3. **Dependency Injection**: Inversion of control
4. **DTO (Data Transfer Objects)**: Data separation
5. **Layered Architecture**: Separation of concerns

## 📦 Installation and Setup

### 1. Clone the Repository

```powershell
git clone https://github.com/seu-usuario/eprocurement-api.git
cd C:\Projeto\EprocurementApi
```

### 2. Open in Visual Studio

```powershell
# Open the solution
Start-Process "C:\Projeto\EprocurementApi\EprocuremmentApi.sln"
```

### 3. Restore Dependencies

```powershell
# In Visual Studio Package Manager Console
Update-Package
dotnet restore
```

### 4. Configure the Database

```powershell
# In Package Manager Console
Add-Migration InitialCreate
Update-Database
```

Or via CLI:

```powershell
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 5. Configure appsettings.json

The `appsettings.json` file already contains default settings:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=(localdb)\\MSSQLLocalDB;Integrated Security=True;..."
  },
  "Jwt": {
    "Key": "f5e8d7c3b2a1e9d8f7c6b5a4e3d2f1c8b7a6e5d4f3c2b1a9e8d7f6c5b4a3e2d1c",
    "Issuer": "EprocurementApi",
    "Audience": "EprocurementApiClient"
  }
}
```

### 6. Run the Application

```powershell
# Via Visual Studio
F5 (or Debug > Start Debugging)

# Via CLI
dotnet run --project EprocuremmentApi
```

The API will be available at: `https://localhost:5001`

## 📂 Project Structure

### Domain Layer (`Eprocurement.Domain`)
Defines business entities and interfaces:

- **User.cs**: User entity with roles and password encryption
- **BaseEntity.cs**: Base class for all entities
- **ApprovalStep.cs**: Purchase approval steps
- **Enums**: ApprovalStatusEnum, UserRolesEnum, etc.

### Application Layer (`Eprocurement.Application`)

Contains business logic:

- **Services**: UserService, AuthService, SupplierService, etc.
- **Abstractions**: Service interfaces
- **Contracts**: DTOs for requests and responses
- **Validators**: FluentValidation validations

### Infrastructure Layer (`Eprocurement.Infrastructure`)

Technical implementations:

- **Repositories**: Repository pattern implementation
- **DbContext**: EF Core entity mapping
- **Identity**: Authentication and encryption services
- **Migrations**: Database change history

### Presentation Layer (`EprocurementApi`)

API exposure:

- **Controllers**: REST endpoints
- **Program.cs**: Middleware and DI configuration
- **appsettings.json**: Application configuration

## ⚙️ Features

### 1. User Management

- ✅ New user registration
- ✅ Duplicate email validation
- ✅ Secure password encryption with BCrypt
- ✅ Role control (Admin, Procurement, Supplier)
- ✅ User activation/deactivation

### 2. Authentication and Security

- ✅ Login with JWT generation
- ✅ Configurable token expiration
- ✅ Issuer and audience validation
- ✅ Refresh tokens (structure prepared)
- ✅ Authentication in protected endpoints

### 3. Purchase Orders

- ✅ Purchase order creation
- ✅ Order listing and queries
- ✅ Supplier validation
- ✅ Status tracking

### 4. Supplier Management

- ✅ Supplier registration
- ✅ Contact information management
- ✅ Activity control

### 5. Purchase History

- ✅ Complete transaction tracking
- ✅ Change audit

### 6. Approval System

- ✅ Configurable approval steps
- ✅ Approval status (Pending, Approved, Rejected)

## 🔌 API Endpoints

### Authentication

#### Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "password123"
}
```

**Response (200 OK)**:
```json
{
    "token": "eyJhbGciOiJIUzI1N...",
    "name": "User",
    "email": "user@example.com",
    "role": "Employee"
}
```

### Users

#### Register New User
```http
POST /api/users/register
Content-Type: application/json

{
  "name": "John Silva",
  "email": "john@example.com",
  "password": "password123",
  "role": 1
}
```

**Response (201 Created)**:
```json
```

**Errors (400 Bad Request)**:
```json
{
  "message": "Email already exists."
}
```

### Purchase Orders

Coming soon...

## 🔐 Authentication and Authorization

### Authentication Flow

```
1. User logs in with email and password
                    ↓
2. API validates credentials
                    ↓
3. API generates JWT signed with secret key
                    ↓
4. User includes token in header: Authorization: Bearer <token>
                    ↓
5. API validates token in each protected request
```

### JWT Configuration

**appsettings.json**:
```json
{
  "Jwt": {
    "Key": "your-secret-key-256-bits",
    "Issuer": "EprocurementApi",
    "Audience": "EprocurementApiClient",
    "ExpirationInMinutes": 60
  }
}
```

### Role-Based Access Control

```csharp
[Authorize(Roles = "Admin,Procurement")]
[HttpGet]
public async Task<ActionResult> GetAll(CancellationToken cancellationToken)
{
    // Only Admin and Procurement can access
}
```

### Available Roles

| Role | Permissions |
|------|-----------|
| **Admin** | Full API access |
| **Procurement** | Manage purchase orders |
| **Supplier** | View and respond to RFQs |
| **Finance** | Approve orders |

## 🗄️ Database

### Context (DbContext)

File: `Eprocurement.Infrastructure\Persistence\ProcurementDbContext.cs`

### Main Tables

| Table | Description |
|-------|-----------|
| **User** | System users |
| **Supplier** | Suppliers/Vendors |
| **PurchaseOrder** | Purchase orders |
| **PurchaseOrderItem** | Order items |
| **ApprovalStep** | Approval steps |
| **PurchaseHistory** | Purchase history |

### Migrations

Migrations are located in `Eprocuremment.Infrastructure\Migrations\`:

```powershell
# Create new migration
dotnet ef migrations add MigrationName

# Apply migrations
dotnet ef database update

# Revert last migration
dotnet ef database update PreviousMigrationName
```

## 💉 Dependency Injection

### Configuration in Program.cs

```csharp
// Infrastructure services
builder.Services.AddInfrastructure(builder.Configuration);

// Application services
builder.Services.AddApplication();
```

### Main Registrations

**Application DependencyInjection.cs**:
- `IUserService` → `UserService`
- `IAuthService` → `AuthService`
- `IPurchaseOrderService` → `PurchaseOrderService`
- etc.

**Infrastructure DependencyInjection.cs**:
- `IUserRepository` → `UserRepository`
- `IUnitOfWork` → `UnitOfWork`
- `IPasswordHasher` → `BcryptPasswordHasher`
- `ITokenService` → `JwtTokenService`

## ✔️ Validation

### FluentValidation

Validations are defined in `Eprocuremment.Application\Validators\`:

```csharp
public class LoginValidator : AbstractValidator<LoginRequest>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters");
    }
}
```

### Validation Responses

```json
{
  "message": "Validation failed.",
  "errors": {
    "Email": ["Email is required"],
    "Password": ["Password must be at least 6 characters"]
  }
}
```

## 🚨 Error Handling

### Error Response Pattern

```json
{
  "message": "Error description",
  "timestamp": "2026-03-20T10:30:00Z",
  "statusCode": 400
}
```

### HTTP Status Codes Used

| Code | Meaning |
|------|---------|
| **200** | OK - Request successful |
| **201** | Created - Resource created successfully |
| **400** | Bad Request - Invalid data |
| **401** | Unauthorized - Authentication failure |
| **403** | Forbidden - Access denied |
| **404** | Not Found - Resource not found |
| **500** | Internal Server Error - Server error |

### Error Handling in Controllers

```csharp
try
{
    var result = await _userService.RegisterAsync(request, cancellationToken);
    return CreatedAtAction(nameof(Register), result);
}
catch (InvalidOperationException ex)
{
    return BadRequest(new { message = ex.Message });
}
catch (Exception ex)
{
    return StatusCode(500, new { message = "Internal server error" });
}
```

## 🔒 Security

### Security Implementations

✅ **Password Encryption**: BCrypt with automatic salt
✅ **JWT**: Tokens with HMAC-SHA256 signature
✅ **HTTPS**: Automatic redirection (except development)
✅ **Input Validation**: FluentValidation on all requests
✅ **Authorization**: Role-based access control
✅ **CORS**: Configurable via appsettings

<!--
### Production Security Checklist

> ⚠️ **Warning**: This project is still under development. The items below are recommendations for when used in production.

- [ ] Change JWT key in production
- [ ] Use HTTPS in production
- [ ] Configure CORS appropriately
- [ ] Implement rate limiting
- [ ] Add logging and monitoring
- [ ] Use secrets manager for credentials
- [ ] Enable database audit
- [ ] Implement CSRF validation
- [ ] Add SQL Injection protection
- [ ] Perform penetration testing
-->
## 🗺️ Roadmap

### Q1 2026 (Current)
- [x] Initial project setup
- [x] Layered architecture
- [x] JWT authentication
- [ ] User CRUD
- [ ] Basic unit tests

### Q2 2026
- [ ] Complete Purchase Order CRUD
- [ ] Supplier Management
- [ ] Approval System
- [ ] Integration tests

### Q3 2026
- [ ] Swagger/OpenAPI
- [ ] Rate Limiting
- [ ] Structured logging
- [ ] Advanced error handling

### Q4 2026
- [ ] Docker & Container
- [ ] Complete documentation
<!--
## 🚀 Deploy

### Production Preparation

```powershell
# 1. Build in Release mode
dotnet build -c Release

# 2. Publish
dotnet publish -c Release -o .\publish

# 3. Configure environment variables
$env:Jwt__Key = "your-secure-secret-key"
$env:ConnectionStrings__DefaultConnection = "server=prod-db;..."
```
-->
## 🐛 Troubleshooting

### Error: "Jwt:Key not configured"

**Cause**: appsettings.json doesn't have JWT key

**Solution**:
```json
{
  "Jwt": {
    "Key": "your-secret-key-256-bits",
    "Issuer": "EprocurementApi",
    "Audience": "EprocurementApiClient"
  }
}
```

### Error: "Database does not exist"

**Cause**: Migrations not applied

**Solution**:
```powershell
dotnet ef database update
```

### Error: "Invalid token"

**Cause**: JWT token expired or was modified

**Solution**: Login again to get a new token

## 📚 External References

- [.NET 10 Documentation](https://learn.microsoft.com/dotnet/)
- [Entity Framework Core](https://learn.microsoft.com/ef/core/)
- [FluentValidation](https://docs.fluentvalidation.net/)
- [JWT.io](https://jwt.io/)
- [BCrypt.Net](https://www.nuget.org/packages/BCrypt.Net-Core/)

## ✍️ Author

**Developed by:** João Vitor Batalha Pereira  
**GitHub:** [DevJoaoVitorBP](https://github.com/DevJoaoVitorBP)  
**Project Type:** Personal | Portfolio

> 💡 This is a personal learning project for developing skills in .NET, clean architecture and enterprise patterns.

## 🤝 Contributions

This is a personal project, but suggestions and feedback are welcome! Feel free to:
- Report issues
- Suggest improvements
- Share feedback

## 📞 Contact

For questions, suggestions or feedback:
- Open an issue in the repository
- Contact via [João Vitor Batalha Pereira](https://www.linkedin.com/in/devjoaopereira)

---

**Last Updated**: March 2026  
**API Version**: 1.0.0 (Beta)  
**Status**: 🔄 Under Development
