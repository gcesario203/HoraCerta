'use client';

import { useEffect } from 'react';
import { ConfigProvider } from 'antd';
import ptBR from 'antd/locale/pt_BR';
import { useThemeStore } from '../stores/theme.store';
import { buildAntdTheme } from './antd-config';

export function ThemeProvider({ children }: { children: React.ReactNode }) {
  const mode = useThemeStore((s) => s.mode);
  const hydrate = useThemeStore((s) => s.hydrate);

  useEffect(() => {
    hydrate();
  }, [hydrate]);

  return (
    <ConfigProvider locale={ptBR} theme={buildAntdTheme(mode)}>
      {children}
    </ConfigProvider>
  );
}
