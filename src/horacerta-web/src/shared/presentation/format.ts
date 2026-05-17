import dayjs from 'dayjs';
import 'dayjs/locale/pt-br';

dayjs.locale('pt-br');

export function formatarDataHora(iso: string): string {
  return dayjs(iso).format('DD/MM/YYYY HH:mm');
}

export function formatarMoeda(valor: number): string {
  return valor.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
}

export function labelEstado(estado: string): string {
  const map: Record<string, string> = {
    PENDENTE: 'Pendente',
    CONFIRMADO: 'Confirmado',
    CANCELADO: 'Cancelado',
    FINALIZADO: 'Finalizado',
    REALIZADO: 'Realizado',
    FALHA: 'Falha',
    DISPONIVEL: 'Disponível',
    OCUPADO: 'Ocupado',
    ATIVO: 'Ativo',
    INATIVO: 'Inativo',
  };
  return map[estado.toUpperCase()] ?? estado;
}
