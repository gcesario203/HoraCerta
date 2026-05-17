import { NextResponse } from 'next/server';
import { TOKEN_COOKIE } from '@/shared/infrastructure/cookies';

export async function POST() {
  const response = NextResponse.json({ ok: true });
  response.cookies.set(TOKEN_COOKIE, '', {
    httpOnly: true,
    path: '/',
    maxAge: 0,
  });
  return response;
}
