# Smoke test — HoraCerta MVP

## WhatsApp (Twilio) — manual

1. `Twilio:Enabled=false` — confirmar agendamento no portal e ver log `[WhatsApp simulado]` / outbox em `mensagens_outbox`.
2. Sandbox Twilio: `ngrok http 5080` → **When a message comes in** = `{TWILIO_WEBHOOK_BASE_URL}/api/webhooks/twilio/whatsapp` (ex.: `https://23a0-2804-1b3-a681-a669-48f-36f6-cb81-3c60.ngrok-free.app/api/webhooks/twilio/whatsapp`); mensagem `HC-{proprietarioId}` inicia o bot.
3. Botão **Agendar no WhatsApp** em `/e/{id}` com `NEXT_PUBLIC_WHATSAPP_NUMERO` configurado.

Ver [integracao-whatsapp/spec.md](integracao-whatsapp/spec.md).

### Erro `relation "mensagens_outbox" does not exist`

A migration `Onda3WhatsAppOutbox` cria `mensagens_outbox`, `sessoes_conversa` e `webhooks_twilio_processados`. Se o volume Postgres foi criado com uma migration vazia/antiga, reaplique:

```bash
docker compose build api
docker compose up -d
```

Se o erro persistir, remova o registro da migration vazia e suba de novo:

```bash
docker compose exec postgres psql -U horacerta -d horacerta -c "DELETE FROM \"__EFMigrationsHistory\" WHERE \"MigrationId\" LIKE '%Onda3WhatsAppOutbox%';"
docker compose restart api
```

Ou recrie o volume (apaga dados locais): `docker compose down -v && docker compose up --build`.

### Erro `text <= timestamp with time zone` (PostgreSQL)

Colunas `DateTime` criadas como `TEXT` (SQLite) precisam ser `timestamptz` no Postgres. A migration `FixPostgresDateTimeColumns` corrige isso automaticamente no `docker compose up` após rebuild da API:

```bash
docker compose build api
docker compose up -d
```

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
