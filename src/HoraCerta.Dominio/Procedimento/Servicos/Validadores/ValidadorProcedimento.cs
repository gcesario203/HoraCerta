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

public class ValidadorProcedimento : ValidadorBase<Procedimento>
{
    public ValidadorProcedimento() : base()
    {
        
    }
}
