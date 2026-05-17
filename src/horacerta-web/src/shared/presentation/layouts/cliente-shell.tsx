'use client';

import type { ReactNode } from 'react';
import Link from 'next/link';
import { Button } from 'antd';
import { ArrowLeftOutlined } from '@ant-design/icons';
import { AppBrand } from '../components/app-brand';
import { ThemeToggle } from '../components/theme-toggle';

type ClienteShellProps = {
  children: ReactNode;
  proprietarioId?: string;
  title?: string;
  subtitle?: string;
  backHref?: string;
};

export function ClienteShell({
  children,
  proprietarioId,
  title,
  subtitle,
  backHref,
}: ClienteShellProps) {
  const home = proprietarioId ? `/e/${proprietarioId}` : '/';

  return (
    <div className="hc-cliente-shell">
      <header className="hc-cliente-shell__header">
        <div className="hc-cliente-shell__header-inner">
          <AppBrand href={home} size="sm" />
          <ThemeToggle />
        </div>
      </header>
      <main className="hc-cliente-shell__main">
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
