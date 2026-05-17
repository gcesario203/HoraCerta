import { describe, expect, it } from 'vitest';
import { formatarDataHora, labelEstado } from './format';

describe('format', () => {
  it('formata data/hora em pt-BR', () => {
    const texto = formatarDataHora('2026-05-17T14:30:00');
    expect(texto).toMatch(/17\/05\/2026/);
    expect(texto).toMatch(/14:30/);
  });

  it('traduz estados conhecidos', () => {
    expect(labelEstado('PENDENTE')).toBe('Pendente');
    expect(labelEstado('REALIZADO')).toBe('Realizado');
  });
});
