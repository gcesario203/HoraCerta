# language: pt
@mvp @cliente @integracao
Funcionalidade: Agendamento pelo cliente
  Como cliente
  Quero agendar um horário
  Para ser atendido no estabelecimento

  Cenário: Fluxo completo de agendamento pendente
    Dado que registro um novo estabelecimento
    Quando faço login como proprietário
    E cadastro o procedimento "Corte Cliente BDD" com valor 50 e duração 45 minutos
    E disponibilizo um horário na agenda
    E acesso a página do estabelecimento
    E inicio o agendamento com meus dados
    E escolho o procedimento cadastrado
    E escolho o primeiro horário disponível
    Então devo ver a mensagem de agendamento pendente

  Cenário: Texto informativo de lembrete na home do cliente
    Dado que registro um novo estabelecimento
    Quando faço login como proprietário
    E acesso a página do estabelecimento
    Então devo ver o texto informativo sobre lembrete
