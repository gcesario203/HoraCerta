'use client';

import { useCallback, useEffect, useMemo, useState } from 'react';
import { useParams, useRouter } from 'next/navigation';
import { App, Button, Card, Empty, List, Space, Tag, Typography } from 'antd';
import {
  CalendarOutlined,
  ClockCircleOutlined,
  StarOutlined,
} from '@ant-design/icons';
import { listarAgendamentosClienteUseCase } from '@/agendamento/application';
import type { AgendamentoClienteListagemDto } from '@/agendamento/application/dtos/agendamento.dto';
import { useClienteEstabelecimento } from '@/cliente/presentation/hooks/use-cliente-estabelecimento';
import { extractApiMessage } from '@/shared/infrastructure/http/api-error';
import { EstabelecimentoGuard } from '@/shared/presentation/components/estabelecimento-guard';
import { ClienteShell } from '@/shared/presentation/layouts/cliente-shell';
import { formatarDataHora, labelEstado } from '@/shared/presentation/format';

function corEstado(estado: string) {
  switch (estado) {
    case 'PENDENTE':
      return 'warning';
    case 'CONFIRMADO':
      return 'success';
    case 'CANCELADO':
      return 'default';
    default:
      return 'processing';
  }
}

function AgendamentoCard({
  item,
  proprietarioId,
}: {
  item: AgendamentoClienteListagemDto;
  proprietarioId: string;
}) {
  const podeAvaliar = ['CONFIRMADO', 'FINALIZADO'].includes(item.estado);

  return (
    <Card className="hc-agendamento-card hc-card-elevated" size="small">
      <div className="hc-agendamento-card__header">
        <strong>{item.procedimentoNome}</strong>
        <Tag color={corEstado(item.estado)}>{labelEstado(item.estado)}</Tag>
      </div>
      {item.slotInicio ? (
        <p className="hc-agendamento-card__when">
          <ClockCircleOutlined /> {formatarDataHora(item.slotInicio)}
        </p>
      ) : null}
      {podeAvaliar ? (
        <Button
          type="primary"
          size="small"
          ghost
          icon={<StarOutlined />}
          href={`/e/${proprietarioId}/avaliar/${item.agendamentoId}`}
        >
          Avaliar atendimento
        </Button>
      ) : null}
    </Card>
  );
}

function MeusAgendamentosContent() {
  const params = useParams<{ proprietarioId: string }>();
  const proprietarioId = params.proprietarioId;
  const router = useRouter();
  const { message } = App.useApp();
  const { ready, clienteId, semSessao } = useClienteEstabelecimento(proprietarioId);
  const [lista, setLista] = useState<AgendamentoClienteListagemDto[]>([]);
  const [loading, setLoading] = useState(true);

  const carregar = useCallback(async () => {
    if (!ready) return;
    if (!clienteId) {
      setLoading(false);
      return;
    }
    setLoading(true);
    try {
      const data = await listarAgendamentosClienteUseCase.execute(clienteId, proprietarioId);
      setLista(data);
    } catch (error) {
      message.error(extractApiMessage(error));
    } finally {
      setLoading(false);
    }
  }, [ready, clienteId, proprietarioId, message]);

  useEffect(() => {
    void carregar();
  }, [carregar]);

  const { pendentes, outros } = useMemo(() => {
    const pend = lista.filter((a) => a.estado === 'PENDENTE');
    const rest = lista.filter((a) => a.estado !== 'PENDENTE');
    return { pendentes: pend, outros: rest };
  }, [lista]);

  if (!ready) {
    return null;
  }

  if (semSessao) {
    return (
      <Card className="hc-card-elevated" variant="borderless">
        <Empty description="Identifique-se para ver seus agendamentos">
          <Button
            type="primary"
            icon={<CalendarOutlined />}
            href={`/e/${proprietarioId}/agendar`}
          >
            Agendar e identificar
          </Button>
        </Empty>
      </Card>
    );
  }

  return (
    <>
      {pendentes.length > 0 ? (
        <section className="hc-agendamentos-section">
          <Typography.Title level={5}>Aguardando confirmação</Typography.Title>
          <List
            loading={loading}
            dataSource={pendentes}
            split={false}
            renderItem={(item) => (
              <List.Item style={{ padding: '0 0 12px', border: 'none' }}>
                <AgendamentoCard item={item} proprietarioId={proprietarioId} />
              </List.Item>
            )}
          />
        </section>
      ) : null}

      <section className="hc-agendamentos-section">
        {pendentes.length > 0 ? (
          <Typography.Title level={5}>Histórico</Typography.Title>
        ) : null}
        <List
          loading={loading}
          dataSource={pendentes.length > 0 ? outros : lista}
          locale={{
            emptyText: (
              <Empty description="Nenhum agendamento neste estabelecimento">
                <Button type="primary" href={`/e/${proprietarioId}/agendar`}>
                  Fazer um agendamento
                </Button>
              </Empty>
            ),
          }}
          split={false}
          renderItem={(item) => (
            <List.Item style={{ padding: '0 0 12px', border: 'none' }}>
              <AgendamentoCard item={item} proprietarioId={proprietarioId} />
            </List.Item>
          )}
        />
      </section>

      <Space style={{ marginTop: 8 }}>
        <Button type="link" onClick={() => router.push(`/e/${proprietarioId}/agendar`)}>
          Novo agendamento
        </Button>
      </Space>
    </>
  );
}

export default function MeusAgendamentosPage() {
  const params = useParams<{ proprietarioId: string }>();
  const proprietarioId = params.proprietarioId;

  return (
    <ClienteShell
      proprietarioId={proprietarioId}
      title="Meus agendamentos"
      subtitle="Cancelamento e remarcação devem ser feitos pelo estabelecimento."
    >
      <EstabelecimentoGuard proprietarioId={proprietarioId}>
        <MeusAgendamentosContent />
      </EstabelecimentoGuard>
    </ClienteShell>
  );
}
