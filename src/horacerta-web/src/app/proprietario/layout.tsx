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
      <Sider breakpoint="lg" collapsedWidth={0}>
        <div style={{ height: 64, display: 'flex', alignItems: 'center', padding: '0 16px' }}>
          <Link href="/proprietario/agendamentos" style={{ color: '#fff', fontWeight: 600 }}>
            HoraCerta
          </Link>
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
        <Header
          style={{
            background: '#fff',
            padding: '0 24px',
            display: 'flex',
            justifyContent: 'flex-end',
          }}
        >
          <Space>
            <Button icon={<LogoutOutlined />} onClick={sair}>
              Sair
            </Button>
          </Space>
        </Header>
        <Content style={{ margin: 24 }}>{children}</Content>
      </Layout>
    </Layout>
  );
}
