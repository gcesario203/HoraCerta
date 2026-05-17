# language: pt
@mvp @publico @integracao
Funcionalidade: Registro de estabelecimento
  Como futuro proprietário
  Quero criar minha conta
  Para acessar o painel

  Cenário: Registrar estabelecimento com sucesso
    Dado que registro um novo estabelecimento
    Então devo ver o campo "E-mail"
