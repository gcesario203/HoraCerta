# language: pt
@mvp @cliente @integracao
Funcionalidade: Meus agendamentos do cliente
  Como cliente
  Quero ver meus agendamentos
  Sem poder cancelar ou remarcar

  Cenário: Listar agendamento após solicitar
    Dado que registro um novo estabelecimento
    Quando faço login como proprietário
    E cadastro o procedimento "Lista BDD" com valor 40 e duração 30 minutos
    E disponibilizo um horário na agenda
    E acesso a página do estabelecimento
    E inicio o agendamento com meus dados
    E escolho o procedimento cadastrado
    E escolho o primeiro horário disponível
    E acesso meus agendamentos
    Então devo ver meu agendamento com o procedimento
    E não devo ver opção de cancelar ou remarcar
