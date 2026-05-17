'use client';

import Link from 'next/link';
import { usePathname, useRouter } from 'next/navigation';
import { Button, Layout, Menu, Space } from 'antd';
import {
  CalendarOutlined,
  ClockCircleOutlined,
  LogoutOutlined,
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

  const sair = async () => {
    await logoutUseCase.execute();
    clearSession();
    router.push('/login');
  };

  return (
    <Layout style={{ minHeight: '100vh' }}>
      <Sider breakpoint="lg" collapsedWidth={0} width={240}>
        <div className="hc-sider-brand">
          <AppBrand href="/proprietario/agendamentos" light size="sm" />
        </div>
        <Menu
          theme="dark"
          mode="inline"
          selectedKeys={[pathname]}
          items={items.map((i) => ({
            ...i,
            label: <Link href={i.key}>{i.label}</Link>,
          }))}
        />
      </Sider>
      <Layout>
        <Header className="hc-proprietario-topbar">
          <Space>
            <ThemeToggle />
            <Button icon={<LogoutOutlined />} onClick={sair}>
              Sair
            </Button>
          </Space>
        </Header>
        <Content className="hc-proprietario-content">{children}</Content>
      </Layout>
    </Layout>
  );
}
