import { create } from 'zustand';
import { THEME_STORAGE_KEY, type ThemeMode } from '../theme/tokens';

type ThemeState = {
  mode: ThemeMode;
  hydrated: boolean;
  setMode: (mode: ThemeMode) => void;
  toggle: () => void;
  hydrate: () => void;
};

function applyDomTheme(mode: ThemeMode) {
  if (typeof document === 'undefined') return;
  document.documentElement.setAttribute('data-theme', mode);
}

export const useThemeStore = create<ThemeState>((set, get) => ({
  mode: 'light',
  hydrated: false,
  setMode: (mode) => {
    applyDomTheme(mode);
    try {
      localStorage.setItem(THEME_STORAGE_KEY, mode);
    } catch {
      /* ignore */
    }
    set({ mode });
  },
  toggle: () => {
    const next = get().mode === 'light' ? 'dark' : 'light';
    get().setMode(next);
  },
  hydrate: () => {
    let mode: ThemeMode = 'light';
    try {
      const stored = localStorage.getItem(THEME_STORAGE_KEY);
      if (stored === 'dark' || stored === 'light') mode = stored;
    } catch {
      /* ignore */
    }
    applyDomTheme(mode);
    set({ mode, hydrated: true });
  },
}));
