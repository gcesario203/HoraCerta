'use client';

import { Descriptions, Divider, Typography } from 'antd';
import { formatarDataHora, formatarMoeda } from '@/shared/presentation/format';

type BookingSummaryProps = {
  procedimentoNome: string;
  procedimentoValor: number;
  procedimentoMinutos: number;
  slotInicio: string;
  clienteNome?: string;
};

export function BookingSummary({
  procedimentoNome,
  procedimentoValor,
  procedimentoMinutos,
  slotInicio,
  clienteNome,
}: BookingSummaryProps) {
  return (
    <section className="hc-booking-summary">
      <Typography.Title level={5} style={{ marginTop: 0 }}>
        Resumo do agendamento
      </Typography.Title>
      <Descriptions column={1} size="small" colon={false}>
        {clienteNome ? (
          <Descriptions.Item label="Cliente">{clienteNome}</Descriptions.Item>
        ) : null}
        <Descriptions.Item label="Serviço">{procedimentoNome}</Descriptions.Item>
        <Descriptions.Item label="Valor">{formatarMoeda(procedimentoValor)}</Descriptions.Item>
        <Descriptions.Item label="Duração">{procedimentoMinutos} min</Descriptions.Item>
        <Descriptions.Item label="Horário">{formatarDataHora(slotInicio)}</Descriptions.Item>
      </Descriptions>
      <Divider style={{ margin: '12px 0' }} />
      <Typography.Paragraph type="secondary" style={{ marginBottom: 0, fontSize: '0.9rem' }}>
        Ao confirmar, seu pedido ficará <strong>pendente</strong> até o estabelecimento aprovar.
        Você pode acompanhar em Meus horários.
      </Typography.Paragraph>
    </section>
  );
}
