'use client';

import Link from 'next/link';
import { Button, Card, Space, Typography } from 'antd';
import { CalendarOutlined, ShopOutlined } from '@ant-design/icons';
import { AppBrand } from '@/shared/presentation/components/app-brand';
import { ThemeToggle } from '@/shared/presentation/components/theme-toggle';

export default function HomePage() {
  return (
    <div className="hc-landing">
      <header className="hc-landing__header">
        <AppBrand href="/" />
        <ThemeToggle />
      </header>
      <section className="hc-landing__hero">
        <div>
          <span className="hc-landing__badge">Agendamentos simplificados</span>
          <h1 className="hc-landing__title">
            O horário certo para você e seus clientes
          </h1>
          <p className="hc-landing__lead">
            Gerencie procedimentos, disponibilize horários e confirme agendamentos em um portal
            profissional, rápido e pensado para mobile.
          </p>
          <Space direction="vertical" size="middle" style={{ width: '100%', maxWidth: 320 }}>
            <Link href="/login">
              <Button type="primary" size="large" block icon={<ShopOutlined />}>
                Área do proprietário
              </Button>
            </Link>
            <Link href="/registrar">
              <Button size="large" block>
                Criar conta gratuita
              </Button>
            </Link>
          </Space>
        </div>
        <Card className="hc-card-elevated" variant="borderless">
          <Space direction="vertical" size="large" style={{ width: '100%' }}>
            <div>
              <CalendarOutlined style={{ fontSize: 28, color: 'var(--hc-primary)' }} />
              <Typography.Title level={4} style={{ marginTop: 12, marginBottom: 4 }}>
                Para clientes
              </Typography.Title>
              <Typography.Paragraph type="secondary" style={{ marginBottom: 0 }}>
                Acesse o link do estabelecimento (ex.: <code>/e/seu-id</code>), escolha o serviço e
                reserve um horário em poucos toques.
              </Typography.Paragraph>
            </div>
          </Space>
        </Card>
      </section>
    </div>
  );
}
