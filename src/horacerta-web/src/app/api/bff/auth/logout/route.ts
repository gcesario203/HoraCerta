import { NextResponse } from 'next/server';
import { TOKEN_COOKIE } from '@/shared/infrastructure/cookies';
import { cookieSecureFromRequest } from '@/shared/infrastructure/cookies-options';

export async function POST(request: Request) {
  const response = NextResponse.json({ ok: true });
  response.cookies.set(TOKEN_COOKIE, '', {
    httpOnly: true,
    sameSite: 'lax',
    path: '/',
    secure: cookieSecureFromRequest(request),
    maxAge: 0,
  });
  return response;
}
