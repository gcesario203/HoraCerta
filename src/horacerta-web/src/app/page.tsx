'use client';

import Link from 'next/link';
import { Button, Card, Space, Typography } from 'antd';

export default function HomePage() {
  return (
    <main
      style={{
        minHeight: '100vh',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        padding: 24,
      }}
    >
      <Card style={{ maxWidth: 480, width: '100%' }}>
        <Typography.Title level={2}>HoraCerta</Typography.Title>
        <Typography.Paragraph>
          Portal de agendamentos para estabelecimentos e clientes.
        </Typography.Paragraph>
        <Space direction="vertical" style={{ width: '100%' }}>
          <Link href="/login">
            <Button type="primary" block>
              Área do proprietário
            </Button>
          </Link>
          <Typography.Text type="secondary">
            Clientes: acesse o link do estabelecimento (ex.: /e/seu-id).
          </Typography.Text>
        </Space>
      </Card>
    </main>
  );
}
