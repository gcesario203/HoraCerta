import { NextResponse } from 'next/server';
import { TOKEN_COOKIE } from '@/shared/infrastructure/cookies';
import { cookieSecureFromRequest } from '@/shared/infrastructure/cookies-options';

const apiUrl = () => process.env.API_URL ?? 'http://localhost:5080';

export async function POST(request: Request) {
  const body = await request.json();

  const upstream = await fetch(`${apiUrl()}/api/auth/login`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(body),
  });

  const text = await upstream.text();
  if (!upstream.ok) {
    return new NextResponse(text, {
      status: upstream.status,
      headers: { 'Content-Type': 'application/json' },
    });
  }

  const data = JSON.parse(text) as Record<string, string>;
  const token = data.token ?? data.Token;
  const proprietarioId = data.proprietarioId ?? data.ProprietarioId;
  if (!token || !proprietarioId) {
    return NextResponse.json({ mensagem: 'Resposta de login inválida' }, { status: 502 });
  }

  const response = NextResponse.json({ proprietarioId });
  response.cookies.set(TOKEN_COOKIE, token, {
    httpOnly: true,
    sameSite: 'lax',
    path: '/',
    secure: cookieSecureFromRequest(request),
    maxAge: 60 * 60 * 8,
  });
  return response;
}
