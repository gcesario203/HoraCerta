import { expect } from '@playwright/test';
import { createBdd } from 'playwright-bdd';
import { ctx } from '../support/context';

const { When, Then } = createBdd();

When('acesso a página do estabelecimento', async ({ page }) => {
  await page.goto(`/e/${ctx.proprietarioId}`);
});

When('inicio o agendamento com meus dados', async ({ page }) => {
  await page.goto(`/e/${ctx.proprietarioId}/agendar`);
  await page.getByLabel('Nome').fill(ctx.nomeCliente);
  await page.getByLabel('Telefone').fill(ctx.telefoneCliente);
  await page.getByRole('button', { name: 'Continuar' }).click();
  await expect(page.locator('.ant-radio-wrapper').first()).toBeVisible({ timeout: 15_000 });
});

When('escolho o procedimento cadastrado', async ({ page }) => {
  const opcao = page.locator('.ant-radio-wrapper', { hasText: ctx.nomeProcedimento });
  await expect(opcao).toBeVisible({ timeout: 15_000 });
  await opcao.click();
  await page.getByRole('button', { name: 'Continuar' }).click();
});

When('escolho o primeiro horário disponível', async ({ page }) => {
  const slot = page.locator('.ant-radio-wrapper').first();
  await expect(slot).toBeVisible({ timeout: 15_000 });
  await slot.click();
  const criar = page.waitForResponse(
    (r) =>
      r.url().includes('/agendamentos/iniciar') &&
      r.request().method() === 'POST' &&
      r.ok(),
  );
  await page.getByRole('button', { name: 'Confirmar agendamento' }).click();
  await criar;
  await expect(page.getByText('Agendamento enviado!')).toBeVisible({ timeout: 15_000 });
});

Then('devo ver a mensagem de agendamento pendente', async ({ page }) => {
  await expect(page.getByText('Agendamento enviado!')).toBeVisible({ timeout: 15_000 });
  await expect(page.getByText(/pendente de confirmação/i)).toBeVisible();
});

Then('devo ver o texto informativo sobre lembrete', async ({ page }) => {
  await expect(page.getByText(/lembrete/i)).toBeVisible();
});

When('acesso meus agendamentos', async ({ page }) => {
  await page.goto(`/e/${ctx.proprietarioId}/meus-agendamentos`);
  await expect(page.getByText(ctx.nomeProcedimento)).toBeVisible({ timeout: 15_000 });
});

Then('devo ver meu agendamento com o procedimento', async ({ page }) => {
  await expect(page.getByText(ctx.nomeProcedimento)).toBeVisible();
});

Then('não devo ver opção de cancelar ou remarcar', async ({ page }) => {
  await expect(page.getByRole('button', { name: /cancelar/i })).toHaveCount(0);
  await expect(page.getByRole('button', { name: /remarcar/i })).toHaveCount(0);
});

When('avalio o atendimento com nota {int}', async ({ page }, nota: number) => {
  await page.goto(`/e/${ctx.proprietarioId}/meus-agendamentos`);
  await expect(page.getByRole('link', { name: 'Avaliar atendimento' })).toBeVisible({
    timeout: 15_000,
  });
  await page.getByRole('link', { name: 'Avaliar atendimento' }).click();
  await expect(page).toHaveURL(/\/avaliar\//, { timeout: 15_000 });
  await expect(page.getByRole('button', { name: 'Enviar avaliação' })).toBeVisible({
    timeout: 15_000,
  });
  if (nota !== 5) {
    await page.getByRole('radio').nth(nota - 1).click();
  }
  const enviar = page.waitForResponse(
    (r) =>
      r.url().includes('/avaliar') &&
      r.request().method() === 'POST' &&
      r.ok(),
  );
  await page.getByRole('button', { name: 'Enviar avaliação' }).click();
  await enviar;
  await expect(page).toHaveURL(/meus-agendamentos/, { timeout: 15_000 });
});
