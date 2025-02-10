using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HoraCerta.Dominio.Cliente;

public class ValidadorTelefone : IValidadorEspecificacao<ClienteEntidade>
{
    public void Valido(ClienteEntidade entidade)
    {
        if (string.IsNullOrEmpty(entidade.Telefone))
            throw new EntidadeInvalidadeExcessao("Telefone é obrigatorio");

        string padrao = @"^(\+55\s?)?\(?\d{2}\)?\s?\d{4,5}-?\d{4}$";

        if (!Regex.IsMatch(entidade.Telefone, padrao))
            throw new EntidadeInvalidadeExcessao("Telefone com formato inválido");
    }
}
