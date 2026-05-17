# Frontend — HoraCerta

Documentação spec-driven do portal web.

| Documento | Conteúdo |
|-----------|----------|
| [spec.md](./spec.md) | Requisitos funcionais, rotas, arquitetura, decisões MVP |

## Skills Cursor (projeto)

| Skill | Quando usar |
|-------|-------------|
| [horacerta-frontend](../../.cursor/skills/horacerta-frontend/SKILL.md) | UI, use cases, entidades, Ant Design, Zustand, Axios |
| [horacerta-frontend-auth](../../.cursor/skills/horacerta-frontend-auth/SKILL.md) | Login, cookies httpOnly, BFF, middleware, sessão cliente |

Arquivos de apoio: `horacerta-frontend/reference.md`, `horacerta-frontend/examples.md`.

## Projeto

Código em [`src/horacerta-web`](../../src/horacerta-web).

```bash
cd src/horacerta-web
cp .env.local.example .env.local
npm install
npm run dev
```

API backend: `http://localhost:5080` (variável `API_URL` no `.env.local`).

### Testes

```bash
npm run test                  # Vitest (unitário)
npm run test:bdd:smoke          # BDD @smoke (sem API)
npm run test:bdd:integracao     # BDD @integracao (requer API :5080)
```

Cenários em `e2e/features/` (ver [spec.md §9.1](./spec.md#91-testes-e2e--bdd-gherkin)).

Stack Docker (API + Web + PostgreSQL): ver [`docker-compose.yml`](../../docker-compose.yml) e [`docs/backend/README.md`](../backend/README.md).
