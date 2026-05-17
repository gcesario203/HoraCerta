'use client';

import { useCallback, useEffect, useMemo, useState } from 'react';
import Link from 'next/link';
import { useParams, useRouter } from 'next/navigation';
import { App, Button, Card, Empty, List, Space, Tag, Typography } from 'antd';
import {
  CalendarOutlined,
  ClockCircleOutlined,
  StarOutlined,
} from '@ant-design/icons';
import { listarAgendamentosClienteUseCase } from '@/agendamento/application';
import type { AgendamentoClienteListagemDto } from '@/agendamento/application/dtos/agendamento.dto';
import { obterSessaoCliente } from '@/cliente/application/sessao-cliente';
import { useClienteSessaoStore } from '@/cliente/presentation/stores/cliente-sessao.store';
import { extractApiMessage } from '@/shared/infrastructure/http/api-error';
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
  const podeAvaliar = ['CONFIRMADO', 'FINALIZADO', 'REALIZADO'].includes(item.estado);

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
        <Link href={`/e/${proprietarioId}/avaliar/${item.agendamentoId}`}>
          <Button type="primary" size="small" ghost icon={<StarOutlined />}>
            Avaliar atendimento
          </Button>
        </Link>
      ) : null}
    </Card>
  );
}

export default function MeusAgendamentosPage() {
  const params = useParams<{ proprietarioId: string }>();
  const proprietarioId = params.proprietarioId;
  const router = useRouter();
  const { message } = App.useApp();
  const clienteId = useClienteSessaoStore((s) => s.clienteId);
  const setSessao = useClienteSessaoStore((s) => s.setSessao);
  const [lista, setLista] = useState<AgendamentoClienteListagemDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [semSessao, setSemSessao] = useState(false);

  const carregar = useCallback(async () => {
    let cid = clienteId;
    if (!cid) {
      const sessao = await obterSessaoCliente();
      if (!sessao || sessao.proprietarioId !== proprietarioId) {
        setSemSessao(true);
        setLoading(false);
        return;
      }
      setSessao(sessao.clienteId, sessao.proprietarioId);
      cid = sessao.clienteId;
    }

    setSemSessao(false);
    setLoading(true);
    try {
      const data = await listarAgendamentosClienteUseCase.execute(cid);
      setLista(data);
    } catch (error) {
      message.error(extractApiMessage(error));
    } finally {
      setLoading(false);
    }
  }, [clienteId, proprietarioId, setSessao, message]);

  useEffect(() => {
    void carregar();
  }, [carregar]);

  const { pendentes, outros } = useMemo(() => {
    const pend = lista.filter((a) => a.estado === 'PENDENTE');
    const rest = lista.filter((a) => a.estado !== 'PENDENTE');
    return { pendentes: pend, outros: rest };
  }, [lista]);

  if (semSessao) {
    return (
      <ClienteShell proprietarioId={proprietarioId} title="Meus agendamentos">
        <Card className="hc-card-elevated" variant="borderless">
          <Empty description="Identifique-se para ver seus agendamentos">
            <Link href={`/e/${proprietarioId}/agendar`}>
              <Button type="primary" icon={<CalendarOutlined />}>
                Agendar e identificar
              </Button>
            </Link>
          </Empty>
        </Card>
      </ClienteShell>
    );
  }

  return (
    <ClienteShell
      proprietarioId={proprietarioId}
      title="Meus agendamentos"
      subtitle="Cancelamento e remarcação devem ser feitos pelo estabelecimento."
    >
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
              <Empty description="Nenhum agendamento ainda">
                <Link href={`/e/${proprietarioId}/agendar`}>
                  <Button type="primary">Fazer um agendamento</Button>
                </Link>
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
    </ClienteShell>
  );
}
