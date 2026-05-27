import { useAuthStore } from '@/auth/presentation/stores/auth.store';
import { bffApiClient } from './axios-client';

let registered = false;

/** Redireciona para login quando o BFF retorna 401 (sessão expirada ou inválida). */
export function registerBffAuthInterceptor() {
  if (registered || typeof window === 'undefined') return;
  registered = true;

  bffApiClient.interceptors.response.use(
    (response) => response,
    (error: unknown) => {
      const status =
        error &&
        typeof error === 'object' &&
        'response' in error &&
        (error as { response?: { status?: number } }).response?.status;

      if (status === 401) {
        useAuthStore.getState().clearSession();
        const path = window.location.pathname;
        if (!path.startsWith('/login') && !path.startsWith('/registrar')) {
          const redirect = encodeURIComponent(path);
          window.location.assign(`/login?redirect=${redirect}`);
        }
      }
      return Promise.reject(error);
    },
  );
}
