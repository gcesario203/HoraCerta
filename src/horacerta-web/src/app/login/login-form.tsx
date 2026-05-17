'use client';

import { useState } from 'react';
import Link from 'next/link';
import { useRouter, useSearchParams } from 'next/navigation';
import { App, Button, Card, Form, Input, Typography } from 'antd';
import { loginUseCase } from '@/auth/application';
import { useAuthStore } from '@/auth/presentation/stores/auth.store';
import { extractApiMessage } from '@/shared/infrastructure/http/api-error';

export function LoginForm() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const { message } = App.useApp();
  const setSession = useAuthStore((s) => s.setSession);
  const [loading, setLoading] = useState(false);

  const onFinish = async (values: { email: string; senha: string }) => {
    setLoading(true);
    try {
      const res = await loginUseCase.execute(values);
      setSession(res.proprietarioId);
      const redirect = searchParams.get('redirect') ?? '/proprietario/agendamentos';
      router.push(redirect);
    } catch (error) {
      message.error(extractApiMessage(error));
    } finally {
      setLoading(false);
    }
  };

  return (
    <Card title="Login do proprietário" style={{ maxWidth: 400, width: '100%' }}>
      <Form layout="vertical" onFinish={onFinish}>
        <Form.Item
          label="E-mail"
          name="email"
          rules={[{ required: true, type: 'email', message: 'Informe um e-mail válido' }]}
        >
          <Input />
        </Form.Item>
        <Form.Item
          label="Senha"
          name="senha"
          rules={[{ required: true, message: 'Informe a senha' }]}
        >
          <Input.Password />
        </Form.Item>
        <Button type="primary" htmlType="submit" block loading={loading}>
          Entrar
        </Button>
      </Form>
      <Typography.Paragraph style={{ marginTop: 16, marginBottom: 0 }}>
        <Link href="/registrar">Criar conta</Link> · <Link href="/">Início</Link>
      </Typography.Paragraph>
    </Card>
  );
}
