'use client';

import { Button, Tooltip } from 'antd';
import { MoonOutlined, SunOutlined } from '@ant-design/icons';
import { useThemeStore } from '../stores/theme.store';

export function ThemeToggle() {
  const mode = useThemeStore((s) => s.mode);
  const toggle = useThemeStore((s) => s.toggle);
  const hydrated = useThemeStore((s) => s.hydrated);

  if (!hydrated) {
    return <Button type="text" icon={<SunOutlined />} aria-hidden />;
  }

  const isDark = mode === 'dark';

  return (
    <Tooltip title={isDark ? 'Modo claro' : 'Modo escuro'}>
      <Button
        type="text"
        aria-label={isDark ? 'Ativar modo claro' : 'Ativar modo escuro'}
        icon={isDark ? <SunOutlined /> : <MoonOutlined />}
        onClick={toggle}
      />
    </Tooltip>
  );
}
