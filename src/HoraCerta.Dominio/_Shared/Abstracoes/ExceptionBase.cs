using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoraCerta.Dominio._Shared.Abstracoes;

public class ExceptionBase : Exception
{
    protected string Mensagem { get; private set; }

    public ExceptionBase(string mensagem)
    {
        this.Mensagem = mensagem;
    }
}
