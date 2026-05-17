import type { CriarProcedimentoRequest, ProcedimentoDto } from '../../application/dtos/procedimento.dto';

export interface IProcedimentoRepository {
  listarAtivos(proprietarioId: string): Promise<ProcedimentoDto[]>;
  criar(proprietarioId: string, data: CriarProcedimentoRequest): Promise<ProcedimentoDto>;
  inativar(proprietarioId: string, procedimentoId: string): Promise<ProcedimentoDto>;
}
