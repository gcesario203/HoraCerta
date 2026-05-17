'use client';

import type { ReactNode } from 'react';
import Link from 'next/link';
import { Button, Skeleton } from 'antd';
import { ArrowLeftOutlined } from '@ant-design/icons';
import { useEstabelecimento } from '@/cliente/presentation/hooks/use-estabelecimento';
import { AppBrand } from '../components/app-brand';
import { ClienteNav } from '../components/cliente-nav';
import { ThemeToggle } from '../components/theme-toggle';

type ClienteShellProps = {
  children: ReactNode;
  proprietarioId?: string;
  title?: string;
  subtitle?: string;
  backHref?: string;
  wide?: boolean;
};

export function ClienteShell({
  children,
  proprietarioId,
  title,
  subtitle,
  backHref,
  wide = false,
}: ClienteShellProps) {
  const home = proprietarioId ? `/e/${proprietarioId}` : '/';
  const { nome, loading } = useEstabelecimento(proprietarioId ?? '');

  return (
    <div className="hc-cliente-shell">
      <header className="hc-cliente-shell__header">
        <div className="hc-cliente-shell__header-inner">
          <div className="hc-cliente-shell__brand">
            <AppBrand href={home} size="sm" />
            {proprietarioId ? (
              <span className="hc-cliente-shell__estabelecimento">
                {loading ? <Skeleton.Input active size="small" style={{ width: 120 }} /> : nome}
              </span>
            ) : null}
          </div>
          <ThemeToggle />
        </div>
        {proprietarioId ? <ClienteNav proprietarioId={proprietarioId} /> : null}
      </header>
      <main
        className={`hc-cliente-shell__main${wide ? ' hc-cliente-shell__main--wide' : ''}`}
      >
        {(title || backHref) && (
          <div className="hc-page-intro">
            {backHref && (
              <Link href={backHref}>
                <Button type="text" icon={<ArrowLeftOutlined />} className="hc-back-btn">
                  Voltar
                </Button>
              </Link>
            )}
            {title && <h1 className="hc-page-title">{title}</h1>}
            {subtitle && <p className="hc-page-subtitle">{subtitle}</p>}
          </div>
        )}
        {children}
      </main>
    </div>
  );
}
