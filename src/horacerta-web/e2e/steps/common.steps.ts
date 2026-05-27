import { expect } from '@playwright/test';
import { createBdd } from 'playwright-bdd';

const { Given, Then } = createBdd();

Given('que estou na página inicial', async ({ page }) => {
  await page.goto('/');
});

Then('devo ver o campo de busca do catálogo', async ({ page }) => {
  await expect(
    page.getByPlaceholder('Buscar por nome do estabelecimento…'),
  ).toBeVisible();
});

Given('que estou na página de login', async ({ page }) => {
  await page.goto('/login');
});

Then('devo ver o botão {string}', async ({ page }, nome: string) => {
  await expect(page.getByRole('button', { name: nome })).toBeVisible();
});

Then('devo ver o campo {string}', async ({ page }, label: string) => {
  await expect(page.getByLabel(label)).toBeVisible();
});
