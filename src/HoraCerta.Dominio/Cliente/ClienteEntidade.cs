using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoraCerta.Dominio.Cliente;

public class ClienteEntidade : EntidadeBase<ClienteEntidade>
{
    public string Nome { get; private set; }

    public string Telefone { get; private set; }

    public ClienteEntidade(string nome, string telefone) : base(new ValidadorCliente())
    {
        Nome = nome;
        Telefone = telefone;

        _validador.Validar(this);
    }

    public void AtualizarNome(string nome)
    {
        Nome = nome;

        _validador.Validar(this);

        Atualizar();
    }

    public void AtualizarTelefone(string telefone)
    {
        Telefone = telefone;

        _validador.Validar(this);

        Atualizar();
    }
}
