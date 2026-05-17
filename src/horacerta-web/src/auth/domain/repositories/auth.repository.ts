import type { LoginRequest, LoginResponse, RegistrarRequest, RegistrarResponse } from '../../application/dtos/auth.dto';

export interface IAuthRepository {
  login(data: LoginRequest): Promise<LoginResponse>;
  registrar(data: RegistrarRequest): Promise<RegistrarResponse>;
  logout(): Promise<void>;
}
