# Order Management API

A RESTful API for managing orders, built with ASP.NET Core and Entity Framework Core. The project demonstrates backend fundamentals such as dependency injection, asynchronous database operations, input validation, PostgreSQL persistence, and integration testing against a real database.

## Tech stack

- **C# / .NET 10** and **ASP.NET Core Web API**
- **Entity Framework Core** with **Npgsql**
- **PostgreSQL 17** and **Docker Compose**
- **xUnit**, **Testcontainers for .NET**, and **WebApplicationFactory** for automated tests
- **OpenAPI** for API description in the Development environment

## Features

- Create an order with a customer name and total amount
- Retrieve all orders or a single order by ID
- Update an existing order
- Delete an order
- Validate request data and return appropriate HTTP status codes
- Store orders in PostgreSQL using EF Core migrations

New orders start with a `Pending` status and receive a UTC creation timestamp.

## API endpoints

| Method   | Endpoint           | Description             | Successful response |
| -------- | ------------------ | ----------------------- | ------------------- |
| `GET`    | `/api/orders`      | Retrieve all orders     | `200 OK`            |
| `GET`    | `/api/orders/{id}` | Retrieve an order by ID | `200 OK`            |
| `POST`   | `/api/orders`      | Create an order         | `201 Created`       |
| `PUT`    | `/api/orders/{id}` | Update an order         | `204 No Content`    |
| `DELETE` | `/api/orders/{id}` | Delete an order         | `204 No Content`    |

Requests with invalid order data return `400 Bad Request`. Requests for nonexistent orders return `404 Not Found` where applicable. A successful POST response includes a `Location` header for the new resource.

### Example: Create an order

`POST /api/orders`

```json
{
  "customerName": "Jane Doe",
  "totalAmount": 199.99
}
```

The customer name is required, and the total amount must be greater than zero.

## Project structure

```text
OrderManagement/
├── OrderManagement.Api/
│   ├── Controllers/       # HTTP endpoints
│   ├── Data/              # EF Core DbContext
│   ├── Dtos/              # Request DTOs and validation
│   ├── Migrations/        # EF Core database migrations
│   ├── Models/            # Order entity
│   ├── Services/          # Order operations
│   └── Program.cs         # Application setup and DI
├── OrderManagement.Tests/
│   ├── Controllers/       # HTTP integration tests
│   ├── Fixtures/          # PostgreSQL container and test host setup
│   └── Services/          # Service tests
├── docker-compose.yml
└── OrderManagement.slnx
```

## Getting started

### Prerequisites

- .NET 10 SDK
- Docker Desktop (or another Docker-compatible environment with Docker Compose)
- Optional: Visual Studio with ASP.NET and web development support
- `dotnet-ef` CLI tool for applying migrations (installation instructions below)

### 1. Start PostgreSQL

From the repository root:

```bash
docker compose up -d
```

The development database is available on `localhost:5434`. The credentials in `docker-compose.yml` are **for disposable local development only**; do not reuse them for production or public deployments.

### 2. Configure the connection string

The API reads `ConnectionStrings:DefaultConnection` from ASP.NET Core configuration. For local development, use **User Secrets** instead of committing credentials to source control.

The API project has a User Secrets ID configured, so you only need to set the connection string:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5434;Database=ordermanagement;Username=ordermanagement;Password=devpassword" --project OrderManagement.Api
```

### 3. Apply database migrations

If the `dotnet ef` command is not installed, install the EF Core CLI tool:

```bash
dotnet tool install --global dotnet-ef --version 10.0.12
```

If you already have `dotnet-ef` installed, check its version:

```bash
dotnet ef --version
```

Then apply the existing migrations:

```bash
dotnet ef database update --project OrderManagement.Api --startup-project OrderManagement.Api
```

### 4. Run the API

```bash
dotnet run --project OrderManagement.Api --launch-profile https
```

The included launch profile uses `https://localhost:7019` and `http://localhost:5229`. If your machine has not trusted the local ASP.NET Core development certificate, you may need to run `dotnet dev-certs https --trust`.

The OpenAPI document is exposed in Development at `/openapi/v1.json`. A graphical API explorer is not included by default.

## Running tests

```bash
dotnet test
```

The test suite includes service tests and HTTP integration tests. Integration tests use **Testcontainers** to start an isolated PostgreSQL instance, apply EF Core migrations, and exercise the application against a real database. Docker must be running before you execute the tests.

The current suite contains **13 tests** covering CRUD behavior, persistence, validation, and HTTP responses.

## Design notes

The API uses a simple controller → service → EF Core DbContext structure. Controllers translate application results into HTTP responses; the service handles order operations; and EF Core manages persistence. This keeps responsibilities clear without adding unnecessary abstraction layers for a small project.

The application is a learning and portfolio project, **not a production-ready order processing system**. It does not implement authentication, authorization policies, payments, or order status workflows.
