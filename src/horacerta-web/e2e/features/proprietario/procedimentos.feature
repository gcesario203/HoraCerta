# language: pt
@mvp @proprietario @integracao
Funcionalidade: Procedimentos do proprietário
  Como proprietário autenticado
  Quero gerenciar procedimentos
  Para oferecer serviços aos clientes

  Cenário: Criar procedimento
    Dado que registro um novo estabelecimento
    Quando faço login como proprietário
    E cadastro o procedimento "Barba BDD" com valor 35 e duração 30 minutos
    Então devo ver o procedimento "Barba BDD" na listagem
