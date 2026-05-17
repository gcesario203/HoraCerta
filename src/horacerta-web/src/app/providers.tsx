'use client';

import { AntdRegistry } from '@ant-design/nextjs-registry';
import { App } from 'antd';
import { SessaoHydrator } from '@/shared/presentation/sessao-hydrator';
import { ThemeProvider } from '@/shared/presentation/theme/theme-provider';

export function Providers({ children }: { children: React.ReactNode }) {
  return (
    <AntdRegistry>
      <ThemeProvider>
        <App>
          <SessaoHydrator />
          {children}
        </App>
      </ThemeProvider>
    </AntdRegistry>
  );
}
