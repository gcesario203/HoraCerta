using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoraCerta.Dominio.Atendimento;

public interface IEstadoAtendimento
{
    EstadoAtendimento EstadoAtual();

    IEstadoAtendimento AlterarEstado(AtendimentoEntidade entidade, EstadoAtendimento estado);
}
