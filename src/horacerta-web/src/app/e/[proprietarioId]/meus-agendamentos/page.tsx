'use client';

import { useCallback, useEffect, useState } from 'react';
import Link from 'next/link';
import { useParams, useRouter } from 'next/navigation';
import { App, Button, Card, List, Tag, Typography } from 'antd';
import { listarAgendamentosClienteUseCase } from '@/agendamento/application';
import type { AgendamentoDto } from '@/agendamento/application/dtos/agendamento.dto';
import { obterSessaoCliente } from '@/cliente/application/sessao-cliente';
import { useClienteSessaoStore } from '@/cliente/presentation/stores/cliente-sessao.store';
import { extractApiMessage } from '@/shared/infrastructure/http/api-error';
import { labelEstado } from '@/shared/presentation/format';

export default function MeusAgendamentosPage() {
  const params = useParams<{ proprietarioId: string }>();
  const proprietarioId = params.proprietarioId;
  const router = useRouter();
  const { message } = App.useApp();
  const clienteId = useClienteSessaoStore((s) => s.clienteId);
  const setSessao = useClienteSessaoStore((s) => s.setSessao);
  const [lista, setLista] = useState<AgendamentoDto[]>([]);
  const [loading, setLoading] = useState(true);

  const carregar = useCallback(async () => {
    let cid = clienteId;
    if (!cid) {
      const sessao = await obterSessaoCliente();
      if (!sessao || sessao.proprietarioId !== proprietarioId) {
        router.replace(`/e/${proprietarioId}/agendar`);
        return;
      }
      setSessao(sessao.clienteId, sessao.proprietarioId);
      cid = sessao.clienteId;
    }

    setLoading(true);
    try {
      const data = await listarAgendamentosClienteUseCase.execute(cid);
      setLista(data);
    } catch (error) {
      message.error(extractApiMessage(error));
    } finally {
      setLoading(false);
    }
  }, [clienteId, proprietarioId, router, setSessao, message]);

  useEffect(() => {
    carregar();
  }, [carregar]);

  return (
    <main style={{ padding: 24, maxWidth: 520, margin: '0 auto' }}>
      <Typography.Title level={3}>Meus agendamentos</Typography.Title>
      <Typography.Paragraph type="secondary">
        Cancelamento e remarcação devem ser feitos pelo estabelecimento.
      </Typography.Paragraph>
      <List
        loading={loading}
        dataSource={lista}
        locale={{ emptyText: 'Nenhum agendamento' }}
        renderItem={(item) => (
          <Card style={{ marginBottom: 12 }} size="small">
            <Typography.Text>
              Procedimento: {item.procedimentoId.slice(0, 8)}…
            </Typography.Text>
            <br />
            <Tag>{labelEstado(item.estado)}</Tag>
            {['CONFIRMADO', 'FINALIZADO', 'REALIZADO'].includes(item.estado) && (
              <div style={{ marginTop: 8 }}>
                <Link href={`/e/${proprietarioId}/avaliar/${item.id}`}>
                  <Button size="small">Avaliar atendimento</Button>
                </Link>
              </div>
            )}
          </Card>
        )}
      />
      <Link href={`/e/${proprietarioId}`}>
        <Button>Voltar</Button>
      </Link>
    </main>
  );
}
