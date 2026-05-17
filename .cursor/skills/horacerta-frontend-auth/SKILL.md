---
name: horacerta-frontend-auth
description: Implements authentication and session for HoraCerta web portal using httpOnly JWT cookies for proprietário and cookies for cliente session, Next.js Route Handlers (BFF), and middleware. Use when building login, logout, protected routes, or cookie-based API calls for HoraCerta.Web.
---

# HoraCerta Frontend — Auth & Session

Apply together with **horacerta-frontend** and [docs/frontend/spec.md](../../../docs/frontend/spec.md) §6.

## Decisions (fixed)

| Actor | Storage | Notes |
|-------|---------|--------|
| Proprietário | **httpOnly cookie** for JWT | Never expose token to `window` or Zustand |
| Cliente | **Cookie** for `clienteId` + `proprietarioId` | Not `localStorage` |
| Zustand | `proprietarioId`, `isAuthenticated` only | Sync after login; not the secret |

## Preferred pattern: BFF Route Handlers

Backend today returns JWT in JSON (`POST /api/auth/login`). Front should:

1. Browser calls **Next** `POST /api/bff/auth/login` (same origin).
2. Route Handler calls `HoraCerta.Api` `POST /api/auth/login`.
3. Handler sets `Set-Cookie: horacerta_token=...; HttpOnly; Secure; SameSite=Lax; Path=/`.
4. Response body: `{ proprietarioId }` only (no token in JSON to browser).

Logout: Route Handler clears cookie (`Max-Age=0`).

## Middleware (`middleware.ts`)

- Match `/proprietario/:path*`
- If cookie `horacerta_token` missing → redirect `/login`
- Do not decode JWT in middleware unless needed; presence check is enough for MVP

## Proxying authenticated API calls

**Option A (MVP):** Route Handlers per resource (`/api/bff/agendamentos/...`) forward cookie as `Authorization: Bearer <token>` to .NET API.

**Option B:** Configure .NET API to accept JWT from cookie (requires backend change). Prefer A until backend supports cookies.

Axios from server components / Route Handlers: read cookie via `cookies()` from `next/headers`.

Axios from browser: use BFF routes only for protected operations, or `withCredentials` if API sets cookie on same site.

## Cliente session cookie

After `POST /api/clientes` + iniciar agendamento:

- Set cookie `horacerta_cliente` (JSON or separate cookies): `{ clienteId, proprietarioId }`
- `httpOnly` optional; if UI needs ids client-side, use Route Handler `GET /api/bff/cliente-sessao` that reads cookie server-side

`/e/[proprietarioId]/meus-agendamentos`: redirect to agendar if cookie missing.

## Zustand sync

```typescript
// After successful BFF login response { proprietarioId }
useAuthStore.getState().setSession(proprietarioId);
```

On logout: `clearSession()` + BFF logout clears cookie.

## Security checklist

- [ ] `HttpOnly` on JWT cookie
- [ ] `Secure` in production
- [ ] `SameSite=Lax` (adjust if cross-site)
- [ ] No JWT in `localStorage` / sessionStorage
- [ ] No JWT in Zustand persist
- [ ] Protected routes under `/proprietario/*`

## Backend reference

- `POST /api/auth/login` — body `{ email, senha }` → `{ token, proprietarioId }`
- `POST /api/auth/registrar` — creates establishment + credentials
- Protected API routes expect `Authorization: Bearer` today — BFF must attach header from httpOnly cookie
