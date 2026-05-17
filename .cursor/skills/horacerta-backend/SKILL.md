---
name: horacerta-backend
description: Builds the HoraCerta API with .NET 8, DDD, Minimal APIs, EF Core, JWT, and domain handlers. Use for HoraCerta.Api endpoints, handlers, migrations, Docker, or Contratos. Always read docs/backend/spec.md first.
---

# HoraCerta Backend

## Before coding

1. Read [docs/backend/spec.md](../../../docs/backend/spec.md) (source of truth).
2. Domain rules: [docs/docs.md](../../../docs/docs.md), [docs/agregados.md](../../../docs/agregados.md).
3. Front contracts: mirror changes in `docs/frontend/spec.md` / `horacerta-web` DTOs.

## Stack (non-negotiable)

| Use | Do not use |
|-----|------------|
| .NET 8, Minimal APIs | Controllers unless justified |
| Handlers + Commands/Queries in `HoraCerta.Aplicacao` | Business logic in endpoints |
| EF Core + repositórios em `HoraCerta.Infaestrutura` | EF em `Dominio` |
| Records em `HoraCerta.Api/Contratos` | Anonymous objects in responses |
| JWT + filters `Proprietario*AuthorizationFilter` | Auth logic only in endpoints |

## Layer layout

```
HoraCerta.Api/           → Endpoints, Contratos, Autenticacao, Program
HoraCerta.Aplicacao/     → Handlers, Commands, Queries, Integracao
HoraCerta.Dominio/       → Entidades, estados, eventos (sem infra)
HoraCerta.Infaestrutura/ → Persistencia, Repositorio, Lembretes
```

## Implementation flow

1. Domain rule / aggregate change in `HoraCerta.Dominio`.
2. Command + Handler in `HoraCerta.Aplicacao`.
3. Register handler in `HoraCerta.Api/Extensions/DependencyInjection.cs`.
4. Endpoint in `HoraCerta.Api/Endpoints/` + request/response in `Contratos/`.
5. Tests: unit (`HoraCerta.Testes.Unitarios`), E2E (`HoraCerta.Testes.E2e`).

## Auth rules (MVP)

- Mutations do proprietário: JWT + `ProprietarioId` no body alinhado ao claim `sub`.
- `POST /api/agendamentos/iniciar` e avaliar: públicos (sem JWT).
- Swagger: Development e ambiente `Docker`.

## Persistence

- Dev local: SQLite + `Database:Provider=Sqlite` + `Migrate()`.
- Docker: PostgreSQL + `Database:Provider=PostgreSQL` + `Migrate()`.
- Agregados Proprietário/Cliente: JSON em `Conteudo`.

## Prohibited

- Lógica de domínio em endpoints (apenas orquestração).
- Bypass dos filters de proprietário em rotas protegidas.
- `EnsureCreated` para evolução de schema (usar migrations).

## Docker

- `docker compose up --build` na raiz; secrets em `.env` (copiar de `.env.example`).
