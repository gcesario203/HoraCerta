# horacerta-web

Portal HoraCerta (Next.js). Spec: [docs/frontend/spec.md](../../docs/frontend/spec.md).

## Desenvolvimento

```bash
cp .env.local.example .env.local
npm install
npm run dev
```

## Testes

```bash
npx playwright install chromium   # primeira vez

npm run test                      # Vitest (unitário)

npm run test:bdd:smoke            # BDD UI-only (sem API)
npm run test:bdd:integracao       # BDD fluxos MVP (requer API em :5080)
```

### BDD com API (stack local)

```bash
# Terminal 1 — raiz do repo
docker compose up -d postgres api

# Terminal 2
cd src/horacerta-web
npm run dev

# Terminal 3
npm run test:bdd:integracao
```

Cenários: `e2e/features/**/*.feature`  
Steps: `e2e/steps/**/*.ts`

Ver [spec §9.1](../../docs/frontend/spec.md#91-testes-e2e--bdd-gherkin).
