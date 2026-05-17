# language: pt
@mvp @integracao
Funcionalidade: Fluxo completo MVP
  Como proprietário e cliente
  Quero executar o ciclo de agendamento
  Desde o cadastro até a avaliação

  Cenário: Cadastro, agendamento, confirmação, atendimento e avaliação
    Dado que registro um novo estabelecimento
    Quando faço login como proprietário
    E cadastro o procedimento "Fluxo Completo BDD" com valor 60 e duração 40 minutos
    E disponibilizo um horário na agenda
    E acesso a página do estabelecimento
    E inicio o agendamento com meus dados
    E escolho o procedimento cadastrado
    E escolho o primeiro horário disponível
    E faço login como proprietário
    E confirmo o agendamento pendente do cliente
    E registro o atendimento do agendamento confirmado
    E marco o atendimento como realizado
    E acesso meus agendamentos
    E avalio o atendimento com nota 5
    E consulto a avaliação do agendamento do cliente
