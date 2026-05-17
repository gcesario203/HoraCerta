# language: pt
@mvp @publico
Funcionalidade: Login do proprietário
  Para que eu gerencie meu estabelecimento
  Como proprietário
  Quero autenticar no portal

  @smoke
  Cenário: Exibir formulário de login
    Dado que estou na página de login
    Então devo ver o campo "E-mail"
    E devo ver o campo "Senha"
