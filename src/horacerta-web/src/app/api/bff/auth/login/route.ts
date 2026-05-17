import { NextResponse } from 'next/server';
import { TOKEN_COOKIE } from '@/shared/infrastructure/cookies';

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

  const data = JSON.parse(text) as { token: string; proprietarioId: string };
  const response = NextResponse.json({ proprietarioId: data.proprietarioId });
  response.cookies.set(TOKEN_COOKIE, data.token, {
    httpOnly: true,
    sameSite: 'lax',
    path: '/',
    secure: process.env.NODE_ENV === 'production',
    maxAge: 60 * 60 * 8,
  });
  return response;
}
