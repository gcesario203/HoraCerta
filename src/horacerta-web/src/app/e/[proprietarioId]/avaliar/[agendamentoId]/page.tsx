'use client';

import { useState } from 'react';
import { useParams, useRouter } from 'next/navigation';
import { App, Button, Card, Empty, Form, Input, Rate, Skeleton } from 'antd';
import { avaliarAgendamentoUseCase } from '@/avaliacao/application';
import { useClienteEstabelecimento } from '@/cliente/presentation/hooks/use-cliente-estabelecimento';
import { extractApiMessage } from '@/shared/infrastructure/http/api-error';
import { EstabelecimentoGuard } from '@/shared/presentation/components/estabelecimento-guard';
import { ClienteShell } from '@/shared/presentation/layouts/cliente-shell';

function AvaliarForm() {
  const params = useParams<{ proprietarioId: string; agendamentoId: string }>();
  const router = useRouter();
  const { message } = App.useApp();
  const { ready, clienteId, semSessao } = useClienteEstabelecimento(params.proprietarioId);
  const [loading, setLoading] = useState(false);

  const onFinish = async (values: { nota: number; comentario?: string }) => {
    if (!clienteId) return;
    setLoading(true);
    try {
      await avaliarAgendamentoUseCase.execute(clienteId, params.agendamentoId, {
        proprietarioId: params.proprietarioId,
        nota: values.nota,
        comentario: values.comentario ?? null,
      });
      message.success('Avaliação enviada. Obrigado!');
      router.push(`/e/${params.proprietarioId}/meus-agendamentos`);
    } catch (error) {
      message.error(extractApiMessage(error));
    } finally {
      setLoading(false);
    }
  };

  if (!ready) {
    return <Skeleton active paragraph={{ rows: 4 }} />;
  }

  if (semSessao) {
    return (
      <Card className="hc-card-elevated" variant="borderless">
        <Empty description="Identifique-se antes de avaliar">
          <Button type="primary" href={`/e/${params.proprietarioId}/agendar`}>
            Agendar e identificar
          </Button>
        </Empty>
      </Card>
    );
  }

  return (
    <Card className="hc-card-elevated" variant="borderless">
      <Form layout="vertical" onFinish={onFinish} initialValues={{ nota: undefined }}>
        <Form.Item
          label="Nota"
          name="nota"
          rules={[{ required: true, message: 'Selecione uma nota' }]}
        >
          <Rate />
        </Form.Item>
        <Form.Item label="Comentário (opcional)" name="comentario">
          <Input.TextArea rows={3} />
        </Form.Item>
        <Button type="primary" htmlType="submit" block size="large" loading={loading}>
          Enviar avaliação
        </Button>
      </Form>
    </Card>
  );
}

export default function AvaliarPage() {
  const params = useParams<{ proprietarioId: string; agendamentoId: string }>();

  return (
    <ClienteShell
      proprietarioId={params.proprietarioId}
      title="Avaliar atendimento"
      subtitle="Sua opinião ajuda o estabelecimento a melhorar."
      backHref={`/e/${params.proprietarioId}/meus-agendamentos`}
    >
      <EstabelecimentoGuard proprietarioId={params.proprietarioId}>
        <AvaliarForm />
      </EstabelecimentoGuard>
    </ClienteShell>
  );
}
