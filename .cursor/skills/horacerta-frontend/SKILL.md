---
name: horacerta-frontend
description: Builds the HoraCerta web portal with Next.js, React, TypeScript, DDD per entity, Ant Design, Axios only in infrastructure, and Zustand (no TanStack Query). Use for HoraCerta.Web pages, components, use cases, stores, or API integration. Always read docs/frontend/spec.md first.
---

# HoraCerta Frontend

## Before coding

1. Read [docs/frontend/spec.md](../../../docs/frontend/spec.md) (source of truth).
2. For auth/cookies/BFF, also apply skill **horacerta-frontend-auth**.
3. For file templates, see [reference.md](reference.md) and [examples.md](examples.md).

## Stack (non-negotiable)

| Use | Do not use |
|-----|------------|
| Next.js App Router, React, TS `strict` | Pages Router |
| Ant Design | Other UI kits without approval |
| Zustand (`presentation/stores/`) | TanStack Query, SWR, Redux |
| Axios in `infrastructure/api` only | Axios/fetch in components |
| Manual DTO types vs `HoraCerta.Api/Contratos` | OpenAPI codegen |

## Entity folder layout

Every bounded context lives under `src/[nome-entidade]/`:

`domain` → `application` → `infrastructure` → `presentation`

`src/app/` = routes only (thin). Business logic stays in entity modules.

## MVP rules (product)

- **Cliente:** agendar (PENDENTE), meus agendamentos, avaliar, lembrete = texto informativo only.
- **Cliente:** no cancel/remarcar UI.
- **Proprietário:** login, procedimentos, slots, agendamentos (confirm/cancel/remarcar), atendimento, ver avaliações.
- No WhatsApp, no UC 11.

## Implementation flow

1. Map endpoint in `src/HoraCerta.Api/Endpoints` + `Contratos`.
2. Add types → repository interface → Axios api → repository impl → use case.
3. Hook + Ant Design UI → route in `app/`.
4. Zustand only if state crosses routes (see reference.md).

## Data loading (no TanStack)

```
Component → hook → useCase.execute() → repository → axios
         ← useState / Zustand ← result
```

Refetch explicitly after mutations.

## Prohibited

- `@tanstack/react-query`, `swr`
- `localStorage` for JWT or primary session
- `stores/global.ts` monolith
- Business rules inside React components

## Additional resources

- [reference.md](reference.md) — routes, entities, stores, API map
- [examples.md](examples.md) — repository, use case, hook, Zustand templates
