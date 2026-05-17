import { expect, test } from '@playwright/test';

test('landing exibe link para área do proprietário', async ({ page }) => {
  await page.goto('/');
  await expect(page.getByRole('button', { name: 'Área do proprietário' })).toBeVisible();
});

test('login exibe formulário', async ({ page }) => {
  await page.goto('/login');
  await expect(page.getByLabel('E-mail')).toBeVisible();
  await expect(page.getByLabel('Senha')).toBeVisible();
});
