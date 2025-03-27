using NUnit.Framework;
using HoraCerta.Dominio.Cliente;
using System.Linq;
using HoraCerta.Dominio;
using HoraCerta.Dominio.Procedimento;

namespace HoraCerta.Testes.Unitarios.Dominio;

[TestFixture]
public class Cliente
{
    private readonly ProcedimentoEntidade procedimento = new ProcedimentoEntidade("Unhas", 40, TimeSpan.FromHours(1));

    [Test]
    public void CriarCliente_ComDadosValidos_DeveCriarComSucesso()
    {
        // Arrange
        string nome = "João Silva";
        string telefone = "(11) 91234-5678";

        // Act
        var cliente = new ClienteEntidade(nome, telefone);

        // Assert
        Assert.That(cliente.Nome, Is.EqualTo(nome));
        Assert.That(cliente.Telefone, Is.EqualTo(telefone));
    }

    [Test]
    public void CriarCliente_ComTelefoneInvalido_DeveLancarExcecao()
    {
        // Arrange
        string nome = "Maria Souza";
        string telefone = "1234"; // Telefone inválido

        // Act & Assert
        Assert.Catch<EntidadeInvalidadeExcessao>(() => new ClienteEntidade(nome, telefone));
    }

    [Test]
    public void AtualizarNome_DeveModificarNomeEValidar()
    {
        // Arrange
        var cliente = new ClienteEntidade("Pedro Oliveira", "(11) 99999-9999");
        string novoNome = "Pedro Santos";

        // Act
        cliente.AtualizarNome(novoNome);

        // Assert
        Assert.That(cliente.Nome, Is.EqualTo(novoNome));
    }

    [Test]
    public void AtualizarTelefone_ComTelefoneValido_DeveAtualizar()
    {
        // Arrange
        var cliente = new ClienteEntidade("Ana Lima", "(21) 98765-4321");
        string novoTelefone = "+55 21 92345-6789";

        // Act
        cliente.AtualizarTelefone(novoTelefone);

        // Assert
        Assert.That(cliente.Telefone, Is.EqualTo(novoTelefone));
    }

    [Test]
    public void AtualizarTelefone_ComTelefoneInvalido_DeveLancarExcecao()
    {
        // Arrange
        var cliente = new ClienteEntidade("Carlos Mendes", "(31) 99876-5432");
        string telefoneInvalido = "9999";

        // Act & Assert
        Assert.Catch<EntidadeInvalidadeExcessao>(() => cliente.AtualizarTelefone(telefoneInvalido));
    }

    [Test]
    public void ClienteDeveCriarAgendamento()
    {
        var cliente = new ClienteEntidade("Carlos Mendes", "(31) 99876-5432");

        var slot = new SlotHorarioEntidade(DateTime.Now);

        var agendamento = cliente.GerenciadorAgendamentos.IniciarAgendamento(procedimento, slot);

        Assert.That(agendamento.EstadoAtual() == HoraCerta.Dominio.Agendamento.EstadoAgendamento.PENDENTE && slot.Status == StatusSlotAgendamento.RESERVADO);
    }

    [Test]
    public void ClienteDeveCancelarAgendamento()
    {
        var cliente = new ClienteEntidade("Carlos Mendes", "(31) 99876-5432");

        var slot = new SlotHorarioEntidade(DateTime.Now);

        var agendamento = cliente.GerenciadorAgendamentos.IniciarAgendamento(procedimento, slot);

        Assert.That(agendamento.EstadoAtual() == HoraCerta.Dominio.Agendamento.EstadoAgendamento.PENDENTE && slot.Status == StatusSlotAgendamento.RESERVADO);

        cliente.GerenciadorAgendamentos.CancelarAgendamento(agendamento.Id);

        Assert.That(agendamento.EstadoAtual() == HoraCerta.Dominio.Agendamento.EstadoAgendamento.CANCELADO && slot.Status == StatusSlotAgendamento.DISPONIVEL);
    }

    [Test]
    public void ClienteDeveConfirmarAgendamento()
    {
        var cliente = new ClienteEntidade("Carlos Mendes", "(31) 99876-5432");

        var slot = new SlotHorarioEntidade(DateTime.Now);

        var agendamento = cliente.GerenciadorAgendamentos.IniciarAgendamento(procedimento, slot);

        Assert.That(agendamento.EstadoAtual() == HoraCerta.Dominio.Agendamento.EstadoAgendamento.PENDENTE && slot.Status == StatusSlotAgendamento.RESERVADO);

        cliente.GerenciadorAgendamentos.ConfirmarAgendamento(agendamento.Id);

        Assert.That(agendamento.EstadoAtual() == HoraCerta.Dominio.Agendamento.EstadoAgendamento.CONFIRMADO && slot.Status == StatusSlotAgendamento.RESERVADO);
    }

    [Test]
    public void ClienteDeveRemarcarAgendamento()
    {
        var cliente = new ClienteEntidade("Carlos Mendes", "(31) 99876-5432");

        var slot = new SlotHorarioEntidade(DateTime.Now);

        var slo2 = new SlotHorarioEntidade(slot.Inicio.AddHours(2));

        var agendamento = cliente.GerenciadorAgendamentos.IniciarAgendamento(procedimento, slot);

        Assert.That(agendamento.EstadoAtual() == HoraCerta.Dominio.Agendamento.EstadoAgendamento.PENDENTE && slot.Status == StatusSlotAgendamento.RESERVADO);

        cliente.GerenciadorAgendamentos.ConfirmarAgendamento(agendamento.Id);

        var remarcado = cliente.GerenciadorAgendamentos.RemarcarAgendamento(agendamento.Id, slo2);

        Assert.That(agendamento.EstadoAtual() == HoraCerta.Dominio.Agendamento.EstadoAgendamento.REMARCADO && slot.Status == StatusSlotAgendamento.DISPONIVEL);

        Assert.That(agendamento.Id.Valor == remarcado.Reagendamento?.Id.Valor && remarcado.SlotHorario?.Id.Valor == slo2.Id.Valor);

        Assert.That(remarcado.SlotHorario?.Id.Valor == slo2.Id.Valor && slo2.Status == StatusSlotAgendamento.RESERVADO);
    }

    [Test]
    public void Cliente_DeveConverter_ParaDTO()
    {
        var cliente = new ClienteEntidade("Carlos Mendes", "(31) 99876-5432");

        var slot = new SlotHorarioEntidade(DateTime.Now);

        cliente.GerenciadorAgendamentos.IniciarAgendamento(procedimento, slot);

        var clienteDTO = ClienteEntidade.ParaDTO(cliente);

        Assert.That(clienteDTO.Id == cliente.Id.Valor);
        Assert.That(clienteDTO.Nome == cliente.Nome);
        Assert.That(clienteDTO.Telefone == cliente.Telefone);
        Assert.That(clienteDTO.DataCriacao == cliente.DataCriacao);
        Assert.That(clienteDTO.DataAlteracao == cliente.DataAlteracao);
    }

    [Test]
    public void ClientDTO_DeveConverter_ParaEntidade()
    {
        var cliente = new ClienteEntidade("Carlos Mendes", "(31) 99876-5432");

        var slot = new SlotHorarioEntidade(DateTime.Now);

        cliente.GerenciadorAgendamentos.IniciarAgendamento(procedimento, slot);

        var clienteDTO = ClienteEntidade.ParaDTO(cliente);

        var clienteEntidade = ClienteEntidade.ParaEntidade(clienteDTO);

        Assert.That(clienteEntidade.Id.Valor == cliente.Id.Valor);
        Assert.That(clienteEntidade.Nome == cliente.Nome);
        Assert.That(clienteEntidade.Telefone == cliente.Telefone);
        Assert.That(clienteEntidade.DataCriacao == cliente.DataCriacao);
        Assert.That(clienteEntidade.DataAlteracao == cliente.DataAlteracao);
    }
}
