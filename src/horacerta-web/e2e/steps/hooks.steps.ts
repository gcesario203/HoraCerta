import { createBdd } from 'playwright-bdd';
import { resetContext } from '../support/context';

const { Before } = createBdd();

Before({ tags: '@integracao' }, () => {
  resetContext();
});
