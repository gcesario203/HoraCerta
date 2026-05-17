'use client';

import Link from 'next/link';
import { useParams } from 'next/navigation';
import { Button, Card, Skeleton, Space, Typography } from 'antd';
import {
  CalendarOutlined,
  ClockCircleOutlined,
  UnorderedListOutlined,
} from '@ant-design/icons';
import { useEstabelecimento } from '@/cliente/presentation/hooks/use-estabelecimento';
import { ClienteShell } from '@/shared/presentation/layouts/cliente-shell';

export default function EstabelecimentoHomePage() {
  const params = useParams<{ proprietarioId: string }>();
  const proprietarioId = params.proprietarioId;
  const { nome, loading } = useEstabelecimento(proprietarioId);

  return (
    <ClienteShell proprietarioId={proprietarioId}>
      <Card className="hc-card-elevated hc-cliente-hero" variant="borderless">
        {loading ? (
          <Skeleton active paragraph={{ rows: 2 }} />
        ) : (
          <>
            <Typography.Title level={3} style={{ marginTop: 0 }}>
              {nome}
            </Typography.Title>
            <Typography.Paragraph type="secondary" style={{ marginBottom: 0 }}>
              Agende online em poucos passos. Após enviar, aguarde a confirmação do
              estabelecimento.
            </Typography.Paragraph>
          </>
        )}
      </Card>

      <Space direction="vertical" size="middle" style={{ width: '100%', marginTop: 16 }}>
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

      <Card className="hc-card-elevated" variant="borderless" style={{ marginTop: 16 }}>
        <Space align="start">
          <ClockCircleOutlined style={{ fontSize: 20, color: 'var(--hc-primary)' }} />
          <Typography.Paragraph type="secondary" style={{ marginBottom: 0 }}>
            Você receberá um lembrete antes do horário (enviado pelo estabelecimento).
            Cancelamentos e remarcações são feitos diretamente com o estabelecimento.
          </Typography.Paragraph>
        </Space>
      </Card>
    </ClienteShell>
  );
}
