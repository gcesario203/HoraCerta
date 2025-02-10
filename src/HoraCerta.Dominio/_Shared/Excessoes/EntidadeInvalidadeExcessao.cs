using HoraCerta.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoraCerta.Dominio;

public class EntidadeInvalidadeExcessao : ExceptionBase
{
    public EntidadeInvalidadeExcessao(string mensagem) : base(mensagem)
    {
        
    }
}
