using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoraCerta.Dominio.Atendimento;

public abstract class EstadoAtendimentoAbstracao : IEstadoAtendimento
{
    public EstadoAtendimento Estado { get; protected set; }

    public abstract IEstadoAtendimento AlterarEstado(AtendimentoEntidade entidade, EstadoAtendimento estado);

    public EstadoAtendimento EstadoAtual()
        => Estado;

}
