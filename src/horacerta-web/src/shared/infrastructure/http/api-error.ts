export class ApiError extends Error {
  constructor(
    message: string,
    readonly status?: number,
  ) {
    super(message);
    this.name = 'ApiError';
  }
}

export function getErrorMessage(error: unknown): string {
  if (error instanceof ApiError) return error.message;
  if (error instanceof Error) return error.message;
  return 'Ocorreu um erro inesperado.';
}

export function extractApiMessage(error: unknown): string {
  if (error && typeof error === 'object' && 'response' in error) {
    const res = (error as { response?: { data?: unknown } }).response;
    const data = res?.data;
    if (data && typeof data === 'object') {
      const msg =
        'mensagem' in data && typeof data.mensagem === 'string'
          ? data.mensagem
          : 'title' in data && typeof data.title === 'string'
            ? data.title
            : null;
      if (msg) return msg;
    }
  }
  return getErrorMessage(error);
}
