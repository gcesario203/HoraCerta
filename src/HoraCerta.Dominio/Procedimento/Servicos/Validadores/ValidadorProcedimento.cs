using HoraCerta.Dominio._Shared.Abstracoes;
using HoraCerta.Dominio._Shared.Interfaces;
using HoraCerta.Dominio.Procedimento.Validadores;
using HoraCerta.Dominio.Shared.Abstracoes;
using HoraCerta.Dominio.Shared.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoraCerta.Dominio.Procedimento;

public class ValidadorProcedimento : ServicoValidacaoBase<Procedimento>
{
    public ValidadorProcedimento() : base(new List<IValidadorEspecificacao<Procedimento>>()
    {
        new ValidadorNome(),
        new ValidadorValor(),
        new ValidadorTempoEstimado()
    })
    {
        
    }
}
