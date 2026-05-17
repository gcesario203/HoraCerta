import axios, { type AxiosInstance } from 'axios';

export function createApiClient(baseURL: string, withCredentials = false): AxiosInstance {
  return axios.create({
    baseURL,
    withCredentials,
    headers: { 'Content-Type': 'application/json' },
  });
}

/** Chamadas públicas (rewrite → backend). */
export const publicApiClient = createApiClient('/api/core');

/** Chamadas autenticadas (BFF adiciona Bearer). */
export const bffApiClient = createApiClient('/api/bff', true);
