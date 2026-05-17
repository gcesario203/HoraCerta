# language: pt
@mvp @proprietario @integracao
Funcionalidade: Agenda de horários
  Como proprietário
  Quero disponibilizar horários
  Para que clientes agendem

  Cenário: Criar slot na agenda
    Dado que registro um novo estabelecimento
    Quando faço login como proprietário
    E disponibilizo um horário na agenda
    Então devo ver o botão "Novo horário"
