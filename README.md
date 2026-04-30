# Risk Management System API

Backend API for managing organizations, users, assets, risks, incidents, action plans, and risk assessments in a multi-tenant setup.

Repository: [m-bengtsson/risk-management-system](https://github.com/m-bengtsson/risk-management-system)

## What This Project Does

This API is designed to support a practical end-to-end risk management workflow, not just isolated CRUD operations.

It helps teams move from risk visibility to risk treatment:

- Establish context with organizations, users, and assets
- Identify and document risks with clear ownership
- Assess each risk with structured factors (`Likelihood`, `Impact`) and calculated score
- Document incidents and connect real events back to risk governance
- Create and track action plans tied to either a risk or an incident
- Maintain role-based control and tenant boundaries so each organization sees only its own data by default

This creates one system of record for risk decisions, mitigation follow-up, and compliance evidence.

## Tech Stack

- .NET 9 (`net9.0`) with ASP.NET Core Web API
- Entity Framework Core 9
- MySQL via `Pomelo.EntityFrameworkCore.MySql`
- JWT Bearer authentication
- Swagger / OpenAPI in development

## Project Structure

```text
risk-management-system/
├── Controllers/                      # API endpoints and authorization rules
│   ├── AuthController.cs
│   ├── OrganizationController.cs
│   ├── UserController.cs
│   ├── AssetController.cs
│   ├── RiskController.cs
│   ├── IncidentController.cs
│   ├── ActionPlanController.cs
│   └── RiskAssessmentController.cs
├── Data/
│   └── RiskManagementDbContext.cs    # EF Core mappings, constraints, relationships
├── Dtos/                             # Request/response contracts and validation
│   ├── Auth/
│   ├── User/
│   ├── Organization/
│   ├── Asset/
│   ├── Risk/
│   ├── Incident/
│   ├── ActionPlan/
│   └── RiskAssessment/
├── Models/                           # Domain entities
├── Services/                         # Tokens, passwords, claims, seed data
├── Migrations/                       # EF Core migration history
├── Properties/
│   └── launchSettings.json           # Local launch profiles and URLs
├── Program.cs                        # Depency Injection, auth, CORS, middleware, startup
├── appsettings.json                  # Base configuration and seed defaults
├── appsettings.Development.json
├── RiskManagement.csproj
└── RiskManagement.sln
```

## Local Development Setup

Follow the steps below from the repository root.

### 1) Install prerequisites

Required:

- .NET SDK 9
- MySQL 8+ (or compatible)
- EF Core CLI (`dotnet-ef`)

Install `dotnet-ef`:

macOS/Linux/Windows:

```
dotnet tool install --global dotnet-ef
```

Verify:

```
dotnet --version
dotnet ef --version
```

### 2) Create local database

Create a MySQL database (example name: `risk_management_system`):

```
mysql -u root -p -e "CREATE DATABASE IF NOT EXISTS risk_management_system;"
```

### 3) Configure secrets and settings

Recommended approach: use .NET user secrets for local development.

Initialize (one-time):

```
dotnet user-secrets init
```

Set required values:

```
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "server=localhost;port=3306;database=risk_management_system;user=root;password=YOUR_PASSWORD;"
dotnet user-secrets set "Jwt:Secret" "REPLACE_WITH_A_LONG_RANDOM_SECRET_AT_LEAST_32_BYTES"
```

Important note:
If you are using your own/personal/company identity setup, you SHOULD set (and override) these via user-secrets or environment variables (don’t rely on the repo defaults):

```
dotnet user-secrets set "Jwt:Issuer" "YOUR_COMPANY_OR_ENV_ISSUER"
dotnet user-secrets set "Jwt:Audience" "YOUR_COMPANY_OR_ENV_AUDIENCE"
```

### 4) Apply migrations

```bash
dotnet ef database update
```

### 5) Run API

```
dotnet run
```

Default development URLs:

- `http://localhost:5263`
- `https://localhost:7231`

### 6) Verify startup

1. Open Swagger: `https://localhost:7231/swagger`
2. Call `POST /api/auth/login` with a seeded user
3. Copy access token from response
4. Use Swagger `Authorize` with `<access_token>`
5. Test `GET /api/users/me`

## Seed Data

On startup, `AppSeedService` seeds one demo organization and three users (if enabled):

- `demo.user@example.com` (`User`)
- `demo.admin@example.com` (`Admin`)
- `demo.superadmin@example.com` (`Superadmin`)

Default passwords are configured in `appsettings.json` under `SeedData:*`.

Important:

- Change seeded passwords before using outside local development
- Toggle seeding with `SeedData:Enabled`
- Existing seeded users can be updated on startup via `SeedData:UpdateExistingUsers`

## Domain Overview

Core entities and relationships:

- `Organization` has many `Users`, `Assets`, `Risks`, `Incidents`, and `ActionPlans`
- `User` belongs to one `Organization`, has a `Role`, and can own risks/action plans
- `Asset` belongs to one `Organization` and can be linked to many `Risks`
- `Risk` belongs to one `Organization`, can reference an `OwnerUser` and an `Asset`, and has many `ActionPlans` and `RiskAssessments`
- `Incident` belongs to one `Organization`, has a reporting user, and can have many `ActionPlans`
- `ActionPlan` belongs to one `Organization` and must reference exactly one of `RiskId` or `IncidentId`
- `RiskAssessment` belongs to one `Organization` and one `Risk`, records assessor and scoring details
- `RefreshToken` stores hashed refresh tokens per user for session rotation/revocation

## Authentication and Authorization

### Session Model

- Access token: JWT signed with `Jwt:Secret`
- Refresh token: random token stored as SHA-256 hash in DB
- Refresh token is returned as `HttpOnly` cookie named `refreshToken`
- Refresh rotates token chain and revokes old token

### Roles

- `User`: normal authenticated user
- `Admin`: elevated organization-level management
- `Superadmin`: elevated platform role used for global and restricted operations

### Policies

- `AnyAuthenticatedUser` (default/fallback): every endpoint requires auth unless explicitly `[AllowAnonymous]`
- `AdminOrSuperadmin`: admin-level operations
- `SuperadminOnly`: sensitive operations

### Claims Used By API

- user id (`NameIdentifier` / `sub`)
- role (`role`)
- organization id (`organizationId`)

`ICurrentUserService` reads these claims and enforces organization scoping in controllers.

## API Endpoints (High-Level)

Base URL in development:

- `http://localhost:5263`
- `https://localhost:7231`

Swagger UI (development):

- `https://localhost:7231/swagger`

### Auth (`/api/auth`)

- `POST /register` - create user (Admin or Superadmin)
- `POST /login` - authenticate and issue tokens
- `POST /refresh` - rotate refresh token and issue new access token
- `POST /logout` - revoke refresh token session

### Organizations (`/api/organization`)

- `GET /` - Superadmin gets all organizations; others get own organization only
- `POST /` - Superadmin only
- `PUT /{id}` - Superadmin only


### Users (`/api/users`)

- `GET /` - Admin/Superadmin
- `GET /{id}` - Admin/Superadmin (org-scoped unless Superadmin)
- `GET /me` - current authenticated user
- `PUT /{id}` - self update for User, broader update for Admin/Superadmin


### Assets (`/api/asset`)

- `GET /`, `GET /{id}` - authenticated users (org-scoped unless Superadmin)
- `POST /` - Admin/Superadmin
- `PUT /{id}` - Admin/Superadmin


### Risks (`/api/risks`)

- `GET /` - list risks in caller org (supports `status` and `ownerUserId` filters)
- `GET /{id}` - risk by id in caller org
- `POST /` - create risk in caller org 
- `PUT /{id}` - update risk in caller org
- `PATCH /{id}/status` - update only status
  
 Important Validation Rules

- The risk status will be "open" initially, it can not be "In progress" or "Mitigated". It can have "In progress" and "Mitigated" status only after the risk assessment is   done (It can be done in update or patch endpoint).


### Incidents (`/api/incidents`)

- `GET /`, `GET /{id}` - org-scoped read
- `POST /` - create incident in caller org, reporter = authenticated user
- `PUT /{id}` - update incident in caller org


### Action Plans (`/api/action-plans`)

- `GET /`, `GET /{id}` - org-scoped read
- `POST /` - Admin/Superadmin
- `PUT /{id}` - Admin/Superadmin


Important validation rule:

- Must provide exactly one of `RiskId` or `IncidentId` (XOR)

### Risk Assessments (`/api/risk-assessment`)

- `GET /`, `GET /{id}` - org-scoped read
- `POST /` - create assessment for org risk
- `PUT /{id}` - update assessment


Behavior notes:

- `RiskScore` is recalculated as `Likelihood * Impact`
- `EconomicalLoss` is validated to `Low`, `Medium`, or `High`

## Request Validation and Data Rules

Validation is implemented with DTO attributes and controller-level checks.

Examples:

- Email format validation on auth/user DTOs
- Min-length constraints for names/passwords
- Range validation for risk assessment `Likelihood`/`Impact` (1-5)
- Regular expression validation for `EconomicalLoss` is validated to `Low`, `Medium`, or `High`
- Cross-entity organization checks (for example owner/risk/asset must belong to the same organization)

## Multi-Tenancy Behavior

The API enforces organization boundaries through claims and query filters:

- Most list/read/update operations are constrained to caller organization
- All endpoints allow Superadmin cross-organization reads

## Development Notes

- Swagger/OpenAPI is enabled in `Development` environment
- CORS policy name: `FrontendCors`
- OpenAPI security scheme expects Bearer token in `Authorization` header

## Known Gaps / Future Improvements

- No automated test project is currently included in the repository

## License

No explicit license file is currently present in the repository. Add one if this project will be shared or reused publicly.
