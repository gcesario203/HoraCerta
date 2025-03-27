using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoraCerta.Dominio;

public record IdEntidade
{
    public string Valor { get; private set; }

    public IdEntidade()
    {
        Valor = Guid.NewGuid().ToString();
    }

    public IdEntidade(string valor) { Valor = valor; }
}
