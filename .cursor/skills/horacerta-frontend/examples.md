# HoraCerta Frontend — Examples

## Axios factory (`shared/infrastructure/http/axios-client.ts`)

```typescript
import axios, { type AxiosInstance } from 'axios';

export function createApiClient(baseURL: string): AxiosInstance {
  return axios.create({
    baseURL,
    withCredentials: true,
    headers: { 'Content-Type': 'application/json' },
  });
}
```

## API layer (`procedimento/infrastructure/api/procedimento.api.ts`)

```typescript
import type { AxiosInstance } from 'axios';
import type { ProcedimentoDto } from '../../application/dtos/procedimento.dto';

export async function listarProcedimentosApi(
  client: AxiosInstance,
  proprietarioId: string,
): Promise<ProcedimentoDto[]> {
  const { data } = await client.get<ProcedimentoDto[]>(
    `/api/proprietarios/${proprietarioId}/procedimentos`,
  );
  return data;
}
```

## Repository (`procedimento/infrastructure/api/procedimento.repository.ts`)

```typescript
import type { IProcedimentoRepository } from '../../domain/repositories/procedimento.repository';
import { listarProcedimentosApi } from './procedimento.api';
import type { AxiosInstance } from 'axios';

export class ProcedimentoRepository implements IProcedimentoRepository {
  constructor(private readonly client: AxiosInstance) {}

  listarAtivos(proprietarioId: string) {
    return listarProcedimentosApi(this.client, proprietarioId);
  }
}
```

## Use case (`procedimento/application/use-cases/listar-procedimentos.use-case.ts`)

```typescript
import type { IProcedimentoRepository } from '../../domain/repositories/procedimento.repository';

export class ListarProcedimentosUseCase {
  constructor(private readonly repo: IProcedimentoRepository) {}

  async execute(proprietarioId: string) {
    return this.repo.listarAtivos(proprietarioId);
  }
}
```

## Hook (`procedimento/presentation/hooks/use-procedimentos.ts`)

```typescript
'use client';

import { useCallback, useEffect, useState } from 'react';
import { message } from 'antd';
import { listarProcedimentosUseCase } from '@/procedimento/application'; // barrel or direct import

export function useProcedimentos(proprietarioId: string) {
  const [items, setItems] = useState<ProcedimentoView[]>([]);
  const [loading, setLoading] = useState(true);

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const data = await listarProcedimentosUseCase.execute(proprietarioId);
      setItems(data);
    } catch {
      message.error('Não foi possível carregar os procedimentos.');
    } finally {
      setLoading(false);
    }
  }, [proprietarioId]);

  useEffect(() => {
    void load();
  }, [load]);

  return { items, loading, reload: load };
}
```

## Zustand (`auth/presentation/stores/auth.store.ts`)

```typescript
import { create } from 'zustand';

type AuthState = {
  proprietarioId: string | null;
  isAuthenticated: boolean;
  setSession: (proprietarioId: string) => void;
  clearSession: () => void;
};

export const useAuthStore = create<AuthState>((set) => ({
  proprietarioId: null,
  isAuthenticated: false,
  setSession: (proprietarioId) =>
    set({ proprietarioId, isAuthenticated: true }),
  clearSession: () =>
    set({ proprietarioId: null, isAuthenticated: false }),
}));
```

## Thin page (`app/proprietario/procedimentos/page.tsx`)

```typescript
import { ProcedimentosPage } from '@/procedimento/presentation/components/procedimentos-page';

export default function Page() {
  return <ProcedimentosPage />;
}
```

Components stay in `presentation/`; `app/` only wires routes.
