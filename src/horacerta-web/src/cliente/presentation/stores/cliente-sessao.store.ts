import { create } from 'zustand';

type ClienteSessaoState = {
  clienteId: string | null;
  proprietarioId: string | null;
  setSessao: (clienteId: string, proprietarioId: string) => void;
  clearSessao: () => void;
};

export const useClienteSessaoStore = create<ClienteSessaoState>((set) => ({
  clienteId: null,
  proprietarioId: null,
  setSessao: (clienteId, proprietarioId) => set({ clienteId, proprietarioId }),
  clearSessao: () => set({ clienteId: null, proprietarioId: null }),
}));
