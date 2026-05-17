import { create } from 'zustand';

type AuthState = {
  proprietarioId: string | null;
  isAuthenticated: boolean;
  setSession: (proprietarioId: string) => void;
  clearSession: () => void;
};

export const useAuthStore = create<AuthState>((set) => ({
  proprietarioId: null,
  isAuthenticated: false,
  setSession: (proprietarioId) => set({ proprietarioId, isAuthenticated: true }),
  clearSession: () => set({ proprietarioId: null, isAuthenticated: false }),
}));
