import { expect } from '@playwright/test';
import { createBdd } from 'playwright-bdd';
import { ctx } from '../support/context';

const { When, Then } = createBdd();

async function aguardarSessaoProprietario(page: import('@playwright/test').Page) {
  await page.waitForResponse(
    (r) => r.url().includes('/api/bff/auth/session'),
    { timeout: 15_000 },
  );
  await page
    .locator('.ant-spin-spinning')
    .waitFor({ state: 'hidden', timeout: 15_000 })
    .catch(() => undefined);
}

When(
  'cadastro o procedimento {string} com valor {int} e duração {int} minutos',
  async ({ page }, nome: string, valor: number, minutos: number) => {
    ctx.nomeProcedimento = nome;
    await page.goto('/proprietario/procedimentos');
    await aguardarSessaoProprietario(page);
    await page.getByRole('button', { name: 'Novo procedimento' }).click();
    const modal = page.locator('.ant-modal');
    await expect(modal).toBeVisible();
    const inputs = modal.locator('input');
    await inputs.nth(0).fill(nome);
    await inputs.nth(1).click();
    await inputs.nth(1).fill(String(valor));
    await inputs.nth(2).click();
    await inputs.nth(2).fill(String(minutos));
    const criar = page.waitForResponse(
      (r) =>
        r.url().includes('/procedimentos') &&
        r.request().method() === 'POST' &&
        r.ok(),
    );
    await modal.getByRole('button', { name: 'Salvar' }).click();
    await criar;
    await expect(modal).toBeHidden({ timeout: 10_000 });
    await expect(page.getByRole('cell', { name: nome })).toBeVisible({ timeout: 15_000 });
  },
);

When('disponibilizo um horário na agenda', async ({ page }) => {
  await page.goto('/proprietario/agenda');
  await aguardarSessaoProprietario(page);

  const session = await page.request.get('/api/bff/auth/session');
  expect(session.ok()).toBeTruthy();
  const { proprietarioId } = (await session.json()) as { proprietarioId: string };

  const inicio = new Date();
  inicio.setDate(inicio.getDate() + 1);
  inicio.setHours(10, 0, 0, 0);

  const res = await page.request.post(`/api/bff/proprietarios/${proprietarioId}/slots`, {
    data: { inicio: inicio.toISOString() },
  });
  expect(res.ok()).toBeTruthy();

  await page.reload();
  await aguardarSessaoProprietario(page);
  await expect(
    page.locator('.hc-week-slot, .hc-slot-chip, .ant-table-tbody tr').first(),
  ).toBeVisible({ timeout: 15_000 });
});

When('confirmo o agendamento pendente do cliente', async ({ page }) => {
  await page.goto('/proprietario/agendamentos');
  await aguardarSessaoProprietario(page);
  const row = page.locator('tr', { hasText: ctx.nomeCliente });
  await expect(row).toBeVisible({ timeout: 15_000 });
  const confirmar = page.waitForResponse(
    (r) =>
      r.url().includes('/confirmar') &&
      r.request().method() === 'POST' &&
      r.ok(),
  );
  await row.getByRole('button', { name: 'Confirmar' }).click();
  await page.locator('.ant-popconfirm').getByRole('button', { name: 'OK' }).click();
  await confirmar;
  await expect(row.getByText('Confirmado')).toBeVisible({ timeout: 15_000 });
});

When('registro o atendimento do agendamento confirmado', async ({ page }) => {
  await page.goto('/proprietario/agendamentos');
  await aguardarSessaoProprietario(page);
  const row = page.locator('tr', { hasText: ctx.nomeCliente });
  await expect(row).toBeVisible({ timeout: 15_000 });
  await row.getByRole('button', { name: 'Registrar atendimento' }).click();
  const drawer = page.locator('.ant-drawer');
  await expect(drawer).toBeVisible({ timeout: 10_000 });
  const registrar = page.waitForResponse(
    (r) =>
      r.url().includes('/atendimento') &&
      r.request().method() === 'POST' &&
      r.ok(),
  );
  await drawer.getByRole('button', { name: 'Registrar atendimento' }).click();
  await registrar;
});

When('marco o atendimento como realizado', async ({ page }) => {
  await page.goto('/proprietario/atendimentos');
  await aguardarSessaoProprietario(page);
  await expect(page.locator('.ant-table-tbody tr').first()).toBeVisible({ timeout: 15_000 });
  const patch = page.waitForResponse(
    (r) =>
      r.url().includes('/atendimentos/') &&
      r.request().method() === 'PATCH' &&
      r.ok(),
  );
  await page.getByRole('combobox').first().click();
  await page.locator('.ant-select-item-option').filter({ hasText: 'Realizado' }).click();
  await page.locator('.ant-modal-confirm').getByRole('button', { name: 'OK' }).click();
  await patch;
});

When('consulto a avaliação do agendamento do cliente', async ({ page }) => {
  await page.goto('/proprietario/agendamentos');
  const row = page.locator('tr', { hasText: ctx.nomeCliente });
  await row.getByRole('button', { name: 'Avaliação' }).click();
  await expect(page.getByText('Nota', { exact: false })).toBeVisible({ timeout: 10_000 });
});

Then('devo ver o procedimento {string} na listagem', async ({ page }, nome: string) => {
  await expect(page.getByRole('cell', { name: nome })).toBeVisible();
});

Then('o agendamento do cliente deve aparecer como confirmado', async ({ page }) => {
  await page.goto('/proprietario/agendamentos');
  await expect(page.locator('tr', { hasText: ctx.nomeCliente }).getByText('Confirmado')).toBeVisible({
    timeout: 15_000,
  });
});
