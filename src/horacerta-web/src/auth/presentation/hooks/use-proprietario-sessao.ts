import { useAuthStore } from '../stores/auth.store';

/** Sessão do proprietário após hidratação do cookie (evita race em reload). */
export function useProprietarioSessao() {
  const proprietarioId = useAuthStore((s) => s.proprietarioId);
  const hydrated = useAuthStore((s) => s.hydrated);

  return {
    proprietarioId,
    hydrated,
    canAct: hydrated && !!proprietarioId,
  };
}
