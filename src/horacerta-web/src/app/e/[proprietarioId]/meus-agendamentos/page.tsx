'use client';

import { useCallback, useEffect, useState } from 'react';
import Link from 'next/link';
import { useParams, useRouter } from 'next/navigation';
import { App, Button, Card, List, Tag } from 'antd';
import { listarAgendamentosClienteUseCase } from '@/agendamento/application';
import type { AgendamentoClienteListagemDto } from '@/agendamento/application/dtos/agendamento.dto';
import { obterSessaoCliente } from '@/cliente/application/sessao-cliente';
import { useClienteSessaoStore } from '@/cliente/presentation/stores/cliente-sessao.store';
import { extractApiMessage } from '@/shared/infrastructure/http/api-error';
import { ClienteShell } from '@/shared/presentation/layouts/cliente-shell';
import { formatarDataHora, labelEstado } from '@/shared/presentation/format';

export default function MeusAgendamentosPage() {
  const params = useParams<{ proprietarioId: string }>();
  const proprietarioId = params.proprietarioId;
  const router = useRouter();
  const { message } = App.useApp();
  const clienteId = useClienteSessaoStore((s) => s.clienteId);
  const setSessao = useClienteSessaoStore((s) => s.setSessao);
  const [lista, setLista] = useState<AgendamentoClienteListagemDto[]>([]);
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
    <ClienteShell
      proprietarioId={proprietarioId}
      title="Meus agendamentos"
      subtitle="Cancelamento e remarcação devem ser feitos pelo estabelecimento."
      backHref={`/e/${proprietarioId}`}
    >
      <List
        loading={loading}
        dataSource={lista}
        locale={{ emptyText: 'Nenhum agendamento' }}
        renderItem={(item) => (
          <Card className="hc-card-elevated" style={{ marginBottom: 12 }} size="small">
            <strong>{item.procedimentoNome}</strong>
            <br />
            {item.slotInicio && (
              <span style={{ color: 'var(--hc-text-muted)', fontSize: '0.9rem' }}>
                {formatarDataHora(item.slotInicio)}
              </span>
            )}
            <br />
            <Tag color="processing" style={{ marginTop: 8 }}>
              {labelEstado(item.estado)}
            </Tag>
            {['CONFIRMADO', 'FINALIZADO', 'REALIZADO'].includes(item.estado) && (
              <div style={{ marginTop: 12 }}>
                <Link href={`/e/${proprietarioId}/avaliar/${item.agendamentoId}`}>
                  <Button type="primary" size="small" ghost>
                    Avaliar atendimento
                  </Button>
                </Link>
              </div>
            )}
          </Card>
        )}
      />
    </ClienteShell>
  );
}
