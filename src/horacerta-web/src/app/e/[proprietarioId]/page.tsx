'use client';

import { useParams } from 'next/navigation';
import { Button, Card, Skeleton, Space, Typography } from 'antd';
import {
  CalendarOutlined,
  ClockCircleOutlined,
  UnorderedListOutlined,
  WhatsAppOutlined,
} from '@ant-design/icons';
import { useEstabelecimento } from '@/cliente/presentation/hooks/use-estabelecimento';
import { EstabelecimentoGuard } from '@/shared/presentation/components/estabelecimento-guard';
import { ClienteShell } from '@/shared/presentation/layouts/cliente-shell';

function buildWhatsAppUrl(proprietarioId: string): string | null {
  const numero = process.env.NEXT_PUBLIC_WHATSAPP_NUMERO?.replace(/\D/g, '');
  if (!numero) return null;
  const texto = encodeURIComponent(`HC-${proprietarioId}`);
  return `https://wa.me/${numero}?text=${texto}`;
}

function EstabelecimentoHomeContent({ proprietarioId }: { proprietarioId: string }) {
  const { nome, loading } = useEstabelecimento(proprietarioId);
  const whatsappUrl = buildWhatsAppUrl(proprietarioId);

  return (
    <>
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
        <Button
          type="primary"
          block
          size="large"
          icon={<CalendarOutlined />}
          href={`/e/${proprietarioId}/agendar`}
        >
          Agendar agora
        </Button>
        <Button
          block
          size="large"
          icon={<UnorderedListOutlined />}
          href={`/e/${proprietarioId}/meus-agendamentos`}
        >
          Meus agendamentos
        </Button>
        {whatsappUrl ? (
          <Button
            block
            size="large"
            icon={<WhatsAppOutlined />}
            href={whatsappUrl}
            target="_blank"
            rel="noopener noreferrer"
            style={{ borderColor: '#25D366', color: '#128C7E' }}
          >
            Agendar no WhatsApp
          </Button>
        ) : null}
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
    </>
  );
}

export default function EstabelecimentoHomePage() {
  const params = useParams<{ proprietarioId: string }>();
  const proprietarioId = params.proprietarioId;

  return (
    <ClienteShell proprietarioId={proprietarioId}>
      <EstabelecimentoGuard proprietarioId={proprietarioId}>
        <EstabelecimentoHomeContent proprietarioId={proprietarioId} />
      </EstabelecimentoGuard>
    </ClienteShell>
  );
}
