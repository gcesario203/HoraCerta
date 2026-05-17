'use client';

import Link from 'next/link';
import { useParams } from 'next/navigation';
import { Button, Card, Space, Typography } from 'antd';

export default function EstabelecimentoHomePage() {
  const params = useParams<{ proprietarioId: string }>();
  const proprietarioId = params.proprietarioId;

  return (
    <main style={{ minHeight: '100vh', padding: 24, maxWidth: 480, margin: '0 auto' }}>
      <Card>
        <Typography.Title level={3}>Agendar horário</Typography.Title>
        <Typography.Paragraph>
          Escolha um procedimento e um horário disponível. Após enviar, aguarde a confirmação do
          estabelecimento.
        </Typography.Paragraph>
        <Space direction="vertical" style={{ width: '100%' }}>
          <Link href={`/e/${proprietarioId}/agendar`}>
            <Button type="primary" block size="large">
              Agendar agora
            </Button>
          </Link>
          <Link href={`/e/${proprietarioId}/meus-agendamentos`}>
            <Button block>Meus agendamentos</Button>
          </Link>
        </Space>
        <Typography.Paragraph type="secondary" style={{ marginTop: 24, marginBottom: 0 }}>
          Você receberá um lembrete antes do horário (enviado pelo estabelecimento).
        </Typography.Paragraph>
      </Card>
    </main>
  );
}
