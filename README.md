# Okane (お金)

🔗 **Live app (alpha):** [okane.bloisdev.com](https://okane.bloisdev.com) — early, unstable, may reset/break without notice.

Okane is a personal finance API built around a single, append-only **Ledger**. Instead of storing wallet balances directly, every balance is derived by replaying the transactions recorded against a wallet — money simply doesn't move any other way.

## Domain model

The full definitions live in [`Docs/`](Docs), but in short:

- **[Ledger](Docs/ledger_definition.md)** — the one and only system-wide, append-only record of every transaction. It cannot be edited or deleted, only appended to. Mistakes are corrected with a new, reversing transaction.
- **[Wallet](Docs/wallet_definition.md)** — a named container of value owned by exactly one user (e.g. "Food", "Savings"). Every user also gets one automatic **External Wallet**, representing the boundary between their tracked finances and the outside world (salary, purchases, ATM withdrawals). A wallet's balance is always derived from the ledger, never stored.
- **[Transaction](Docs/transaction_definition.md)** — a strict one-to-one transfer of a positive `Amount` (BRL) from one wallet to another, both belonging to the same user. Immutable once recorded.

## Project layout

The solution is split into a host and independent domain modules, wired together in `Okane.Api`:

| Project | Role |
|---|---|
| `Okane.Api` | ASP.NET Core Web API — composition root, controllers, auth, persistence, error handling, observability. |
| `Okane.User` | User domain and application logic (registration, credential validation). |
| `Okane.Wallet` | Wallet domain and application logic. |
| `Okane.Transaction` | Transaction domain and application logic. |

`Okane.User`, `Okane.Wallet`, and `Okane.Transaction` are plain class libraries with no dependency on ASP.NET Core or Postgres — each exposes a domain model plus application-layer interfaces (repositories, services) that `Okane.Api` implements and wires up via dependency injection.

> **Status:** Auth (`register` / `login` / `me`) is implemented end-to-end against Postgres. The `Wallet` and `Transaction` modules currently define the domain and application interfaces only — their controllers and persistence implementations are still in progress.

## Tech stack

- **.NET 10** / ASP.NET Core Web API
- **PostgreSQL 17** via **Npgsql** (raw SQL, no ORM)
- **JWT bearer authentication** (`Microsoft.AspNetCore.Authentication.JwtBearer`)
- **BCrypt.Net-Next** for password hashing
- **OpenTelemetry** for structured logging (console exporter for now)

## Getting started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Docker (for PostgreSQL)

### 1. Start the database

```bash
docker compose up -d
```

This starts a Postgres 17 container and automatically applies the SQL files in [`Migrations/`](Migrations) on first boot.

### 2. Run the API

```bash
dotnet run --project Okane.Api
```

The API listens on `http://localhost:5091` by default (see `Okane.Api/Properties/launchSettings.json`). The Postgres connection string and JWT settings are configured in `Okane.Api/appsettings.json` / `appsettings.Development.json`.

### 3. Try it out

Sample requests for every endpoint are in [`Okane.Api/Okane.Api.http`](Okane.Api/Okane.Api.http) (usable directly from Visual Studio / VS Code / Rider's HTTP client), for example:

```http
POST http://localhost:5091/api/auth/register
Content-Type: application/json

{
  "name": "Ada Lovelace",
  "email": "ada@example.com",
  "password": "SuperSecret123"
}
```

All responses are wrapped in a consistent envelope (`message`, `status`, `timestamp`, and optional `details`).

## Production configuration

Production configuration is **environment-variable only** — no secrets or environment-specific values are ever committed to `appsettings.*.json`. `appsettings.json` / `appsettings.Development.json` only hold local-development defaults. ASP.NET Core reads environment variables automatically (`__` is the JSON hierarchy separator, e.g. `Jwt__SigningKey` maps to `Jwt:SigningKey`), so no code changes are needed to supply these — they just need to be set on whatever is hosting the app.

Before deploying to Production, all of the following **must** be set:

| Variable | Notes |
|---|---|
| `Jwt__Issuer` | |
| `Jwt__Audience` | |
| `Jwt__SigningKey` | A real secret. Never reuse the value committed in `appsettings.Development.json`. |
| `Jwt__ExpiryMinutes` | |
| `Cors__Origins__0` (add `__1`, `__2`, ... for additional allowed origins) | The real Production frontend origin(s). |
| `ConnectionStrings__Okane` | The real Production Postgres connection string. Never reuse the `okane`/`okane` dev credentials. |

The app fails fast at startup if any of these are missing — this is intentional, not a bug to work around with placeholder defaults.

Optional overrides (safe defaults already exist in `appsettings.json`):

| Variable | Default | When to override |
|---|---|---|
| `Observability__ServiceName` | `Okane.Api` | Rarely needed. |
| `Observability__OtlpEndpoint` | `http://localhost:4317` | Almost always needed in Production — point it at the real otel-collector endpoint. |

## Database migrations

Migrations are plain, numbered SQL files in [`Migrations/`](Migrations), each starting with a `-- migrate:up` marker:

- `0001_create_users_table.sql`
- `0002_create_wallets_table.sql`
- `0003_create_ledger_table.sql`
- `0004_add_owner_id_and_indexes_to_ledger.sql`
- `0005_add_unique_owner_name_to_wallets.sql`

Locally, `docker compose up` applies them in order automatically the first time the Postgres container initializes (bind-mounted at `/docker-entrypoint-initdb.d`) — this only runs once, against an empty data directory.

In Production, the database is Amazon RDS, so instead the same files are applied via [dbmate](https://github.com/amacneil/dbmate) during EC2 instance boot (see [`terraform/templates/user_data.sh.tftpl`](terraform/templates/user_data.sh.tftpl)). dbmate tracks which files have already run in a `schema_migrations` table, so re-running it (including on every instance redeploy) only applies new migrations. To add a migration, drop a new `NNNN_description.sql` file in `Migrations/` starting with `-- migrate:up`.
