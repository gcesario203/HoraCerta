import { Suspense } from 'react';
import { AuthShell } from '@/shared/presentation/layouts/auth-shell';
import { LoginForm } from './login-form';

export default function LoginPage() {
  return (
    <AuthShell>
      <Suspense fallback={null}>
        <LoginForm />
      </Suspense>
    </AuthShell>
  );
}
