# Integração WhatsApp (Twilio) — Onda 1

## Escopo

- Bot de agendamento (UC 1–3): identificar cliente, escolher procedimento/horário, criar agendamento **PENDENTE**
- Notificações: confirmação, cancelamento, remarcação via **Transactional Outbox**
- Lembretes: agendados na tabela `lembretes`, enviados via outbox no vencimento
- Multi-tenant: número Twilio único + link `wa.me` com `HC-{proprietarioId}`

## Webhook

`POST /api/webhooks/twilio/whatsapp` (form-urlencoded Twilio)

### ngrok (dev local com Docker)

```bash
docker compose build api
docker compose up -d api
ngrok http 5080
```

Se `GET /api/webhooks/twilio/whatsapp` retornar **404**, a API em execução está **desatualizada** (build anterior ao WhatsApp). Rebuild obrigatório (`docker compose build api`). No Swagger deve aparecer o grupo **Webhooks Twilio**.

Pare qualquer `dotnet run` antigo na porta 5080 antes de subir o Docker.

| Onde | URL |
|------|-----|
| `.env` → `TWILIO_WEBHOOK_BASE_URL` | `https://<subdomínio>.ngrok-free.app` (sem barra final, **sem** `/api/...`) |
| Twilio Console → **When a message comes in** | `https://<subdomínio>.ngrok-free.app/api/webhooks/twilio/whatsapp` |
| Teste rápido (GET) | mesma URL do webhook — deve retornar `{ "status": "webhook ativo" }` |
| Swagger (opcional) | `https://<subdomínio>.ngrok-free.app/swagger` |

Exemplo atual (tunnel ativo):

- Base: `https://23a0-2804-1b3-a681-a669-48f-36f6-cb81-3c60.ngrok-free.app`
- Webhook Twilio: `https://23a0-2804-1b3-a681-a669-48f-36f6-cb81-3c60.ngrok-free.app/api/webhooks/twilio/whatsapp`

`Status callback URL` no Twilio pode ficar em branco na Onda 1.

| Campo | Uso |
|-------|-----|
| `MessageSid` | Idempotência (tabela `webhooks_twilio_processados`) |
| `From` | Telefone cliente (E.164) |
| `Body` | Texto / código `HC-{guid}` |

Validação opcional: `X-Twilio-Signature` (`Twilio:ValidarAssinaturaWebhook`).

## Outbox

Tabela `mensagens_outbox` — worker `OutboxWhatsAppBackgroundService` envia com retry/backoff.

| Status | Significado |
|--------|-------------|
| Pendente | Aguardando envio |
| Processando | Reservado pelo worker |
| Enviado | Twilio OK |
| Falha | Esgotou tentativas |
| Cancelado | Agendamento cancelado |

## Bot — estados

1. Resolver estabelecimento (`HC-{id}` ou código)
2. Identificar cliente (nome + telefone WhatsApp)
3. Escolher procedimento (lista numerada)
4. Escolher horário (slots disponíveis)
5. Revisar (SIM/NÃO) → `IniciarAgendamento` → PENDENTE

## Opt-out

Palavras `SAIR`, `PARAR`, `STOP` → `Cliente.OptOutWhatsApp = true`.

## Configuração

Ver `appsettings.json`: seções `Twilio`, `Outbox`, `WhatsApp`.

Dev: `Twilio:Enabled=false` → log no console (sem API real).

## Testes automatizados

| Projeto | Arquivo | Cobertura |
|---------|---------|-----------|
| `HoraCerta.Testes.Unitarios` | `Comunicacao/NormalizadorTelefoneTests` | E.164, prefixo whatsapp |
| `HoraCerta.Testes.Integracao` | `Comunicacao/MensagemOutboxIntegracaoTests` | Idempotência, reserva, falha, cancelamento |
| | `Comunicacao/OutboxProcessamentoIntegracaoTests` | Envio fake, falha definitiva |
| | `Comunicacao/WebhookIdempotenciaIntegracaoTests` | `MessageSid` duplicado |
| | `Comunicacao/ConfirmacaoOutboxIntegracaoTests` | Evento confirmado → outbox, opt-out |
| | `Comunicacao/OrquestradorBotAgendamentoTests` | Fluxo bot completo, cliente existente |

```bash
dotnet test src/HoraCerta.Testes.Unitarios
dotnet test src/HoraCerta.Testes.Integracao
```

## Frontend

Botão **Agendar no WhatsApp** em `/e/[proprietarioId]` — env `NEXT_PUBLIC_WHATSAPP_NUMERO`.
