import { createApiClient } from '@/shared/infrastructure/http/axios-client';
import type {
  LoginRequest,
  RegistrarRequest,
} from '../../application/dtos/auth.dto';
import type { IAuthRepository } from '../../domain/repositories/auth.repository';
import { loginApi, logoutApi, registrarApi } from './auth.api';

const bffRoot = createApiClient('', true);

export class AuthRepository implements IAuthRepository {
  login(data: LoginRequest) {
    return loginApi(bffRoot, data);
  }

  registrar(data: RegistrarRequest) {
    return registrarApi(bffRoot, data);
  }

  logout() {
    return logoutApi(bffRoot);
  }
}

export const authRepository = new AuthRepository();
