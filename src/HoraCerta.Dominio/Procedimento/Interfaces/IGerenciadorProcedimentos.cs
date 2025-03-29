using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoraCerta.Dominio.Procedimento;

public interface IGerenciadorProcedimentos
{
    void CriarProcedimento(string nome, decimal valor, TimeSpan tempoEstimado);

    void AlterarProcedimento(IdEntidade id, string nome, decimal valor, TimeSpan tempoEstimado); 

    void InativarProcedimento(ProcedimentoEntidade procedimentoEntidade);

    void AtivarProcedimento(ProcedimentoEntidade procedimentoEntidade);

    ICollection<ProcedimentoEntidade> RecuperarProcedimentos();

    ProcedimentoEntidade BuscarProcedimentoPorId(IdEntidade idEntidade);
}
