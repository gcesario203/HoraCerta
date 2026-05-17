# Smoke test — HoraCerta MVP

## Automatizado (BDD Gherkin)

| Comando | Escopo | Requer API |
|---------|--------|------------|
| `npm run test:bdd:smoke` | `@smoke` (landing, login UI) | Não |
| `npm run test:bdd:integracao` | `@integracao` (fluxos MVP) | Sim |
| `npm run test:bdd` | alias de integração | Sim |

```bash
# Sem API (2 cenários)
cd src/horacerta-web
npx playwright install chromium
npm run test:bdd:smoke

# Com API + portal (stack completa)
docker compose up -d postgres api
cd src/horacerta-web && npm run dev   # outro terminal
npm run test:bdd:integracao
```

### Features

| Pasta | Arquivos |
|-------|----------|
| `e2e/features/publico/` | `landing`, `login`, `registrar`, `login-autenticado` |
| `e2e/features/proprietario/` | `procedimentos`, `agenda` |
| `e2e/features/cliente/` | `agendar`, `meus-agendamentos` |
| `e2e/features/mvp/` | `fluxo-completo` |

Especificação: [frontend/spec.md §9.1](frontend/spec.md#91-testes-e2e--bdd-gherkin).

## Manual (checklist)

Use após subir a stack (`docker compose up`) ou dev local.

### Proprietário

1. [ ] Registrar em `/registrar` — obter `proprietarioId`
2. [ ] Login em `/login` — redireciona para `/proprietario/agendamentos`
3. [ ] Criar procedimento em `/proprietario/procedimentos`
4. [ ] Criar slot em `/proprietario/agenda`
5. [ ] (Cliente) Agendar em `/e/{proprietarioId}/agendar` — estado PENDENTE
6. [ ] Confirmar agendamento no painel
7. [ ] Registrar atendimento e alterar estado (REALIZADO)
8. [ ] Ver avaliação (após cliente avaliar)

### Cliente

1. [ ] Wizard agendar — mensagem de espera (PENDENTE)
2. [ ] Meus agendamentos — nome do procedimento e horário visíveis
3. [ ] Avaliar após atendimento realizado
4. [ ] Sem botões de cancelar/remarcar

### API (opcional)

- Swagger: http://localhost:5080/swagger (Development ou ambiente `Docker`)
