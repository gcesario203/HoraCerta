using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoraCerta.Dominio;

public class OperacaoInvalidaExcessao : ExceptionBase
{
    public OperacaoInvalidaExcessao(string mensagem) : base(mensagem)
    {
    }
}
