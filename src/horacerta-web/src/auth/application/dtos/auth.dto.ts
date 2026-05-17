export type LoginRequest = { email: string; senha: string };
export type LoginResponse = { proprietarioId: string };

export type RegistrarRequest = {
  proprietarioId?: string | null;
  nomeEstabelecimento?: string | null;
  email: string;
  senha: string;
};

export type RegistrarResponse = { proprietarioId: string };
