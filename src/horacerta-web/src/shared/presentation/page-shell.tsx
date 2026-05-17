'use client';

import { Layout, Typography } from 'antd';

const { Header, Content } = Layout;

type PageShellProps = {
  title: string;
  extra?: React.ReactNode;
  children: React.ReactNode;
};

export function PageShell({ title, extra, children }: PageShellProps) {
  return (
    <Layout style={{ minHeight: '100vh' }}>
      <Header
        style={{
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'space-between',
          background: '#001529',
        }}
      >
        <Typography.Title level={4} style={{ color: '#fff', margin: 0 }}>
          HoraCerta
        </Typography.Title>
        {extra}
      </Header>
      <Content style={{ padding: 24, maxWidth: 960, margin: '0 auto', width: '100%' }}>
        <Typography.Title level={3} style={{ marginBottom: 24 }}>
          {title}
        </Typography.Title>
        {children}
      </Content>
    </Layout>
  );
}
