# Smoke test — HoraCerta MVP

## Automatizado (BDD Gherkin)

| Comando | Escopo | Requer API |
|---------|--------|------------|
| `npm run test:bdd:smoke` | `@smoke` (landing, login UI) | Não |
| `npm run test:bdd:integracao` | `@integracao` (fluxos MVP) | Sim |
| `npm run test:bdd` | alias de integração | Sim |

```bash
# Sem API (cenários @smoke)
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
| `e2e/features/mvp/` | `fluxo-completo` (ciclo proprietário + cliente + avaliação) |

Especificação: [frontend/spec.md §9.1](frontend/spec.md#91-testes-e2e--bdd-gherkin).

## Manual (checklist)

Use após subir a stack (`docker compose up`) ou dev local.

### Público (home)

1. [ ] Abrir `/` — campo de busca do catálogo e atalho “Área do proprietário”
2. [ ] Com estabelecimento cadastrado (procedimento + slot futuro), card aparece no catálogo
3. [ ] “Agendar agora” no card leva a `/e/{proprietarioId}/agendar`

### Proprietário

1. [ ] Registrar em `/registrar` — obter `proprietarioId`
2. [ ] Login em `/login` — redireciona para `/proprietario/agendamentos`
3. [ ] Criar procedimento em `/proprietario/procedimentos`
4. [ ] Criar slot em `/proprietario/agenda` — visível na grade **Semana** (ou Lista/Tabela)
5. [ ] (Cliente) Agendar em `/e/{proprietarioId}/agendar` — estado PENDENTE
6. [ ] Confirmar agendamento no painel
7. [ ] Registrar atendimento e alterar estado (REALIZADO)
8. [ ] Ver avaliação (após cliente avaliar)

### Cliente

1. [ ] Wizard agendar: identificação → serviço → horário na grade (`.hc-week-slot`) → **Revisão** → confirmar
2. [ ] Mensagem de espera (PENDENTE) após envio
3. [ ] Meus agendamentos — nome do procedimento e horário visíveis; nav “Meus horários”
4. [ ] Avaliar após atendimento realizado
5. [ ] Sem botões de cancelar/remarcar

### API (opcional)

- Swagger: http://localhost:5080/swagger (Development ou ambiente `Docker`)
- Catálogo: `GET http://localhost:5080/api/catalogo/estabelecimentos`
