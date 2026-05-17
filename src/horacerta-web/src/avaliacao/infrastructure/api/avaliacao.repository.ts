import { bffApiClient, publicApiClient } from '@/shared/infrastructure/http/axios-client';
import type { AvaliarAgendamentoRequest } from '../../application/dtos/avaliacao.dto';
import { avaliarAgendamentoApi, obterAvaliacaoApi } from './avaliacao.api';

export class AvaliacaoRepository {
  avaliar(clienteId: string, agendamentoId: string, data: AvaliarAgendamentoRequest) {
    return avaliarAgendamentoApi(publicApiClient, clienteId, agendamentoId, data).then(
      (r) => r.data,
    );
  }

  obter(proprietarioId: string, agendamentoId: string) {
    return obterAvaliacaoApi(bffApiClient, proprietarioId, agendamentoId).then((r) => r.data);
  }
}

export const avaliacaoRepository = new AvaliacaoRepository();
