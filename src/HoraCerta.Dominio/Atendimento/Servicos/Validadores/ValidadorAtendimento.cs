using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoraCerta.Dominio.Atendimento;

public class ValidadorAtendimento : ServicoValidacaoBase<AtendimentoEntidade>
{
    public ValidadorAtendimento() : base(new List<IValidadorEspecificacao<AtendimentoEntidade>>
    {
        new ValidadorAgendamento(),
        new ValidadorValorNegociado()
    })
    {
    }
}
