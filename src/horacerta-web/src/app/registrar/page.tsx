'use client';

import { useState } from 'react';
import Link from 'next/link';
import { useRouter } from 'next/navigation';
import { App, Button, Card, Form, Input, Typography } from 'antd';
import { registrarUseCase } from '@/auth/application';
import { AuthShell } from '@/shared/presentation/layouts/auth-shell';
import { extractApiMessage } from '@/shared/infrastructure/http/api-error';

export default function RegistrarPage() {
  const router = useRouter();
  const { message } = App.useApp();
  const [loading, setLoading] = useState(false);

  const onFinish = async (values: {
    nomeEstabelecimento: string;
    email: string;
    senha: string;
  }) => {
    setLoading(true);
    try {
      await registrarUseCase.execute({
        nomeEstabelecimento: values.nomeEstabelecimento,
        email: values.email,
        senha: values.senha,
      });
      message.success('Conta criada. Faça login para continuar.');
      router.push('/login');
    } catch (error) {
      message.error(extractApiMessage(error));
    } finally {
      setLoading(false);
    }
  };

  return (
    <AuthShell>
      <Card
        title="Registrar estabelecimento"
        className="hc-card-elevated"
        style={{ maxWidth: 440, width: '100%' }}
      >
        <Form layout="vertical" onFinish={onFinish}>
          <Form.Item
            label="Nome do estabelecimento"
            name="nomeEstabelecimento"
            rules={[{ required: true, message: 'Informe o nome' }]}
          >
            <Input />
          </Form.Item>
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
            rules={[{ required: true, min: 6, message: 'Mínimo 6 caracteres' }]}
          >
            <Input.Password />
          </Form.Item>
          <Button type="primary" htmlType="submit" block loading={loading} size="large">
            Registrar
          </Button>
        </Form>
        <Typography.Paragraph style={{ marginTop: 16, marginBottom: 0 }}>
          <Link href="/login">Já tenho conta</Link>
        </Typography.Paragraph>
      </Card>
    </AuthShell>
  );
}
