import { create } from 'zustand';

type AuthState = {
  proprietarioId: string | null;
  isAuthenticated: boolean;
  /** Cookie/session já lido (evita ações antes da hidratação). */
  hydrated: boolean;
  setSession: (proprietarioId: string) => void;
  clearSession: () => void;
  setHydrated: () => void;
};

export const useAuthStore = create<AuthState>((set) => ({
  proprietarioId: null,
  isAuthenticated: false,
  hydrated: false,
  setSession: (proprietarioId) =>
    set({ proprietarioId, isAuthenticated: true, hydrated: true }),
  clearSession: () =>
    set({ proprietarioId: null, isAuthenticated: false, hydrated: true }),
  setHydrated: () => set({ hydrated: true }),
}));
