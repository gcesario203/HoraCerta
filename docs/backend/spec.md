# Spec: API Backend HoraCerta

Versão: 1.0 (MVP — espelho da implementação)  
Código: `src/HoraCerta.Api` e projetos referenciados  
Domínio legado: [`docs/docs.md`](../docs.md), [`docs/agregados.md`](../agregados.md)  
Portal: [`docs/frontend/spec.md`](../frontend/spec.md)

---

## 1. Visão

API **ASP.NET Core 8** (Minimal APIs) que expõe casos de uso de agendamento, autenticação de proprietário (JWT), lembretes (background job) e persistência **híbrida JSON em tabelas relacionais** (agregados serializados).

| Componente | Projeto | Responsabilidade |
|------------|---------|------------------|
| API HTTP | `HoraCerta.Api` | Endpoints, contratos, auth, Swagger |
| Aplicação | `HoraCerta.Aplicacao` | Commands, queries, handlers, integração (eventos, lembretes) |
| Domínio | `HoraCerta.Dominio` | Entidades, estados, regras, eventos de domínio |
| Infraestrutura | `HoraCerta.Infaestrutura` | EF Core, repositórios, lembretes, migrations |
| Transversal | `HoraCerta.Transversal` | Utilitários compartilhados |

**Fora do escopo desta entrega (MVP web):** WhatsApp / UC 11, OpenAPI codegen, cancelar/remarcar pelo cliente na API pública.

---

## 2. Decisões de produto e arquitetura (fechadas)

| # | Tópico | Decisão |
|---|--------|---------|
| 1 | Estilo API | Minimal APIs + records em `Contratos/` |
| 2 | Persistência agregados | JSON em coluna `Conteudo` (`proprietarios`, `clientes`) |
| 3 | Auth proprietário | JWT Bearer; `sub` = `proprietarioId` |
| 4 | Rotas mutáveis proprietário | `ProprietarioId` no body + filter valida claim JWT |
| 5 | Iniciar agendamento | **Público** (sem JWT) |
| 6 | Avaliar agendamento | **Público** (sem JWT); validação no domínio |
| 7 | Lembretes | Tabela `lembretes` + `LembreteBackgroundService` + `ConsoleEnviadorLembrete` |
| 8 | Banco local dev | SQLite (`horacerta.db`) |
| 9 | Banco Docker | PostgreSQL 16 (`Database:Provider=PostgreSQL`) |
| 10 | Migrations | EF migrations (SQLite); PostgreSQL usa `EnsureCreated` no bootstrap Docker |

---

## 3. Stack e requisitos não funcionais

| Item | Escolha |
|------|---------|
| Runtime | .NET 8 |
| ORM | EF Core 8 |
| Banco dev | SQLite |
| Banco compose | PostgreSQL (Npgsql) |
| Auth | `Microsoft.AspNetCore.Authentication.JwtBearer` |
| Docs HTTP | Swagger (somente Development) |
| Testes | xUnit — unitários, integração, E2E (`WebApplicationFactory`) |
| Idioma erros API | PT-BR (`mensagem`) via `TratamentoExcecoesDominio` |
| JSON | camelCase (padrão ASP.NET Core) |

---

## 4. Estrutura da solução

```
src/
├── HoraCerta.Api/              # Host, Endpoints, Contratos, Autenticacao
├── HoraCerta.Aplicacao/        # Handlers, Commands, Queries, Integracao
├── HoraCerta.Dominio/          # Entidades, estados, eventos
├── HoraCerta.Infaestrutura/    # Persistencia, Repositorio, Lembretes
├── HoraCerta.Transversal/
├── HoraCerta.Testes.Unitarios/
├── HoraCerta.Testes.Integracao/
├── HoraCerta.Testes.E2e/
└── horacerta-web/              # Portal (consumidor da API)
```

### 4.1 Camadas e dependência

```
Api → Aplicacao → Dominio
Api → Infaestrutura → Aplicacao, Dominio
```

- **Proibido:** `Dominio` referenciar EF, ASP.NET ou handlers.
- **Handlers** na aplicação orquestram domínio + repositórios + dispatcher de eventos.

### 4.2 Persistência (modelo relacional + JSON)

| Tabela | Uso |
|--------|-----|
| `proprietarios` | `Id`, `Conteudo` (JSON do agregado Proprietário) |
| `clientes` | `Id`, `Conteudo` (JSON do agregado Cliente) |
| `credenciais_proprietario` | Login (`Email` único, `PasswordHash`) |
| `lembretes` | Fila de envio (status, `EnviarEm`, índices) |

Agregados **Proprietário** e **Cliente** encapsulam procedimentos, slots, agendamentos, atendimentos e avaliações no JSON.

---

## 5. Autenticação e autorização

### 5.1 JWT

Configuração em `appsettings.json` → seção `Jwt`:

| Campo | Descrição |
|-------|-----------|
| `Issuer` / `Audience` | `HoraCerta` |
| `Key` | Segredo simétrico (mín. 32 caracteres em produção) |
| `ExpiracaoHoras` | Padrão 8 |

`POST /api/auth/login` → `{ token, proprietarioId }`.

### 5.2 Filtros

| Filtro | Uso |
|--------|-----|
| `ProprietarioAuthorizationFilter` | Rota `{proprietarioId}` deve coincidir com claim `sub` |
| `ProprietarioBodyAuthorizationFilter` | Body com `ProprietarioId` deve coincidir com claim |

Endpoints com `.RequireAuthorization()` exigem header `Authorization: Bearer {token}`.

---

## 6. API REST — mapa completo

Base: `/api`. Tags Swagger conforme `Endpoints/*.cs`.

### 6.1 Autenticação (`/api/auth`)

| Método | Rota | Auth | Request | Response |
|--------|------|------|---------|----------|
| POST | `/registrar` | Não | `RegistrarCredencialRequisicao` | `201` `{ proprietarioId }` |
| POST | `/login` | Não | `LoginRequisicao` | `200` `LoginResposta` |

`RegistrarCredencialRequisicao`: `proprietarioId?`, `nomeEstabelecimento?`, `email`, `senha` — cria proprietário se `nomeEstabelecimento` informado.

### 6.2 Cadastro (`/api`)

| Método | Rota | Auth | Request | Response |
|--------|------|------|---------|----------|
| POST | `/proprietarios` | Não | `CriarProprietarioRequisicao` | `201` `ProprietarioResposta` |
| GET | `/proprietarios/{id}` | Não | — | `200` / `404` |
| POST | `/clientes` | Não | `CriarClienteRequisicao` | `201` `ClienteResposta` |
| GET | `/clientes/{id}` | Não | — | `200` / `404` |

### 6.3 Procedimentos (`/api/proprietarios/{proprietarioId}/procedimentos`)

| Método | Rota | Auth | Request | Response |
|--------|------|------|---------|----------|
| GET | `/` | Não | — | `200` `ProcedimentoResposta[]` (ativos) |
| POST | `/` | JWT + filter | `CriarProcedimentoRequisicao` | `201` `ProcedimentoResposta` |
| POST | `/{procedimentoId}/inativar` | JWT + filter | — | `200` `ProcedimentoResposta` |

### 6.4 Slots (`/api/proprietarios/{proprietarioId}/slots`)

| Método | Rota | Auth | Request | Response |
|--------|------|------|---------|----------|
| GET | `/disponiveis` | Não | — | `200` `SlotHorarioResposta[]` |
| POST | `/` | JWT + filter | `CriarSlotRequisicao` | `201` `SlotHorarioResposta` |

### 6.5 Agendamentos (`/api/agendamentos`)

| Método | Rota | Auth | Request | Response |
|--------|------|------|---------|----------|
| POST | `/iniciar` | Não | `IniciarAgendamentoRequisicao` | `201` `AgendamentoResposta` |
| POST | `/{id}/confirmar` | JWT + body filter | `ConfirmarAgendamentoRequisicao` | `200` |
| POST | `/{id}/cancelar` | JWT + body filter | `CancelarAgendamentoRequisicao` | `200` |
| POST | `/{id}/remarcar` | JWT + body filter | `RemarcarAgendamentoRequisicao` | `201` |
| POST | `/{id}/atendimento` | JWT + body filter | `RegistrarAtendimentoRequisicao` | `201` `AtendimentoResposta` |

### 6.6 Avaliação e atendimento (rotas avulsas)

| Método | Rota | Auth | Request | Response |
|--------|------|------|---------|----------|
| POST | `/api/clientes/{clienteId}/agendamentos/{agendamentoId}/avaliar` | Não | `AvaliarAgendamentoRequisicao` | `200` `AvaliacaoResposta` |
| PATCH | `/api/proprietarios/{proprietarioId}/atendimentos/{atendimentoId}/estado` | JWT + filter | `AlterarEstadoAtendimentoRequisicao` | `200` `AtendimentoResposta` |

Estados atendimento: `REALIZADO`, `CANCELADO`, `FALHA` (enum case-insensitive).

### 6.7 Consultas

| Método | Rota | Auth | Response |
|--------|------|------|----------|
| GET | `/api/clientes/{clienteId}/agendamentos` | Não | `AgendamentoResposta[]` |
| GET | `/api/proprietarios/{proprietarioId}/agendamentos` | JWT + filter | `AgendamentoListagemResposta[]` |
| GET | `/api/proprietarios/{proprietarioId}/atendimentos` | JWT + filter | `AtendimentoResposta[]` |
| GET | `/api/proprietarios/{proprietarioId}/agendamentos/{agendamentoId}/avaliacao` | JWT + filter | `AvaliacaoResposta` / `404` |

---

## 7. Contratos (DTOs)

Arquivos em `HoraCerta.Api/Contratos/`:

| Arquivo | Records principais |
|---------|-------------------|
| `Requisicoes.cs` | Todas as requisições POST/PATCH |
| `AuthRespostas.cs` | `LoginResposta`, `AgendamentoListagemResposta`, `AvaliacaoResposta` |
| `AgendamentoResposta.cs` | `Id`, `ClienteId`, `ProcedimentoId`, `SlotHorarioId`, `Estado`, `ReagendamentoId` |
| `ProcedimentoResposta.cs` | `Id`, `Nome`, `Valor`, `TempoEstimadoMinutos`, `Estado` |
| `SlotHorarioResposta.cs` | `Id`, `Inicio`, `Fim`, `Status` |
| `AtendimentoResposta.cs` | `Id`, `AgendamentoId`, `ValorNegociado`, `Estado` |
| `ClienteResposta.cs` | `Id`, `Nome`, `Telefone` |
| `ProprietarioResposta.cs` | `Id`, `Nome` |

Mapeamento: `RespostaMapeamento.cs`.

---

## 8. Aplicação — handlers registrados

Registro em `HoraCerta.Api/Extensions/DependencyInjection.cs` → `AddHoraCerta`.

| Handler | Responsabilidade |
|---------|------------------|
| `RegistrarCredencialHandler` / `LoginHandler` | Auth |
| `CriarProcedimentoHandler` / `InativarProcedimentoHandler` / `ListarProcedimentosAtivosHandler` | UC 9 |
| `CriarSlotDisponivelHandler` / `ListarSlotsDisponiveisHandler` | UC 10 |
| `IniciarAgendamentoHandler` | UC 1–3 (PENDENTE) |
| `ConfirmarAgendamentoHandler` / `CancelarAgendamentoHandler` / `RemarcarAgendamentoHandler` | UC 4–6 |
| `RegistrarAtendimentoHandler` / `AlterarEstadoAtendimentoHandler` | Atendimento |
| `AvaliarAgendamentoHandler` | UC 8 |
| `ListarAgendamentosClienteHandler` / `ListarAgendamentosProprietarioHandler` | Consultas |
| `ListarAtendimentosHandler` / `ObterAvaliacaoAgendamentoHandler` | Consultas |

### 8.1 Eventos de domínio (integração)

`DomainEventDispatcher` + handlers em `HoraCerta.Aplicacao/Integracao/Eventos/`:

- Agendamento confirmado/cancelado/remarcado → lembretes (`Agendamento*LembreteHandler`)
- Logs de auditoria (`LogAgendamento*Handler`)

---

## 9. Lembretes (UC 7 — backend)

| Item | Implementação |
|------|----------------|
| Config | `Lembretes:HorasAntecedencia` (24), `IntervaloMinutos` (15) |
| Persistência | `lembretes` |
| Worker | `LembreteBackgroundService` (hosted service) |
| Envio | `IEnviadorLembrete` → `ConsoleEnviadorLembrete` (log) |
| Desabilitar | Ambiente `Testing` (`incluirBackground: false` em `Program.cs`) |

---

## 10. Configuração

### 10.1 `appsettings.json`

```json
{
  "ConnectionStrings": { "HoraCerta": "Data Source=horacerta.db" },
  "Database": { "Provider": "Sqlite" },
  "Jwt": { "Issuer": "HoraCerta", "Audience": "HoraCerta", "Key": "...", "ExpiracaoHoras": 8 },
  "Lembretes": { "HorasAntecedencia": 24, "IntervaloMinutos": 15 }
}
```

### 10.2 Variáveis de ambiente (Docker)

| Variável | Exemplo |
|----------|---------|
| `ConnectionStrings__HoraCerta` | `Host=postgres;Port=5432;Database=horacerta;Username=horacerta;Password=horacerta` |
| `Database__Provider` | `PostgreSQL` |
| `Jwt__Key` | segredo forte |
| `ASPNETCORE_URLS` | `http://+:8080` |

---

## 11. Execução local

```bash
cd src/HoraCerta.Api
dotnet run
# http://localhost:5080/swagger
```

Migrations aplicadas automaticamente ao iniciar (exceto ambiente `Testing`).

---

## 12. Docker (stack completa)

Ver [`docker-compose.yml`](../../docker-compose.yml) na raiz do repositório.

| Serviço | Imagem / build | Porta host |
|---------|----------------|------------|
| `postgres` | `postgres:16-alpine` | 5432 |
| `api` | `src/HoraCerta.Api/Dockerfile` | 5080 → 8080 |
| `web` | `src/horacerta-web/Dockerfile` | 3000 |

```bash
docker compose up --build
# Portal: http://localhost:3000
# API:    http://localhost:5080/swagger (se Development) ou /api
```

---

## 13. Critérios de aceite (API MVP)

- [ ] Registrar/login proprietário com JWT válido
- [ ] CRUD procedimentos (criar, listar ativos, inativar) com JWT
- [ ] Criar slot e listar disponíveis
- [ ] Iniciar agendamento (público) → estado PENDENTE
- [ ] Confirmar, cancelar, remarcar (JWT proprietário)
- [ ] Registrar atendimento e alterar estado REALIZADO/CANCELADO/FALHA
- [ ] Avaliar agendamento (público) e consultar avaliação (JWT)
- [ ] Listar agendamentos cliente e proprietário
- [ ] Lembretes gravados e processados pelo background service
- [ ] Compose sobe API + Web + PostgreSQL com healthcheck

---

## 14. Evolução planejada

- Migrations EF dedicadas ao PostgreSQL (substituir `EnsureCreated` no Docker)
- Provider de envio real de lembretes (WhatsApp / SMS / e-mail)
- Endpoints cliente para cancelar/remarcar com token
- OpenAPI exportável para codegen opcional do front
