# language: pt
@mvp @publico @integracao
Funcionalidade: Login do proprietário autenticado
  Como proprietário registrado
  Quero fazer login
  Para acessar o painel

  Cenário: Login com credenciais válidas
    Dado que registro um novo estabelecimento
    Quando faço login como proprietário
    Então devo estar autenticado no painel do proprietário
