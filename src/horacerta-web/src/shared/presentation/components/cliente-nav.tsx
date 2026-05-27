'use client';

import Link from 'next/link';
import { usePathname } from 'next/navigation';
import { Button } from 'antd';
import {
  CalendarOutlined,
  HomeOutlined,
  LogoutOutlined,
  UnorderedListOutlined,
} from '@ant-design/icons';
import { useClienteEstabelecimento } from '@/cliente/presentation/hooks/use-cliente-estabelecimento';

type ClienteNavProps = {
  proprietarioId: string;
};

const items = (proprietarioId: string) => [
  { href: `/e/${proprietarioId}`, label: 'Início', icon: HomeOutlined, exact: true },
  { href: `/e/${proprietarioId}/agendar`, label: 'Agendar', icon: CalendarOutlined, exact: false },
  {
    href: `/e/${proprietarioId}/meus-agendamentos`,
    label: 'Meus agendamentos',
    icon: UnorderedListOutlined,
    exact: false,
  },
];

export function ClienteNav({ proprietarioId }: ClienteNavProps) {
  const pathname = usePathname();
  const links = items(proprietarioId);
  const { ready, semSessao, sair } = useClienteEstabelecimento(proprietarioId);

  return (
    <nav className="hc-cliente-nav" aria-label="Área do cliente">
      {links.map(({ href, label, icon: Icon, exact }) => {
        const active = exact ? pathname === href : pathname.startsWith(href);
        return (
          <Link
            key={href}
            href={href}
            className={`hc-cliente-nav__item${active ? ' hc-cliente-nav__item--active' : ''}`}
          >
            <Icon />
            <span>{label}</span>
          </Link>
        );
      })}
      {ready && !semSessao ? (
        <Button
          type="text"
          className="hc-cliente-nav__sair"
          icon={<LogoutOutlined />}
          onClick={() => void sair()}
        >
          Sair
        </Button>
      ) : null}
    </nav>
  );
}
