# Backend — HoraCerta

Documentação spec-driven da API, alinhada à implementação em `src/HoraCerta.*`.

| Documento | Conteúdo |
|-----------|----------|
| [spec.md](./spec.md) | Arquitetura, endpoints, contratos, auth, persistência, Docker |
| [../docs.md](../docs.md) | Requisitos de negócio originais |
| [../agregados.md](../agregados.md) | Modelo de agregados |
| [../frontend/spec.md](../frontend/spec.md) | Portal consumidor |

## Execução

```bash
cd src/HoraCerta.Api
dotnet run
```

Swagger: http://localhost:5080/swagger

## Docker (API + Web + PostgreSQL)

Na raiz do repositório:

```bash
docker compose up --build
```

- Portal: http://localhost:3000  
- API: http://localhost:5080  
