# TaskNow REST

API .NET 10 do TaskNow.

## Rodar localmente

1. Suba o banco de dados (por exemplo, PostgreSQL no Docker, caso possua o `docker-compose` configurado):

```powershell
docker compose up -d
```
*(Nota: Se estiver usando o SQLite configurado no AGENTS.md, você não precisa subir nenhum contêiner)*

2. Atualize o banco de dados (caso esteja usando Migrations):
```powershell
dotnet ef database update --project TaskNow.DAO --startup-project TaskNow.API
```

3. Inicie a API:

```powershell
dotnet restore
dotnet run --project TaskNow.API --launch-profile http
```

3. Abra o Swagger:

```text
http://localhost:5232/swagger
```

## Configuracao local

A API usa estes valores por padrao em desenvolvimento:

```text
ConnectionStrings__Default=Host=localhost;Port=5432;Database=tasknow;Username=postgres;Password=postgres
ASPNETCORE_URLS=http://localhost:5232
```

Para sobrescrever sem alterar arquivos versionados, use variaveis de ambiente ou User Secrets.

## Organizacao

- `TaskNow.API`: controllers, middlewares, hubs, Swagger e configuracao da aplicacao.
- `TaskNow.BLL`: regras de negocio.
- `TaskNow.DAL`: acesso a dados.
- `TaskNow.DAO`: DbContext e entidades persistidas.
- `TaskNow.DTO`: contratos de entrada e saida.
- `TaskNow.XUnitTest`: testes automatizados.