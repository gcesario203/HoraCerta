const moeda = new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' });

const dataHora = new Intl.DateTimeFormat('pt-BR', {
  weekday: 'short',
  day: '2-digit',
  month: 'short',
  hour: '2-digit',
  minute: '2-digit',
});

export function formatarMoeda(valor: number) {
  return moeda.format(valor);
}

export function formatarDataHora(iso: string) {
  return dataHora.format(new Date(iso));
}

export function formatarFaixaPreco(min: number | null, max: number | null) {
  if (min === null || max === null) return null;
  if (min === max) return formatarMoeda(min);
  return `${formatarMoeda(min)} – ${formatarMoeda(max)}`;
}

export function formatarDuracao(minutos: number) {
  if (minutos < 60) return `${minutos} min`;
  const h = Math.floor(minutos / 60);
  const m = minutos % 60;
  return m > 0 ? `${h}h ${m}min` : `${h}h`;
}
