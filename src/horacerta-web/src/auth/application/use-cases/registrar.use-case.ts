import type { IAuthRepository } from '../../domain/repositories/auth.repository';
import type { RegistrarRequest } from '../dtos/auth.dto';

export class RegistrarUseCase {
  constructor(private readonly repository: IAuthRepository) {}

  execute(data: RegistrarRequest) {
    return this.repository.registrar(data);
  }
}
