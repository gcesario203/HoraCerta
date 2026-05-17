import { authRepository } from '../infrastructure/api/auth.repository';
import { LoginUseCase } from './use-cases/login.use-case';
import { LogoutUseCase } from './use-cases/logout.use-case';
import { RegistrarUseCase } from './use-cases/registrar.use-case';

export const loginUseCase = new LoginUseCase(authRepository);
export const registrarUseCase = new RegistrarUseCase(authRepository);
export const logoutUseCase = new LogoutUseCase(authRepository);
