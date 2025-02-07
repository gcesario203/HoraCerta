# Documento de Requisitos do Sistema

# HoraCerta

## 1. Descrição de Domínio

#### Visão Geral

O **HoraCerta** é um sistema de agendamento automatizado que visa facilitar o processo de
marcação de procedimentos, principalmente via WhatsApp. O objetivo principal é otimizar a
interação com os clientes enquanto mantém a humanização no atendimento. O sistema
permite ao proprietário gerenciar procedimentos, horários disponíveis, agendamentos,
fluxos de comunicação e avaliações, além de possibilitar uma interação automatizada com
os clientes via WhatsApp Business.

#### Atores

1. **Proprietário** : Responsável por gerenciar os procedimentos, horários disponíveis e
    agendamentos. Também pode visualizar as avaliações dos atendimentos e
    configurar mensagens automatizadas.
2. **Cliente** : Realiza o agendamento, interage com o WhatsApp Bot para confirmar,
    cancelar ou remarcar agendamentos e fornecer avaliações.
3. **WhatsApp Bot** : Responsável pela interação automatizada com os clientes,
    enviando mensagens de confirmação, lembretes, cancelamentos e solicitações de
    avaliação.

#### Entidades e Relacionamentos

```
● Procedimento : Representa os serviços oferecidos pelo proprietário. Cada
procedimento tem um nome, valor e tempo estimado de duração.
● Agendamento : Reflete a solicitação de um cliente para um procedimento em um
horário específico. Cada agendamento tem status (Pendente, Confirmado,
Cancelado, Remarcado).
● Atendimento : Após a confirmação de um agendamento, o atendimento é realizado.
O status do atendimento pode ser marcado como "Realizado", "Cancelado" ou
"Falha".
● Fluxo de Comunicação : Controla as interações automatizadas enviadas pelo
WhatsApp Bot, incluindo mensagens de agendamento, confirmação, cancelamento,
remarcação e avaliação.
```
## 2. Requisitos Funcionais


1. **Gestão de Procedimentos**
    ○ O sistema deve permitir que o proprietário adicione, edite e remova
       procedimentos.
    ○ Cada procedimento deve ter um nome, uma descrição, um valor e um tempo
       estimado de duração.
    ○ O sistema deve exibir os procedimentos disponíveis para o cliente durante o
       processo de agendamento.
2. **Gestão de Agenda**
    ○ O sistema deve permitir que o proprietário defina horários disponíveis para
       cada procedimento.
    ○ O sistema deve permitir que o proprietário atualize a agenda, incluindo a
       alteração ou remoção de horários disponíveis.
    ○ O sistema deve bloquear automaticamente os horários já ocupados pelos
       agendamentos.
    ○ O sistema deve mostrar aos clientes apenas os horários disponíveis para
       cada procedimento durante o agendamento.
3. **Agendamento de Procedimentos**
    ○ O cliente deve poder visualizar os procedimentos disponíveis e selecionar um
       deles para agendar.
    ○ O cliente deve poder visualizar os horários disponíveis para o procedimento
       selecionado.
    ○ O cliente deve ser capaz de confirmar o agendamento.
    ○ O agendamento deve ser registrado com status **Pendente** até a confirmação
       do cliente.
4. **Confirmação de Agendamento**
    ○ Após o cliente confirmar o horário e o procedimento, o status do
       agendamento deve ser alterado para **Confirmado**.
    ○ O sistema deve enviar uma mensagem de confirmação ao cliente via
       WhatsApp, informando sobre o agendamento.
5. **Cancelamento e Remarcação de Agendamentos**
    ○ O cliente deve poder cancelar ou remarcado um agendamento.
    ○ O sistema deve alterar o status do agendamento para **Cancelado** ou
       **Remarcado** conforme a ação do cliente.
    ○ O cliente deve receber uma confirmação de cancelamento ou remarcação via
       WhatsApp.
6. **Gestão de Atendimento**
    ○ O proprietário deve poder visualizar todos os agendamentos confirmados e
       seus respectivos status.
    ○ O sistema deve permitir ao proprietário marcar o status do atendimento como
       **Realizado** , **Cancelado** ou **Falha**.
7. **Avaliação de Atendimento**
    ○ Após o atendimento, o cliente deve ser solicitado a avaliar o atendimento
       realizado.
    ○ O cliente deve poder atribuir uma nota e escrever um comentário sobre o
       atendimento.
    ○ O sistema deve permitir que o proprietário visualize as avaliações dos
       clientes.
8. **Comunicação Automatizada via WhatsApp**


```
○ O sistema deve permitir o envio automatizado de mensagens via WhatsApp
durante o processo de agendamento, confirmação, cancelamento,
remarcação e avaliação.
○ O sistema deve permitir que o cliente interaja com o WhatsApp Bot para
agendar, confirmar, cancelar ou remarcado agendamentos.
○ O WhatsApp Bot deve notificar o cliente sobre o status do agendamento
(pendente, confirmado, cancelado, etc.) e lembretes sobre o atendimento.
```
9. **Gestão de Fluxos de Comunicação**
    ○ O sistema deve permitir ao proprietário configurar mensagens automáticas
       em pontos específicos do processo (ex: lembretes de agendamento,
       confirmações, e avaliações).
    ○ O proprietário deve ser capaz de visualizar e monitorar os fluxos de
       comunicação, assegurando que os clientes recebam as mensagens
       corretamente.

## 3. Requisitos Não Funcionais

1. **Desempenho**
    ○ O sistema deve ser capaz de processar agendamentos e interações com os
       clientes de forma eficiente, com tempo de resposta inferior a 5 segundos
       para ações críticas (ex: confirmação de agendamento).
    ○ O sistema deve ser capaz de gerenciar pelo menos 100 agendamentos
       simultâneos sem degradação significativa de desempenho.
2. **Escalabilidade**
    ○ O sistema deve ser escalável para suportar um aumento no número de
       clientes, agendamentos e interações sem necessidade de reescrever a
       arquitetura.
    ○ O sistema deve ser capaz de expandir para incluir novos procedimentos e
       integrações no futuro.
3. **Disponibilidade**
    ○ O sistema deve garantir uma disponibilidade mínima de 99% durante o
       horário de funcionamento.
    ○ O sistema deve ser resiliente a falhas temporárias de integração com o
       WhatsApp, com uma estratégia de fallback caso ocorra uma falha na API.
4. **Segurança**
    ○ O sistema deve garantir a segurança das informações dos clientes, utilizando
       criptografia para dados sensíveis como nome, telefone e detalhes de
       agendamentos.
    ○ O sistema deve utilizar autenticação e controle de acesso para permitir que
       apenas o proprietário tenha permissões para alterar procedimentos, agenda
       e visualizar avaliações.
5. **Usabilidade**
    ○ O sistema deve ser fácil de usar, com uma interface clara e simples tanto
       para o proprietário quanto para os clientes.


```
○ O WhatsApp Bot deve ser intuitivo e responder às interações dos clientes de
forma amigável e sem erros de entendimento.
```
6. **Compatibilidade**
    ○ O sistema de agendamento via WhatsApp deve ser compatível com as
       versões mais recentes do WhatsApp Business e funcionar sem falhas em
       dispositivos móveis e computadores.
    ○ O sistema deve ser acessível por meio de navegadores populares (Google
       Chrome, Firefox, Safari, etc.).
7. **Manutenibilidade**
    ○ O código do sistema deve ser bem documentado, facilitando a manutenção e
       futuras atualizações.
    ○ O sistema deve ser projetado para permitir a inclusão de novos
       procedimentos, a alteração de fluxos de comunicação e o ajuste de
       funcionalidades sem grandes reestruturações.
8. **Privacidade**
    ○ O sistema deve garantir que os dados dos clientes sejam armazenados de
       forma segura e conforme as leis de proteção de dados, como a LGPD (Lei
       Geral de Proteção de Dados Pessoais).
    ○ O sistema deve permitir que os clientes solicitem a exclusão de seus dados
       pessoais a qualquer momento.
9. **Testabilidade**
    ○ O sistema deve ser projetado para ser facilmente testado, com cobertura de
       testes unitários e testes de integração para garantir o bom funcionamento do
       processo de agendamento e a comunicação via WhatsApp.
    ○ O sistema deve permitir testes em diferentes cenários de fluxo de
       comunicação (ex: agendamento bem-sucedido, agendamento cancelado,
       etc.).
10. **Acessibilidade**
    ○ O sistema deve ser acessível a pessoas com deficiência, seguindo as
       diretrizes de acessibilidade para interfaces web.
    ○ As mensagens enviadas pelo WhatsApp Bot devem ser claras e não dep
    ○ endem de recursos visuais para compreensão.

### 4. Casos de uso

**1. Iniciar Agendamento
Ator** : Cliente, WhatsAppBot
**Descrição** : O cliente inicia o processo de agendamento por meio do WhatsAppBot. O bot
solicita ao cliente que selecione o procedimento desejado para agendar. Após a seleção, o
sistema verifica a disponibilidade e sugere horários ao cliente.
**Fluxo de Eventos** :
    1. O cliente envia uma mensagem para iniciar o agendamento.
    2. O WhatsAppBot solicita ao cliente que escolha o procedimento.
    3. O cliente escolhe o procedimento.


4. O WhatsAppBot consulta a agenda e sugere horários disponíveis.
5. O cliente escolhe o horário preferido.
**2. Escolher Procedimento
Ator** : Cliente, WhatsAppBot
**Descrição** : O cliente escolhe o procedimento desejado entre os disponíveis. O
WhatsAppBot apresenta uma lista de procedimentos e solicita a escolha.
**Fluxo de Eventos** :
1. O cliente recebe uma lista de procedimentos.
2. O cliente seleciona o procedimento desejado.
3. O WhatsAppBot confirma a escolha do cliente e sugere horários.
**3. Visualizar Horários Disponíveis
Ator** : Cliente, WhatsAppBot
**Descrição** : O cliente solicita os horários disponíveis para o procedimento selecionado. O
WhatsAppBot consulta a agenda e exibe os horários disponíveis.
**Fluxo de Eventos** :
1. O cliente solicita horários disponíveis.
2. O WhatsAppBot consulta a agenda e retorna com os horários disponíveis.
3. O cliente escolhe um horário.
**4. Confirmar Agendamento
Ator** : Cliente, WhatsAppBot, ControladorDeAgendamento
**Descrição** : O cliente confirma o agendamento após selecionar o procedimento e o horário.
O WhatsAppBot confirma a reserva e o agendamento é registrado.
**Fluxo de Eventos** :
1. O cliente escolhe o horário e confirma o agendamento.
2. O WhatsAppBot confirma o agendamento com o cliente.
3. O ControladorDeAgendamento registra o agendamento na agenda.
4. O sistema envia uma mensagem de confirmação ao cliente.


**5. Cancelar Agendamento
Ator** : Cliente, WhatsAppBot, ControladorDeAgendamento
**Descrição** : O cliente solicita o cancelamento de um agendamento existente. O
WhatsAppBot confirma a solicitação e o agendamento é cancelado.
**Fluxo de Eventos** :
    1. O cliente solicita o cancelamento do agendamento.
    2. O WhatsAppBot solicita confirmação para o cancelamento.
    3. O cliente confirma o cancelamento.
    4. O ControladorDeAgendamento altera o status do agendamento para "Cancelado".
    5. O sistema envia uma mensagem de cancelamento ao cliente.
**6. Remarcar Agendamento
Ator** : Cliente, WhatsAppBot, ControladorDeAgendamento
**Descrição** : O cliente solicita a remarcação de um agendamento para um novo horário. O
WhatsAppBot verifica a disponibilidade e realiza a remarcação.
**Fluxo de Eventos** :
    1. O cliente solicita a remarcação de um agendamento.
    2. O WhatsAppBot solicita o novo horário.
    3. O cliente escolhe um novo horário.
    4. O ControladorDeAgendamento altera o agendamento na agenda.
    5. O sistema envia uma mensagem de confirmação ao cliente.
**7. Receber Lembrete de Agendamento
Ator** : Cliente, WhatsAppBot
**Descrição** : O cliente recebe um lembrete automático de seu agendamento, informado pelo
WhatsAppBot.
**Fluxo de Eventos** :
    1. O WhatsAppBot envia um lembrete de agendamento próximo ao cliente.
    2. O cliente confirma o recebimento do lembrete.
**8. Avaliar Agendamento ou Atendimento
Ator** : Cliente, ControladorDeAvaliacao


**Descrição** : Após a conclusão do procedimento agendado, o cliente pode avaliar o
agendamento ou o atendimento.
**Fluxo de Eventos** :

1. O cliente recebe um pedido de avaliação.
2. O cliente fornece uma nota de 0 a 5 e, opcionalmente, um feedback.
3. O sistema registra a avaliação para o agendamento ou atendimento.
**9. Gerenciar Procedimentos
Ator** : Proprietário, GestorDeProcedimentos
**Descrição** : O proprietário pode adicionar ou remover procedimentos da lista de opções
disponíveis para agendamento.
**Fluxo de Eventos** :
1. O proprietário acessa o GestorDeProcedimentos.
2. O proprietário adiciona ou remove procedimentos conforme necessário.
3. O sistema atualiza a lista de procedimentos.
**10. Gerenciar Agenda
Ator** : Proprietário, GestorDeAgenda
**Descrição** : O proprietário define e gerencia a agenda da empresa, incluindo horários
disponíveis para cada procedimento.
**Fluxo de Eventos** :
1. O proprietário acessa o GestorDeAgenda.
2. O proprietário define os horários disponíveis para a agenda.
3. O sistema registra a agenda e a disponibiliza para os clientes.
**11. Personalizar Fluxos de Comunicação
Ator** : Proprietário, GestorDeComunicacao
**Descrição** : O proprietário personaliza mensagens automáticas enviadas para os clientes
durante o agendamento e o atendimento.
**Fluxo de Eventos** :
1. O proprietário acessa o GestorDeComunicacao.


2. O proprietário personaliza as mensagens automáticas.
3. O sistema envia as mensagens personalizadas conforme o fluxo de comunicação.

### 5. Épicos

#### Épico 1: Agendamento Automatizado

Este épico abrange todos os aspectos do agendamento automatizado via WhatsApp,
incluindo iniciar o processo, escolher procedimentos, visualizar horários, e gerenciar o
agendamento.
**História 1.1: Iniciar Agendamento**
● **Tarefa 1.1.1** : Implementar o método iniciarAgendamento no
WhatsAppBotImpl.
● **Tarefa 1.1.2** : Criar o fluxo de comunicação inicial para informar ao cliente que o
agendamento foi iniciado.
**História 1.2: Escolher Procedimento**
● **Tarefa 1.2.1** : Implementar a funcionalidade de exibição de procedimentos
disponíveis.
● **Tarefa 1.2.2** : Desenvolver a interação do bot para que o cliente possa selecionar um
procedimento.
**História 1.3: Visualizar Horários Disponíveis**
● **Tarefa 1.3.1** : Criar integração entre o GestorDeAgenda e o WhatsAppBotImpl
para exibir horários disponíveis.
● **Tarefa 1.3.2** : Ajustar o comportamento do bot para retornar os horários disponíveis
ao cliente.
**História 1.4: Confirmar Agendamento**
● **Tarefa 1.4.1** : Implementar a funcionalidade de confirmação de agendamento.
● **Tarefa 1.4.2** : Criar mensagens de confirmação e feedback para o cliente.
**História 1.5: Cancelar Agendamento**
● **Tarefa 1.5.1** : Implementar a funcionalidade de cancelamento de agendamento.
● **Tarefa 1.5.2** : Gerar notificações de cancelamento e permitir remoção de
agendamentos na agenda.

#### Épico 2: Gestão de Procedimentos

Este épico abrange todas as funcionalidades relacionadas ao gerenciamento de
procedimentos e valores.
**História 2.1: Cadastrar Procedimento**


● **Tarefa 2.1.1** : Criar funcionalidade no GestorDeProcedimentos para configurar
novos procedimentos.
● **Tarefa 2.1.2** : Definir valor, tempo estimado e tipo para cada procedimento.
**História 2.2: Remover Procedimento**
● **Tarefa 2.2.1** : Implementar funcionalidade para remover procedimentos da lista no
GestorDeProcedimentos.
**História 2.3: Verificar Disponibilidade de Procedimentos**
● **Tarefa 2.3.1** : Implementar a funcionalidade no GestorDeProcedimentos para
verificar a disponibilidade de um procedimento.

#### Épico 3: Comunicação e Fluxos de Trabalho

Este épico envolve a configuração e gerenciamento de mensagens automatizadas e fluxos
de comunicação com o cliente.
**História 3.1: Configurar Canal de Comunicação**
● **Tarefa 3.1.1** : Implementar a configuração do canal de comunicação no
GestorDeComunicacao.
● **Tarefa 3.1.2** : Desenvolver funcionalidades para personalizar mensagens
automáticas.
**História 3.2: Gerenciar Fluxo de Comunicação**
● **Tarefa 3.2.1** : Criar a classe FluxoComunicacao e os fluxos de trabalho para os
agendamentos e atendimentos.
● **Tarefa 3.2.2** : Desenvolver a integração com a classe Checkpoint para rastrear
pontos de comunicação.
**História 3.3: Enviar Lembretes de Agendamento**
● **Tarefa 3.3.1** : Criar a lógica para envio de lembretes automatizados.
● **Tarefa 3.3.2** : Integrar lembretes com o bot e definir horários para envio.

#### Épico 4: Avaliação de Atendimento e Agendamento

Este épico trata das funcionalidades de avaliação após o agendamento ou atendimento.
**História 4.1: Avaliar Atendimento**
● **Tarefa 4.1.1** : Implementar a avaliação de atendimento pelo cliente.
● **Tarefa 4.1.2** : Integrar a avaliação com o sistema de feedback.
**História 4.2: Avaliar Agendamento**


```
● Tarefa 4.2.1 : Implementar a avaliação do agendamento feito pelo cliente.
● Tarefa 4.2.2 : Registrar as avaliações no sistema.
```
#### Épico 5: Gestão de Atendimento

Este épico abrange os processos de atendimento, incluindo falhas e status do atendimento.
**História 5.1: Iniciar Atendimento**
● **Tarefa 5.1.1** : Implementar a função iniciarAtendimento na classe
Atendimento.
● **Tarefa 5.1.2** : Criar status de atendimento (Realizado, Cancelado, Falha).
**História 5.2: Gerenciar Falhas no Atendimento**
● **Tarefa 5.2.1** : Implementar o método de notificação de falhas de atendimento.
● **Tarefa 5.2.2** : Integrar falhas com o fluxo de comunicação do bot.

### 6. Ondas de desenvolvimento

#### Onda 1: Fundamentos do Agendamento e Comunicação

Objetivo: Estabelecer as bases do agendamento automatizado, configuração de
procedimentos e comunicação com o cliente.
**Epicos envolvidos:**
● **Épico 1: Agendamento Automatizado**
○ **História 1.1** : Iniciar Agendamento
○ **História 1.2** : Escolher Procedimento
○ **História 1.3** : Visualizar Horários Disponíveis
● **Épico 2: Gestão de Procedimentos**
○ **História 2.1** : Cadastrar Procedimento
○ **História 2.2** : Remover Procedimento
● **Épico 3: Comunicação e Fluxos de Trabalho**
○ **História 3.1** : Configurar Canal de Comunicação
○ **História 3.2** : Gerenciar Fluxo de Comunicação
**Tarefas da Onda 1:**
● Inicializar a integração do WhatsAppBot para iniciar e interagir com o cliente no
processo de agendamento.
● Permitir que os clientes escolham os procedimentos disponíveis.
● Mostrar horários disponíveis baseados nas configurações de agendamento.
● Criar funcionalidades para cadastrar e remover procedimentos.
● Configurar a comunicação inicial via WhatsApp, incluindo mensagens automáticas e
interações com o cliente.


● Gerenciar o fluxo básico de comunicação até a confirmação de agendamento.
**Objetivo da Onda 1:**
● O cliente deve ser capaz de iniciar um agendamento, escolher um procedimento,
visualizar horários e confirmar o agendamento.
● O sistema deve permitir o cadastro de novos procedimentos e realizar o
gerenciamento básico de fluxos de comunicação.

#### Onda 2: Funcionalidades Avançadas e Avaliação de Atendimento

Objetivo: Adicionar as funcionalidades de confirmação, cancelamento, lembretes, avaliação
e gestão de atendimentos.
**Epicos envolvidos:**
● **Épico 1: Agendamento Automatizado**
○ **História 1.4** : Confirmar Agendamento
○ **História 1.5** : Cancelar Agendamento
● **Épico 3: Comunicação e Fluxos de Trabalho**
○ **História 3.3** : Enviar Lembretes de Agendamento
● **Épico 4: Avaliação de Atendimento e Agendamento**
○ **História 4.1** : Avaliar Atendimento
○ **História 4.2** : Avaliar Agendamento
● **Épico 5: Gestão de Atendimento**
○ **História 5.1** : Iniciar Atendimento
○ **História 5.2** : Gerenciar Falhas no Atendimento
**Tarefas da Onda 2:**
● Finalizar a funcionalidade de confirmação e cancelamento de agendamentos.
● Implementar o envio de lembretes automatizados para clientes sobre os
agendamentos.
● Permitir que o cliente avalie o atendimento e o agendamento após a realização do
serviço.
● Configurar o atendimento pós-agendamento, com status de realização e falhas.
● Garantir que falhas de atendimento sejam tratadas e notificadas.
**Objetivo da Onda 2:**
● O cliente deve poder cancelar e confirmar agendamentos.
● O sistema deve enviar lembretes automáticos sobre agendamentos e possibilitar a
avaliação dos atendimentos.
● A gestão de atendimento deve estar pronta, com status de falha e registro de
feedbacks de clientes.



