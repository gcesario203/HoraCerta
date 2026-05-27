'use client';

import type { ReactNode } from 'react';
import Link from 'next/link';
import { Button, Result, Skeleton } from 'antd';
import { useEstabelecimento } from '@/cliente/presentation/hooks/use-estabelecimento';

type EstabelecimentoGuardProps = {
  proprietarioId: string;
  children: ReactNode;
};

export function EstabelecimentoGuard({ proprietarioId, children }: EstabelecimentoGuardProps) {
  const { loading, invalido } = useEstabelecimento(proprietarioId);

  if (loading) {
    return <Skeleton active paragraph={{ rows: 4 }} />;
  }

  if (invalido) {
    return (
      <div className="hc-estabelecimento-invalido">
        <Result
          status="404"
          title="Estabelecimento não encontrado"
          subTitle="O link pode estar incorreto ou o estabelecimento não está mais ativo."
          extra={
            <Link href="/">
              <Button type="primary">Voltar ao catálogo</Button>
            </Link>
          }
        />
      </div>
    );
  }

  return <>{children}</>;
}
