import { expect } from '@playwright/test';
import { createBdd } from 'playwright-bdd';
import { ctx } from '../support/context';

const { Given, When, Then } = createBdd();

Given('que registro um novo estabelecimento', async ({ page }) => {
  await page.goto('/registrar');
  await page.getByLabel('Nome do estabelecimento').fill(ctx.nomeEstabelecimento);
  await page.getByLabel('E-mail').fill(ctx.email);
  await page.getByLabel('Senha').fill(ctx.senha);
  await page.getByRole('button', { name: 'Registrar' }).click();
  await expect(page).toHaveURL(/\/login/, { timeout: 15_000 });
});

When('faço login como proprietário', async ({ page }) => {
  await page.goto('/login');
  await page.getByLabel('E-mail').fill(ctx.email);
  await page.getByLabel('Senha').fill(ctx.senha);
  await page.getByRole('button', { name: 'Entrar' }).click();
  await expect(page).toHaveURL(/\/proprietario\/agendamentos/, { timeout: 15_000 });

  const session = await page.request.get('/api/bff/auth/session');
  expect(session.ok()).toBeTruthy();
  const data = (await session.json()) as { proprietarioId: string };
  ctx.proprietarioId = data.proprietarioId;
});

Then('devo estar autenticado no painel do proprietário', async ({ page }) => {
  await expect(page).toHaveURL(/\/proprietario\//);
});
