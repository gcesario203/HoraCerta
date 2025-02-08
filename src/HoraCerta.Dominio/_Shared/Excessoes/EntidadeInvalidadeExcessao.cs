using HoraCerta.Dominio._Shared.Abstracoes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoraCerta.Dominio._Shared.Excessoes;

public class EntidadeInvalidadeExcessao : ExceptionBase
{
    public EntidadeInvalidadeExcessao(string mensagem) : base(mensagem)
    {
        
    }
}
