'use client';

import { useState } from 'react';
import Link from 'next/link';
import { useParams, useRouter } from 'next/navigation';
import { App, Button, Card, Form, Input, Rate, Typography } from 'antd';
import { avaliarAgendamentoUseCase } from '@/avaliacao/application';
import { obterSessaoCliente } from '@/cliente/application/sessao-cliente';
import { useClienteSessaoStore } from '@/cliente/presentation/stores/cliente-sessao.store';
import { extractApiMessage } from '@/shared/infrastructure/http/api-error';

export default function AvaliarPage() {
  const params = useParams<{ proprietarioId: string; agendamentoId: string }>();
  const router = useRouter();
  const { message } = App.useApp();
  const clienteId = useClienteSessaoStore((s) => s.clienteId);
  const setSessao = useClienteSessaoStore((s) => s.setSessao);
  const [loading, setLoading] = useState(false);

  const onFinish = async (values: { nota: number; comentario?: string }) => {
    let cid = clienteId;
    if (!cid) {
      const sessao = await obterSessaoCliente();
      if (!sessao) {
        message.warning('Faça um agendamento antes de avaliar');
        router.push(`/e/${params.proprietarioId}/agendar`);
        return;
      }
      setSessao(sessao.clienteId, sessao.proprietarioId);
      cid = sessao.clienteId;
    }

    setLoading(true);
    try {
      await avaliarAgendamentoUseCase.execute(cid, params.agendamentoId, {
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

  return (
    <main style={{ padding: 24, maxWidth: 480, margin: '0 auto' }}>
      <Typography.Title level={3}>Avaliar atendimento</Typography.Title>
      <Card>
        <Form layout="vertical" onFinish={onFinish} initialValues={{ nota: 5 }}>
          <Form.Item label="Nota" name="nota" rules={[{ required: true }]}>
            <Rate />
          </Form.Item>
          <Form.Item label="Comentário (opcional)" name="comentario">
            <Input.TextArea rows={3} />
          </Form.Item>
          <Button type="primary" htmlType="submit" block loading={loading}>
            Enviar avaliação
          </Button>
        </Form>
      </Card>
      <Typography.Paragraph style={{ marginTop: 16 }}>
        <Link href={`/e/${params.proprietarioId}/meus-agendamentos`}>Voltar</Link>
      </Typography.Paragraph>
    </main>
  );
}
