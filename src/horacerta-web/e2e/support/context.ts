export type BddContext = {
  email: string;
  senha: string;
  nomeEstabelecimento: string;
  proprietarioId: string;
  nomeProcedimento: string;
  nomeCliente: string;
  telefoneCliente: string;
};

export const ctx: BddContext = {
  email: '',
  senha: 'Senha123!',
  nomeEstabelecimento: '',
  proprietarioId: '',
  nomeProcedimento: '',
  nomeCliente: '',
  telefoneCliente: '11999999999',
};

export function resetContext(): void {
  const id = Date.now();
  ctx.email = `bdd-${id}@test.local`;
  ctx.senha = 'Senha123!';
  ctx.nomeEstabelecimento = `Estabelecimento BDD ${id}`;
  ctx.nomeProcedimento = `Corte BDD ${id}`;
  ctx.nomeCliente = `Cliente BDD ${id}`;
  ctx.telefoneCliente = '11999999999';
  ctx.proprietarioId = '';
}
