using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoraCerta.Dominio.Cliente;

public class ValidadorNome : IValidadorEspecificacao<ClienteEntidade>
{

    public void Valido(ClienteEntidade entidade)
    {
        if (string.IsNullOrEmpty(entidade.Nome))
            throw new EntidadeInvalidadeExcessao("Nome é obrigatório");

        if (entidade.Nome.Length <= 0 || entidade.Nome.Length > 150)
            throw new EntidadeInvalidadeExcessao("Nome com tamanho invalido");
    }
}
