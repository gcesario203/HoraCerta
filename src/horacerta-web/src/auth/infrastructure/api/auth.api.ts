import type { AxiosInstance } from 'axios';
import type {
  LoginRequest,
  LoginResponse,
  RegistrarRequest,
  RegistrarResponse,
} from '../../application/dtos/auth.dto';

export async function loginApi(client: AxiosInstance, data: LoginRequest) {
  const { data: res } = await client.post<LoginResponse>('/api/bff/auth/login', data);
  return res;
}

export async function registrarApi(client: AxiosInstance, data: RegistrarRequest) {
  const { data: res } = await client.post<RegistrarResponse>('/api/bff/auth/registrar', data);
  return res;
}

export async function logoutApi(client: AxiosInstance) {
  await client.post('/api/bff/auth/logout');
}
