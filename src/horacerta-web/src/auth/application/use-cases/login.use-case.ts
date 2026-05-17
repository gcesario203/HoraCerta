import type { IAuthRepository } from '../../domain/repositories/auth.repository';
import type { LoginRequest } from '../dtos/auth.dto';

export class LoginUseCase {
  constructor(private readonly repository: IAuthRepository) {}

  execute(data: LoginRequest) {
    return this.repository.login(data);
  }
}
