import type { IAuthRepository } from '../../domain/repositories/auth.repository';

export class LogoutUseCase {
  constructor(private readonly repository: IAuthRepository) {}

  execute() {
    return this.repository.logout();
  }
}
