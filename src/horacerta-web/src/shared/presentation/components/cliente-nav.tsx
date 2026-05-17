'use client';

import Link from 'next/link';
import { usePathname } from 'next/navigation';
import { CalendarOutlined, HomeOutlined, UnorderedListOutlined } from '@ant-design/icons';

type ClienteNavProps = {
  proprietarioId: string;
};

const items = (proprietarioId: string) => [
  { href: `/e/${proprietarioId}`, label: 'Início', icon: HomeOutlined, exact: true },
  { href: `/e/${proprietarioId}/agendar`, label: 'Agendar', icon: CalendarOutlined, exact: false },
  {
    href: `/e/${proprietarioId}/meus-agendamentos`,
    label: 'Meus horários',
    icon: UnorderedListOutlined,
    exact: false,
  },
];

export function ClienteNav({ proprietarioId }: ClienteNavProps) {
  const pathname = usePathname();
  const links = items(proprietarioId);

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
    </nav>
  );
}
