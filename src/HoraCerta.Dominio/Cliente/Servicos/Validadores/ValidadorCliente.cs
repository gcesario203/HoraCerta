using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoraCerta.Dominio.Cliente;

public class ValidadorCliente : ServicoValidacaoBase<ClienteEntidade>
{
    public ValidadorCliente() : base(new List<IValidadorEspecificacao<ClienteEntidade>>
    {
        new ValidadorNome(),
        new ValidadorTelefone()
    })
    {
    }
}
