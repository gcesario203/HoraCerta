# HoraCerta

Sistema de agendamento para estabelecimentos — API .NET 8 + portal Next.js.

## Arquitetura

```mermaid
flowchart LR
  Browser[Browser]
  Web[horacerta-web :3000]
  API[HoraCerta.Api :5080]
  DB[(PostgreSQL)]

  Browser --> Web
  Web -->|BFF /api/bff| Web
  Web -->|rewrite /api/core| API
  API --> DB
```

| Componente | Pasta | Documentação |
|------------|-------|----------------|
| Fluxo de desenvolvimento | — | [docs/workflow/spec.md](docs/workflow/spec.md) |
| API | `src/HoraCerta.Api` | [docs/backend/spec.md](docs/backend/spec.md) |
| Portal | `src/horacerta-web` | [docs/frontend/spec.md](docs/frontend/spec.md) |
| Domínio | `docs/docs.md`, `docs/agregados.md` | |

**MVP web:** portal + API com catálogo público (`GET /api/catalogo/estabelecimentos`), fluxo completo de agendamento e painel do proprietário. Critérios de aceite: [docs/frontend/spec.md §10](docs/frontend/spec.md#10-critérios-de-aceite-mvp), [docs/backend/spec.md §13](docs/backend/spec.md#13-critérios-de-aceite-api-mvp).

## Início rápido (Docker)

```bash
cp .env.example .env
docker compose up --build
```

| URL | Descrição |
|-----|-----------|
| http://localhost:3000 | Portal |
| http://localhost:5080/swagger | API (ambiente Docker) |
| http://localhost:5050 | pgAdmin (e-mail/senha em `.env`) |

No pgAdmin, registre o servidor com: **Host** `postgres`, **Port** `5432`, **Database** / usuário / senha iguais às variáveis `POSTGRES_*` do `.env`.

## Desenvolvimento local

**API**

```bash
cd src/HoraCerta.Api
dotnet run
```

**Portal**

```bash
cd src/horacerta-web
cp .env.local.example .env.local
npm install
npm run dev
```

## Testes

```bash
cd src && dotnet test src.sln
cd src/horacerta-web && npm run test
cd src/horacerta-web && npm run test:bdd:smoke       # BDD UI (sem API)
cd src/horacerta-web && npm run test:bdd:integracao # BDD MVP (com API)
```

Checklist manual: [docs/smoke-test.md](docs/smoke-test.md).

## Skills Cursor

- [horacerta-workflow](.cursor/skills/horacerta-workflow/SKILL.md)
- [horacerta-backend](.cursor/skills/horacerta-backend/SKILL.md)
- [horacerta-frontend](.cursor/skills/horacerta-frontend/SKILL.md)
- [horacerta-frontend-auth](.cursor/skills/horacerta-frontend-auth/SKILL.md)
