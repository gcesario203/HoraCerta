'use client';

import Link from 'next/link';
import { Button, Card, Space, Tag, Typography } from 'antd';
import {
  CalendarOutlined,
  ClockCircleOutlined,
  ScissorOutlined,
} from '@ant-design/icons';
import type { EstabelecimentoCatalogoDto } from '../application/dtos/catalogo.dto';
import {
  formatarDataHora,
  formatarDuracao,
  formatarFaixaPreco,
  formatarMoeda,
} from './formatters';

type Props = {
  estabelecimento: EstabelecimentoCatalogoDto;
};

export function EstabelecimentoCard({ estabelecimento }: Props) {
  const faixaPreco = formatarFaixaPreco(
    estabelecimento.precoMinimo,
    estabelecimento.precoMaximo,
  );
  const proximoHorario = estabelecimento.proximoHorarioInicio
    ? formatarDataHora(estabelecimento.proximoHorarioInicio)
    : null;

  return (
    <Card className="hc-estabelecimento-card hc-card-elevated" variant="borderless">
      <div className="hc-estabelecimento-card__header">
        <Typography.Title level={4} className="hc-estabelecimento-card__nome">
          {estabelecimento.nome}
        </Typography.Title>
        <Space size={[4, 4]} wrap>
          <Tag color="green" icon={<ScissorOutlined />}>
            {estabelecimento.quantidadeProcedimentos} serviço
            {estabelecimento.quantidadeProcedimentos !== 1 ? 's' : ''}
          </Tag>
          <Tag color="blue" icon={<ClockCircleOutlined />}>
            {estabelecimento.quantidadeHorariosDisponiveis} horário
            {estabelecimento.quantidadeHorariosDisponiveis !== 1 ? 's' : ''}
          </Tag>
          {faixaPreco ? <Tag>{faixaPreco}</Tag> : null}
        </Space>
      </div>

      {proximoHorario ? (
        <Typography.Paragraph type="secondary" className="hc-estabelecimento-card__proximo">
          <CalendarOutlined /> Próximo horário: <strong>{proximoHorario}</strong>
        </Typography.Paragraph>
      ) : null}

      <ul className="hc-estabelecimento-card__servicos">
        {estabelecimento.procedimentos.map((p) => (
          <li key={p.id}>
            <span>{p.nome}</span>
            <span className="hc-estabelecimento-card__servico-meta">
              {formatarMoeda(p.valor)} · {formatarDuracao(p.tempoEstimadoMinutos)}
            </span>
          </li>
        ))}
        {estabelecimento.quantidadeProcedimentos > estabelecimento.procedimentos.length ? (
          <li className="hc-estabelecimento-card__mais">
            +{estabelecimento.quantidadeProcedimentos - estabelecimento.procedimentos.length}{' '}
            outro
            {estabelecimento.quantidadeProcedimentos - estabelecimento.procedimentos.length !== 1
              ? 's'
              : ''}{' '}
            serviço
          </li>
        ) : null}
      </ul>

      <Space direction="vertical" size="small" style={{ width: '100%', marginTop: 16 }}>
        <Link href={`/e/${estabelecimento.id}/agendar`}>
          <Button type="primary" block size="large" icon={<CalendarOutlined />}>
            Agendar agora
          </Button>
        </Link>
        <Link href={`/e/${estabelecimento.id}`}>
          <Button block type="link" size="small">
            Ver página do estabelecimento
          </Button>
        </Link>
      </Space>
    </Card>
  );
}
