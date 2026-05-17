'use client';

import Link from 'next/link';
import { useParams } from 'next/navigation';
import { Button, Card, Space, Typography } from 'antd';
import { CalendarOutlined, UnorderedListOutlined } from '@ant-design/icons';
import { ClienteShell } from '@/shared/presentation/layouts/cliente-shell';

export default function EstabelecimentoHomePage() {
  const params = useParams<{ proprietarioId: string }>();
  const proprietarioId = params.proprietarioId;

  return (
    <ClienteShell
      proprietarioId={proprietarioId}
      title="Agendar horário"
      subtitle="Escolha um procedimento e um horário disponível. Após enviar, aguarde a confirmação do estabelecimento."
    >
      <Card className="hc-card-elevated" variant="borderless">
        <Space direction="vertical" size="middle" style={{ width: '100%' }}>
          <Link href={`/e/${proprietarioId}/agendar`}>
            <Button type="primary" block size="large" icon={<CalendarOutlined />}>
              Agendar agora
            </Button>
          </Link>
          <Link href={`/e/${proprietarioId}/meus-agendamentos`}>
            <Button block size="large" icon={<UnorderedListOutlined />}>
              Meus agendamentos
            </Button>
          </Link>
        </Space>
        <Typography.Paragraph type="secondary" style={{ marginTop: 24, marginBottom: 0 }}>
          Você receberá um lembrete antes do horário (enviado pelo estabelecimento).
        </Typography.Paragraph>
      </Card>
    </ClienteShell>
  );
}
