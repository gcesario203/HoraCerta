'use client';

import { useState } from 'react';
import Link from 'next/link';
import { usePathname, useRouter } from 'next/navigation';
import { Button, Drawer, Layout, Menu, Space } from 'antd';
import {
  CalendarOutlined,
  ClockCircleOutlined,
  LogoutOutlined,
  MenuOutlined,
  ScheduleOutlined,
  ToolOutlined,
} from '@ant-design/icons';
import { logoutUseCase } from '@/auth/application';
import { useAuthStore } from '@/auth/presentation/stores/auth.store';
import { AppBrand } from '@/shared/presentation/components/app-brand';
import { ThemeToggle } from '@/shared/presentation/components/theme-toggle';

const { Header, Sider, Content } = Layout;

const items = [
  { key: '/proprietario/procedimentos', icon: <ToolOutlined />, label: 'Procedimentos' },
  { key: '/proprietario/agenda', icon: <ClockCircleOutlined />, label: 'Agenda' },
  { key: '/proprietario/agendamentos', icon: <ScheduleOutlined />, label: 'Agendamentos' },
  { key: '/proprietario/atendimentos', icon: <CalendarOutlined />, label: 'Atendimentos' },
];

export default function ProprietarioLayout({ children }: { children: React.ReactNode }) {
  const pathname = usePathname();
  const router = useRouter();
  const clearSession = useAuthStore((s) => s.clearSession);
  const [menuOpen, setMenuOpen] = useState(false);

  const sair = async () => {
    await logoutUseCase.execute();
    clearSession();
    router.push('/login');
  };

  const menu = (
    <Menu
      theme="dark"
      mode="inline"
      selectedKeys={[pathname]}
      items={items.map((i) => ({
        ...i,
        label: (
          <Link href={i.key} onClick={() => setMenuOpen(false)}>
            {i.label}
          </Link>
        ),
      }))}
    />
  );

  return (
    <Layout style={{ minHeight: '100vh' }}>
      <Sider breakpoint="lg" collapsedWidth={0} width={240} className="hc-proprietario-sider">
        <div className="hc-sider-brand">
          <AppBrand href="/proprietario/agenda" light size="sm" />
        </div>
        {menu}
      </Sider>
      <Layout>
        <Header className="hc-proprietario-topbar">
          <Space>
            <Button
              className="hc-proprietario-mobile-trigger"
              icon={<MenuOutlined />}
              onClick={() => setMenuOpen(true)}
              aria-label="Abrir menu"
            />
            <ThemeToggle />
            <Button icon={<LogoutOutlined />} onClick={sair}>
              Sair
            </Button>
          </Space>
        </Header>
        <Content className="hc-proprietario-content">{children}</Content>
      </Layout>

      <Drawer
        title="Menu"
        placement="left"
        open={menuOpen}
        onClose={() => setMenuOpen(false)}
        className="hc-proprietario-drawer"
        styles={{ body: { padding: 0, background: '#001529' } }}
      >
        <div className="hc-sider-brand" style={{ padding: 16 }}>
          <AppBrand href="/proprietario/agenda" light size="sm" />
        </div>
        {menu}
      </Drawer>
    </Layout>
  );
}
