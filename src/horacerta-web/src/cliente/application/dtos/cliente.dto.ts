export type ClienteDto = {
  id: string;
  nome: string;
  telefone: string;
};

export type CriarClienteRequest = {
  nome: string;
  telefone: string;
};
