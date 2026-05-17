# Smoke test manual — HoraCerta MVP

Checklist rápido após subir a stack (`docker compose up` ou dev local).

## Proprietário

1. [ ] `POST /api/auth/registrar` ou UI `/registrar` — obter `proprietarioId`
2. [ ] Login em `/login` — redireciona para `/proprietario/agendamentos`
3. [ ] Criar procedimento em `/proprietario/procedimentos`
4. [ ] Criar slot em `/proprietario/agenda`
5. [ ] (Cliente) Agendar em `/e/{proprietarioId}/agendar` — estado PENDENTE
6. [ ] Confirmar agendamento no painel
7. [ ] Registrar atendimento e alterar estado (REALIZADO)
8. [ ] Ver avaliação (após cliente avaliar)

## Cliente

1. [ ] Wizard agendar — mensagem de espera (PENDENTE)
2. [ ] Meus agendamentos — nome do procedimento e horário visíveis
3. [ ] Avaliar após atendimento realizado
4. [ ] Sem botões de cancelar/remarcar

## API (opcional)

- Swagger: http://localhost:5080/swagger (Development ou ambiente `Docker`)
