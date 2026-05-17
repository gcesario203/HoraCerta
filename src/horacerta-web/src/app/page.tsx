'use client';

import { useCallback, useEffect, useMemo, useState } from 'react';
import Link from 'next/link';
import { Alert, Button, Empty, Input, Skeleton, Space, Typography } from 'antd';
import { SearchOutlined, ShopOutlined } from '@ant-design/icons';
import { listarEstabelecimentosCatalogo } from '@/catalogo/application/listar-estabelecimentos-catalogo';
import type { EstabelecimentoCatalogoDto } from '@/catalogo/application/dtos/catalogo.dto';
import { EstabelecimentoCard } from '@/catalogo/presentation/estabelecimento-card';
import { AppBrand } from '@/shared/presentation/components/app-brand';
import { ThemeToggle } from '@/shared/presentation/components/theme-toggle';

export default function HomePage() {
  const [estabelecimentos, setEstabelecimentos] = useState<EstabelecimentoCatalogoDto[]>([]);
  const [busca, setBusca] = useState('');
  const [buscaAplicada, setBuscaAplicada] = useState<string | undefined>();
  const [carregando, setCarregando] = useState(true);
  const [erro, setErro] = useState<string | null>(null);

  const carregar = useCallback(async (termo?: string) => {
    setCarregando(true);
    setErro(null);
    try {
      const dados = await listarEstabelecimentosCatalogo(termo);
      setEstabelecimentos(dados);
      setBuscaAplicada(termo);
    } catch {
      setErro('Não foi possível carregar os estabelecimentos. Tente novamente em instantes.');
    } finally {
      setCarregando(false);
    }
  }, []);

  useEffect(() => {
    void carregar();
  }, [carregar]);

  const resumo = useMemo(() => {
    if (carregando) return null;
    const n = estabelecimentos.length;
    if (n === 0) return 'Nenhum estabelecimento com horários disponíveis no momento.';
    return `${n} estabelecimento${n !== 1 ? 's' : ''} com horários abertos para agendamento`;
  }, [carregando, estabelecimentos.length]);

  function aplicarBusca() {
    const termo = busca.trim() || undefined;
    void carregar(termo);
  }

  return (
    <div className="hc-landing">
      <header className="hc-landing__header hc-landing__header--wide">
        <AppBrand href="/" />
        <Space>
          <Link href="/login">
            <Button type="text" icon={<ShopOutlined />}>
              Área do proprietário
            </Button>
          </Link>
          <ThemeToggle />
        </Space>
      </header>

      <section className="hc-landing__intro">
        <span className="hc-landing__badge">Agende em poucos cliques</span>
        <h1 className="hc-landing__title">Encontre horários disponíveis perto de você</h1>
        <p className="hc-landing__lead">
          Estabelecimentos com serviços ativos e horários livres para reserva online — sem cadastro
          prévio para agendar.
        </p>
        <Space.Compact className="hc-catalogo-busca" size="large">
          <Input
            allowClear
            size="large"
            placeholder="Buscar por nome do estabelecimento…"
            prefix={<SearchOutlined />}
            value={busca}
            onChange={(e) => setBusca(e.target.value)}
            onPressEnter={aplicarBusca}
          />
          <Button type="primary" size="large" onClick={aplicarBusca}>
            Buscar
          </Button>
        </Space.Compact>
        {resumo ? (
          <Typography.Text type="secondary" className="hc-catalogo-resumo">
            {resumo}
            {buscaAplicada ? ` · filtro: “${buscaAplicada}”` : ''}
          </Typography.Text>
        ) : null}
      </section>

      <section className="hc-catalogo">
        {erro ? (
          <Alert
            type="error"
            showIcon
            message={erro}
            action={
              <Button size="small" onClick={() => void carregar(buscaAplicada)}>
                Tentar novamente
              </Button>
            }
          />
        ) : null}

        {carregando ? (
          <div className="hc-catalogo-grid">
            {[1, 2, 3].map((k) => (
              <Skeleton key={k} active paragraph={{ rows: 6 }} className="hc-catalogo-skeleton" />
            ))}
          </div>
        ) : null}

        {!carregando && !erro && estabelecimentos.length === 0 ? (
          <Empty
            className="hc-catalogo-empty"
            description={
              buscaAplicada
                ? 'Nenhum estabelecimento encontrado com esse nome.'
                : 'Ainda não há estabelecimentos com procedimentos e horários disponíveis.'
            }
          >
            <Typography.Paragraph type="secondary">
              É proprietário?{' '}
              <Link href="/registrar">Crie sua conta</Link>, cadastre serviços e libere horários na
              agenda.
            </Typography.Paragraph>
          </Empty>
        ) : null}

        {!carregando && !erro && estabelecimentos.length > 0 ? (
          <div className="hc-catalogo-grid">
            {estabelecimentos.map((e) => (
              <EstabelecimentoCard key={e.id} estabelecimento={e} />
            ))}
          </div>
        ) : null}
      </section>

      <footer className="hc-landing__footer">
        <Link href="/registrar">
          <Button size="large">Sou proprietário — criar conta</Button>
        </Link>
      </footer>
    </div>
  );
}
