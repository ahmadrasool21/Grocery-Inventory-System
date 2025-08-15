# Grocery Inventory System

A clean, layered **.NET 8** backend for managing grocery inventory.  
Includes Products, Categories, Stock Movements, **FluentValidation**, consistent **ProblemDetails** errors, and **JWT auth** (Admin/Clerk).

## Features

- Products & Categories CRUD
- Stock movements: **purchase**, **sale**, **adjust**
- Stock level + basic reports (stock summary, low stock)
- **FluentValidation** + unified **ProblemDetails** error responses
- **JWT** authentication with roles (**Admin**, **Clerk**)
- OpenAPI/Swagger docs
- Unit & API tests (xUnit, WebApplicationFactory)

## Tech Stack

- **Backend:** .NET 8, ASP.NET Core Web API
- **Data:** EF Core (SQLite or SQL Server)
- **Auth:** JWT Bearer (role-based)
- **Docs:** Swagger / OpenAPI
- **Tests:** xUnit, FluentAssertions, FluentValidation.TestHelper

## Project Structure

```
src/
  GroceryInventory.Domain/         # Entities
  GroceryInventory.Application/    # DTOs, abstractions, services, validators
  GroceryInventory.Infrastructure/ # EF Core DbContext & repositories, DI
  GroceryInventory.Api/            # Controllers, Program, Swagger, Auth
tests/
  GroceryInventory.UnitTests/      # Validators & service tests
  GroceryInventory.ApiTests/       # API integration tests
```

---

## Getting Started (Local)

### Prerequisites
- .NET 8 SDK
- (Optional) **dotnet-ef** tools:
  ```bash
  dotnet tool install --global dotnet-ef
  ```

### 1) Restore & build
```bash
dotnet restore
dotnet build
```

### 2) Configure the database
The API supports **SQLite** (recommended for dev) or **SQL Server**.

**SQLite (recommended):**
- Make sure the provider is installed:
  ```bash
  dotnet add src/GroceryInventory.Infrastructure package Microsoft.EntityFrameworkCore.Sqlite
  ```
- In `src/GroceryInventory.Api/appsettings.json` set:
  ```json
  "UseSqlite": true,
  "ConnectionStrings": { "DefaultConnection": "Data Source=grocery.db" }
  ```

**SQL Server (optional):**
```json
"UseSqlite": false,
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=GroceryInventoryDb;Trusted_Connection=True;TrustServerCertificate=True"
}
```

### 3) Create & apply migrations
> The API also applies migrations at startup, but running them once here is handy.

```bash
dotnet ef migrations add InitialCreate   -p src/GroceryInventory.Infrastructure   -s src/GroceryInventory.Api   -o Persistence/Migrations

dotnet ef database update   -p src/GroceryInventory.Infrastructure   -s src/GroceryInventory.Api
```

### 4) Run the API
```bash
cd src/GroceryInventory.Api
dotnet run
```

- Swagger: http://localhost:5000/swagger  
- Health:  http://localhost:5000/health

---

## Authentication

Demo users (replace later with real user store):

| Username | Password    | Role  |
|---------:|-------------|-------|
| `admin`  | `Admin123!` | Admin |
| `clerk`  | `Clerk123!` | Clerk |

**Get a token:**
```bash
curl -X POST http://localhost:5000/api/auth/token   -H "Content-Type: application/json"   -d '{ "username": "admin", "password": "Admin123!" }'
```

Copy `accessToken` from the response.

- **Swagger:** Click **Authorize**, paste the token (no `Bearer` prefix).
- **Postman:** Set header `Authorization: Bearer <token>`.

---

## API Summary

| Method | Path                                 | Role            | Notes |
|-------:|--------------------------------------|-----------------|-------|
| GET    | `/api/products`                      | Clerk or Admin  | List products |
| GET    | `/api/products/{id}`                 | Clerk or Admin  | Get product |
| POST   | `/api/products`                      | **Admin**       | Create |
| PUT    | `/api/products/{id}`                 | **Admin**       | Update |
| DELETE | `/api/products/{id}`                 | **Admin**       | Delete |
| GET    | `/api/categories`                    | Clerk or Admin  | List categories |
| GET    | `/api/categories/{id}`               | Clerk or Admin  | Get category |
| POST   | `/api/categories`                    | **Admin**       | Create |
| PUT    | `/api/categories/{id}`               | **Admin**       | Update |
| DELETE | `/api/categories/{id}`               | **Admin**       | Delete |
| GET    | `/api/stock/level/{productId}`       | Clerk or Admin  | On-hand qty |
| GET    | `/api/stock/movements`               | Clerk or Admin  | Filters: `productId`, `from`, `to` |
| POST   | `/api/stock/purchase`                | **Admin**       | Quantity must be positive |
| POST   | `/api/stock/sale`                    | Clerk or Admin  | Quantity must be positive |
| POST   | `/api/stock/adjust`                  | **Admin**       | If negative, `reason` required |
| GET    | `/api/reports/stock-summary`         | Clerk or Admin  | Optional report |
| GET    | `/api/reports/low-stock`             | Clerk or Admin  | Optional report |
| POST   | `/api/auth/token`                    | Anonymous       | Get JWT |
| GET    | `/health`                            | Anonymous       | Health probe |

Errors are returned as **ProblemDetails** (`application/problem+json`).

---

## Testing

Projects:
- `tests/GroceryInventory.UnitTests` (validators, `StockService`)
- `tests/GroceryInventory.ApiTests` (API endpoints, in-memory DB, test auth)

**Run all:**
```bash
dotnet test
```

> Ensure your `Program.cs` ends with `public partial class Program { }` so `WebApplicationFactory<Program>` works in integration tests.

---

## Postman

Fastest way: import via Swagger.

- Postman → **Import** → **Link** → `http://localhost:5000/swagger/v1/swagger.json`
- Then call `POST /api/auth/token` and set `Authorization: Bearer <token>` on the collection.

---

## Configuration

`appsettings.json` (or environment variables):
- `ConnectionStrings:DefaultConnection` — DB connection string
- `UseSqlite` — `true` to use SQLite, `false` for SQL Server
- `Jwt:Issuer`, `Jwt:Audience`, `Jwt:SecretKey` — JWT settings (use a long random secret in production)

---

## Roadmap

- Oversell protection & FEFO (batch/expiry)
- Pagination/filtering/sorting on list endpoints
- Suppliers & Purchase Orders
- React admin UI (CORS, role-based UI)
- Serilog & request logging
- CI/CD (GitHub Actions)

---

## License

MIT (or your preferred license — add a `LICENSE` file).

## Contributing

1. Fork and create a feature branch  
2. Add/update tests  
3. Run `dotnet test`  
4. Open a PR with a clear description
