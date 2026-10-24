# Behsa — Account and Transaction Management

A web application with an ASP.NET Core 8 backend and Angular 18 frontend. The backend includes account and transaction services, profile management, user roles, and CSV handling.

## Structure

- `behsa-backend/src/Domain/`: entities and roles.
- `behsa-backend/src/Application/`: DTOs, interfaces, services, and CSV utilities.
- `behsa-backend/src/Infrastructure/`: PostgreSQL persistence and repositories.
- `behsa-backend/src/Web/`: controllers and application startup.
- `behsa-backend/test/`: xUnit tests.
- `behsa-frontend/project/`: Angular client.

## Backend setup

Requirements: .NET 8 SDK and PostgreSQL.

Configure `ConnectionStrings:DefaultConnection`, `JwtSettings:Key`, and `RootUser:Password` using environment variables or .NET user secrets. Use a strong, unique JWT signing key and administrator password. Review the remaining `RootUser` and JWT options in `src/Web/appsettings.json`.

```sh
cd behsa-backend
dotnet restore
dotnet build
dotnet test
dotnet run --project src/Web/Web.csproj
```

Startup initializes seed data. Review database migration requirements and `src/Web/Startup/` before connecting to an existing database.

## Frontend setup

From a separate terminal:

```sh
cd behsa-frontend/project
npm ci
npm start
```

Review the frontend API URL and backend CORS settings before connecting the client. The Angular development server normally uses `http://localhost:4200`; use the addresses printed by the two processes.

## Configuration

Docker and Compose files are provided under `behsa-backend/`. Inspect their database, port, and environment settings before use. Committed development configuration must not be used as production credentials.

For a local PowerShell session, set `$env:ConnectionStrings__DefaultConnection`, `$env:JwtSettings__Key`, and `$env:RootUser__Password` before starting the backend. In Docker Compose, supply `POSTGRES_PASSWORD`, `JWT_SIGNING_KEY`, and `ROOT_USER_PASSWORD`; the configuration requires those values rather than providing committed passwords.
