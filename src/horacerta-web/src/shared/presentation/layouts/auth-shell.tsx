'use client';

import type { ReactNode } from 'react';
import { AppBrand } from '../components/app-brand';
import { ThemeToggle } from '../components/theme-toggle';

type AuthShellProps = {
  children: ReactNode;
};

export function AuthShell({ children }: AuthShellProps) {
  return (
    <div className="hc-auth-shell">
      <header className="hc-auth-shell__header">
        <AppBrand href="/" />
        <ThemeToggle />
      </header>
      <div className="hc-auth-shell__body">{children}</div>
    </div>
  );
}
