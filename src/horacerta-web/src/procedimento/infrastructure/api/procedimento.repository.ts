import { bffApiClient, publicApiClient } from '@/shared/infrastructure/http/axios-client';
import type { CriarProcedimentoRequest } from '../../application/dtos/procedimento.dto';
import type { IProcedimentoRepository } from '../../domain/repositories/procedimento.repository';
import {
  criarProcedimentoApi,
  inativarProcedimentoApi,
  listarProcedimentosApi,
} from './procedimento.api';

export class ProcedimentoRepository implements IProcedimentoRepository {
  listarAtivos(proprietarioId: string) {
    return listarProcedimentosApi(publicApiClient, proprietarioId).then((r) => r.data);
  }

  criar(proprietarioId: string, data: CriarProcedimentoRequest) {
    return criarProcedimentoApi(bffApiClient, proprietarioId, data).then((r) => r.data);
  }

  inativar(proprietarioId: string, procedimentoId: string) {
    return inativarProcedimentoApi(bffApiClient, proprietarioId, procedimentoId).then(
      (r) => r.data,
    );
  }
}

export const procedimentoRepository = new ProcedimentoRepository();
