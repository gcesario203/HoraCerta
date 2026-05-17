'use client';

import { AntdRegistry } from '@ant-design/nextjs-registry';
import { App, ConfigProvider } from 'antd';
import ptBR from 'antd/locale/pt_BR';
import { SessaoHydrator } from '@/shared/presentation/sessao-hydrator';

export function Providers({ children }: { children: React.ReactNode }) {
  return (
    <AntdRegistry>
      <ConfigProvider locale={ptBR} theme={{ token: { colorPrimary: '#1677ff' } }}>
        <App>
          <SessaoHydrator />
          {children}
        </App>
      </ConfigProvider>
    </AntdRegistry>
  );
}
