import { bffApiClient } from '@/shared/infrastructure/http/axios-client';
import type {
  AlterarEstadoAtendimentoRequest,
  RegistrarAtendimentoRequest,
} from '../../application/dtos/atendimento.dto';
import {
  alterarEstadoAtendimentoApi,
  listarAtendimentosApi,
  registrarAtendimentoApi,
} from './atendimento.api';

export class AtendimentoRepository {
  listar(proprietarioId: string) {
    return listarAtendimentosApi(bffApiClient, proprietarioId).then((r) => r.data);
  }

  registrar(agendamentoId: string, data: RegistrarAtendimentoRequest) {
    return registrarAtendimentoApi(bffApiClient, agendamentoId, data).then((r) => r.data);
  }

  alterarEstado(
    proprietarioId: string,
    atendimentoId: string,
    data: AlterarEstadoAtendimentoRequest,
  ) {
    return alterarEstadoAtendimentoApi(bffApiClient, proprietarioId, atendimentoId, data).then(
      (r) => r.data,
    );
  }
}

export const atendimentoRepository = new AtendimentoRepository();
