import { type NextRequest, NextResponse } from 'next/server';
import { TOKEN_COOKIE } from '@/shared/infrastructure/cookies';

function proprietarioIdFromToken(token: string): string | null {
  const parts = token.split('.');
  if (parts.length !== 3) return null;
  try {
    const base64 = parts[1].replace(/-/g, '+').replace(/_/g, '/');
    const payload = JSON.parse(Buffer.from(base64, 'base64').toString('utf8')) as {
      sub?: string;
    };
    return payload.sub ?? null;
  } catch {
    return null;
  }
}

export async function GET(request: NextRequest) {
  const token = request.cookies.get(TOKEN_COOKIE)?.value;
  if (!token) {
    return NextResponse.json({ mensagem: 'Não autenticado' }, { status: 401 });
  }

  const proprietarioId = proprietarioIdFromToken(token);
  if (!proprietarioId) {
    return NextResponse.json({ mensagem: 'Sessão inválida' }, { status: 401 });
  }

  return NextResponse.json({ proprietarioId });
}
