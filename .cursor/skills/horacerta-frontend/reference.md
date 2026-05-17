# HoraCerta Frontend — Reference

## Project layout

```
src/
  app/                    # Next routes, layouts, Route Handlers (BFF)
  shared/infrastructure/http/
    axios-client.ts
    api-error.ts
  auth/
  proprietario/
  cliente/
  procedimento/
  slot-horario/
  agendamento/
  atendimento/
  avaliacao/
```

Per entity:

```
[nome-entidade]/
  domain/entities/
  domain/value-objects/
  domain/repositories/       # I*Repository
  application/use-cases/
  application/dtos/
  infrastructure/api/        # *.api.ts + *.repository.ts
  presentation/components/
  presentation/hooks/
  presentation/stores/       # Zustand (if needed)
  presentation/mappers/
```

## Routes

| Path | Auth |
|------|------|
| `/login`, `/registrar` | Public |
| `/proprietario/*` | Proprietário JWT (httpOnly cookie) |
| `/e/[proprietarioId]/*` | Public (+ cliente cookie for meus-agendamentos) |

Details: `docs/frontend/spec.md` §7.

## Zustand stores

| Store path | State |
|------------|--------|
| `auth/presentation/stores/auth.store.ts` | `proprietarioId`, `isAuthenticated` — never store JWT string |
| `cliente/presentation/stores/cliente-sessao.store.ts` | `clienteId`, `proprietarioId` |

## API map (backend today)

| Module | Endpoints |
|--------|-----------|
| auth | `POST /api/auth/login`, `POST /api/auth/registrar` |
| proprietario | `GET/POST /api/proprietarios`, `GET /api/proprietarios/{id}` |
| cliente | `POST/GET /api/clientes`, `GET /api/clientes/{id}/agendamentos` |
| procedimento | `GET/POST /api/proprietarios/{id}/procedimentos`, `POST .../inativar` |
| slot-horario | `GET .../slots/disponiveis`, `POST .../slots` |
| agendamento | `POST /api/agendamentos/iniciar`, `POST .../confirmar` (JWT), `cancelar`, `remarcar`, `atendimento` |
| atendimento | `PATCH /api/proprietarios/{id}/atendimentos/{id}/estado` |
| avaliacao | `POST .../avaliar`, `GET .../avaliacao` |

C# contracts: `src/HoraCerta.Api/Contratos/`.

## Ant Design

- Wrap app with `ConfigProvider` locale `pt_BR`.
- Listagens: `Table` + `Spin`/`Skeleton`.
- Forms: `Form` + `rules` for validation messages in PT.
- Feedback: `App.useApp().message` or static `message` from `antd`.

## Naming

- Use cases: verb + entity (`ListarProcedimentos`, `ConfirmarAgendamento`).
- Hooks: `use` + feature (`useProcedimentos`, `useConfirmarAgendamento`).
- API files: `procedimento.api.ts`, `procedimento.repository.ts`.
- Match backend entity names: `proprietario`, `cliente`, `agendamento` (not `establishment`).

## Tests

| Layer | What to test |
|-------|----------------|
| use-cases | Mock `I*Repository` |
| hooks | Mock use case or repository |
| components | RTL + mocked hook |

Do not unit-test Axios in presentation.
