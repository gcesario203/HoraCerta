import { expect } from '@playwright/test';
import { createBdd } from 'playwright-bdd';
import { ctx } from '../support/context';

const { When, Then } = createBdd();

When(
  'cadastro o procedimento {string} com valor {int} e duração {int} minutos',
  async ({ page }, nome: string, valor: number, minutos: number) => {
    ctx.nomeProcedimento = nome;
    await page.goto('/proprietario/procedimentos');
    await page.getByRole('button', { name: 'Novo procedimento' }).click();
    await page.getByLabel('Nome').fill(nome);
    await page.getByLabel('Valor (R$)').fill(String(valor));
    await page.getByLabel('Tempo estimado (minutos)').fill(String(minutos));
    await page.getByRole('button', { name: 'Salvar' }).click();
    await expect(page.getByRole('cell', { name: nome })).toBeVisible({ timeout: 15_000 });
  },
);

When('disponibilizo um horário na agenda', async ({ page }) => {
  await page.goto('/proprietario/agenda');
  await page.getByRole('button', { name: 'Novo horário' }).click();
  await page.getByLabel('Data e hora').click();
  const nowBtn = page.locator('.ant-picker-now-btn');
  if (await nowBtn.isVisible()) {
    await nowBtn.click();
  }
  await page.locator('.ant-picker-ok button').click();
  await page.getByRole('button', { name: 'Salvar' }).click();
  await expect(page.locator('.ant-table-tbody tr').first()).toBeVisible({ timeout: 15_000 });
});

When('confirmo o agendamento pendente do cliente', async ({ page }) => {
  await page.goto('/proprietario/agendamentos');
  const row = page.locator('tr', { hasText: ctx.nomeCliente });
  await expect(row).toBeVisible({ timeout: 15_000 });
  await row.getByRole('button', { name: 'Confirmar' }).click();
  await expect(row.getByText('Confirmado')).toBeVisible({ timeout: 15_000 });
});

When('registro o atendimento do agendamento confirmado', async ({ page }) => {
  await page.goto('/proprietario/agendamentos');
  const row = page.locator('tr', { hasText: ctx.nomeCliente });
  await row.getByRole('button', { name: 'Atendimento' }).click();
  await page.getByRole('button', { name: 'Registrar atendimento' }).click();
  await page.waitForTimeout(1000);
});

When('marco o atendimento como realizado', async ({ page }) => {
  await page.goto('/proprietario/atendimentos');
  await page.getByRole('combobox').first().click();
  await page.getByTitle('Realizado').click();
  await page.waitForTimeout(1000);
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
