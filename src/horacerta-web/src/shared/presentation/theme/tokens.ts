/** Design tokens HoraCerta — ver docs/frontend/spec.md §4.1 */
export const palette = {
  primary: '#10B981',
  primaryHover: '#059669',
  primaryActive: '#047857',
  accent: '#34D399',
  light: {
    bg: '#F8FAFC',
    surface: '#FFFFFF',
    surfaceMuted: '#F1F5F9',
    border: '#E2E8F0',
    text: '#0F172A',
    textMuted: '#64748B',
    header: '#0F172A',
  },
  dark: {
    bg: '#0F172A',
    surface: '#111827',
    surfaceElevated: '#1F2937',
    border: '#334155',
    text: '#F8FAFC',
    textMuted: '#94A3B8',
    header: '#111827',
  },
} as const;

export type ThemeMode = 'light' | 'dark';

export const THEME_STORAGE_KEY = 'horacerta-theme';
