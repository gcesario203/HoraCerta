using NUnit.Framework;
using HoraCerta.Dominio.Cliente;
using System;
using HoraCerta.Dominio;

namespace HoraCerta.Testes.Unitarios.Dominio;

[TestFixture]
public class Cliente
{
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
}
