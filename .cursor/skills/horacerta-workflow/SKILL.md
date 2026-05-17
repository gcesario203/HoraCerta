---
name: horacerta-workflow
description: HoraCerta development workflow for new features, bugfixes, and maintenance. Use before any implementation task. Read docs/workflow/spec.md first, then layer-specific specs and skills.
---

# HoraCerta — Fluxo de desenvolvimento

## Before coding

1. Read [docs/workflow/spec.md](../../../docs/workflow/spec.md) (processo e DoD).
2. Read the layer spec: [backend](../../../docs/backend/spec.md) and/or [frontend](../../../docs/frontend/spec.md).
3. Domain rules: [docs/docs.md](../../../docs/docs.md), [docs/agregados.md](../../../docs/agregados.md).

## Order (full-stack feature)

1. Domain / aggregates (if business rule changes)
2. Backend: handler → endpoint → tests → migration if needed
3. Frontend: DTO → use case → repository → UI → BDD if visible flow
4. Update specs in the same change when behavior or decisions change

## DoD (minimum)

- `dotnet test src.sln`
- `npm run lint && npm run test && npm run build` in `horacerta-web`
- BDD or smoke when UI/auth changes
- No secrets in commits

## Layer skills

| Task | Skill |
|------|--------|
| API, EF, migrations | horacerta-backend |
| Portal, modules, BDD | horacerta-frontend |
| JWT, cookies, BFF | horacerta-frontend-auth |

## Environments

- Fast local: API SQLite + `npm run dev` (`API_URL=http://localhost:5080`)
- Integrated: `docker compose up --build` (PostgreSQL + healthchecks)
