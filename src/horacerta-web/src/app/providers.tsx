'use client';

import { useEffect } from 'react';
import { AntdRegistry } from '@ant-design/nextjs-registry';
import { App } from 'antd';
import { SessaoHydrator } from '@/shared/presentation/sessao-hydrator';
import { registerBffAuthInterceptor } from '@/shared/infrastructure/http/bff-auth-interceptor';
import { ThemeProvider } from '@/shared/presentation/theme/theme-provider';

function BffAuthSetup() {
  useEffect(() => {
    registerBffAuthInterceptor();
  }, []);
  return null;
}

export function Providers({ children }: { children: React.ReactNode }) {
  return (
    <AntdRegistry>
      <ThemeProvider>
        <App>
          <BffAuthSetup />
          <SessaoHydrator />
          {children}
        </App>
      </ThemeProvider>
    </AntdRegistry>
  );
}
