# TaskNow — Plano de Estruturação do Backend (API)

> Clone simplificado de Trello/kanban em .NET 10, seguindo o padrão BLL / DAL / DTO / DAO / API / XUnitTest. **Este projeto existe pra ensinar** — é a base sobre a qual dois devs em formação vão receber tarefas incrementais, então a arquitetura é deliberadamente igual a de um projeto "de verdade" (mesmo padrão usado em produção), só que com domínio simples o suficiente pra aprender sem se perder no negócio.
>
> Leia a seção 14 antes de distribuir ou pegar a primeira tarefa — é o guia de onboarding e a trilha de aprendizado.

---

## 1. Visão Geral do Produto

| Módulo | Funcionalidade |
|---|---|
| **Quadros (Boards)** | Cada usuário cria quadros; um quadro organiza o trabalho de um "projeto" |
| **Listas (Colunas)** | Cada quadro tem colunas (ex.: A Fazer, Em Andamento, Feito), reordenáveis |
| **Cartões (Cards)** | Tarefas dentro de uma lista; têm título, descrição, prazo, responsável; arrastáveis entre listas |
| **Etiquetas (Labels)** | Cor + nome, atribuídas a cartões pra categorizar |
| **Comentários** | Discussão por cartão |
| **Membros** | Um quadro pode ter mais de um usuário (dono + convidados) |
| **Notificações em tempo real** | Quando alguém move/edita um cartão, os outros membros veem sem dar F5 |

---

## 2. Estrutura de Projetos (.NET Solution)

```
TaskNow.sln
├── TaskNow.API          # ASP.NET Core — Controllers, Hubs (SignalR), Middlewares, Program.cs
├── TaskNow.BLL          # Regras de negócio — Services/BLLs
├── TaskNow.DAL          # Acesso a dados — DALs sobre EF Core
├── TaskNow.DAO          # DbContext, Entities (modelos EF), Migrations
├── TaskNow.DTO          # DTOs, Requests, Utils (RetornoDTO, PagedResultDTO…)
└── TaskNow.XUnitTest    # Testes unitários (xUnit + Moq)
```

### Dependências entre projetos

```
API  →  BLL  →  DAL  →  DAO
              ↘  DTO  ↙
```

Essa é a mesma arquitetura usada em projetos reais (BLL/DAL/DTO/DAO) — aprender ela aqui, num domínio simples, é o que faz o pulo pra um projeto de produção ser tranquilo depois.

---

## 3. TaskNow.DAO

### 3.1 Entities (modelos EF Core)

```
TaskNow.DAO/
├── TaskNowDbContext.cs
├── ApplicationUser.cs            # IdentityUser
└── Entities/
    ├── Quadro.cs                 # Board: nome, descrição, dono
    ├── MembroQuadro.cs           # Usuario + Quadro + Papel (Dono, Membro)
    ├── Lista.cs                  # Coluna: nome, ordem, QuadroId
    ├── Cartao.cs                 # Título, descrição, ordem, prazo, responsável, ListaId
    ├── Etiqueta.cs                # Nome + cor, por quadro
    ├── CartaoEtiqueta.cs         # N:N entre Cartao e Etiqueta
    └── Comentario.cs             # Texto + autor + CartaoId
```

### 3.2 TaskNowDbContext.cs (resumo)

```csharp
public class TaskNowDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Quadro> Quadros { get; set; }
    public DbSet<MembroQuadro> MembrosQuadro { get; set; }
    public DbSet<Lista> Listas { get; set; }
    public DbSet<Cartao> Cartoes { get; set; }
    public DbSet<Etiqueta> Etiquetas { get; set; }
    public DbSet<CartaoEtiqueta> CartoesEtiquetas { get; set; }
    public DbSet<Comentario> Comentarios { get; set; }
}
```

---

## 4. TaskNow.DTO

```
TaskNow.DTO/
├── Utils/
│   ├── RetornoDTO.cs              # RetornoDTO<T> — sucesso/erro padronizado
│   ├── PagedResultDTO.cs
│   └── AutenticacaoDTO.cs
├── Entities/
│   ├── QuadroDTO.cs
│   ├── ListaDTO.cs
│   ├── CartaoDTO.cs
│   ├── EtiquetaDTO.cs
│   ├── ComentarioDTO.cs
│   └── MembroQuadroDTO.cs
└── Requests/
    ├── Auth/
    │   ├── RegistrarRequestDTO.cs
    │   └── LoginRequestDTO.cs
    ├── Quadro/
    │   ├── QuadroCriarRequestDTO.cs
    │   └── QuadroEditarRequestDTO.cs
    ├── Lista/
    │   ├── ListaCriarRequestDTO.cs
    │   └── ListaReordenarRequestDTO.cs      # nova ordem das listas
    ├── Cartao/
    │   ├── CartaoCriarRequestDTO.cs
    │   ├── CartaoEditarRequestDTO.cs
    │   └── CartaoMoverRequestDTO.cs         # novo ListaId + nova ordem (drag-and-drop)
    ├── Etiqueta/
    │   └── EtiquetaCriarRequestDTO.cs
    ├── Comentario/
    │   └── ComentarioCriarRequestDTO.cs
    └── Membro/
        └── MembroConvidarRequestDTO.cs
```

---

## 5. TaskNow.DAL

### 5.1 Base genérico

```
TaskNow.DAL/
├── Base/
│   └── BaseDAL.cs                 # BaseDAL<TEntity, TDTO> — CRUD genérico
└── Entities/
    ├── Interfaces/
    │   ├── IQuadroDAL.cs
    │   ├── IListaDAL.cs
    │   ├── ICartaoDAL.cs
    │   ├── IEtiquetaDAL.cs
    │   ├── IComentarioDAL.cs
    │   └── IMembroQuadroDAL.cs
    ├── QuadroDAL.cs                # + ObterPorUsuarioAsync (quadros que o usuário é dono/membro)
    ├── ListaDAL.cs                 # + ObterPorQuadroOrdenadoAsync
    ├── CartaoDAL.cs                # + ObterPorListaOrdenadoAsync
    ├── EtiquetaDAL.cs
    ├── ComentarioDAL.cs            # + ObterPorCartaoAsync
    └── MembroQuadroDAL.cs          # + EhMembroAsync(quadroId, usuarioId)
```

### 5.2 Exemplo: métodos extras em CartaoDAL

```csharp
public interface ICartaoDAL
{
    // herdado via BaseDAL
    Task<CartaoDTO?> GetByIdAsync(int id);
    Task<CartaoDTO> CreateAsync(CartaoDTO dto);
    Task<CartaoDTO?> EditAsync(int id, CartaoDTO dto);
    Task<bool> DeleteAsync(int id);
    IQueryable<Cartao> GetQuery(bool asNoTracking, params Expression<Func<Cartao, object>>[] includes);
    // específicos
    Task<List<CartaoDTO>> ObterPorListaOrdenadoAsync(int listaId);
    Task<int> ObterProximaOrdemAsync(int listaId);
}
```

---

## 6. TaskNow.BLL

```
TaskNow.BLL/
├── Utils/
│   ├── Interfaces/
│   │   └── IUsuarioContexto.cs
│   └── UsuarioContexto.cs         # Resolve userId do JWT
└── Entities/
    ├── Interfaces/
    │   ├── IQuadroBLL.cs
    │   ├── IListaBLL.cs
    │   ├── ICartaoBLL.cs          # inclui MoverAsync (drag-and-drop)
    │   ├── IEtiquetaBLL.cs
    │   ├── IComentarioBLL.cs
    │   └── IMembroQuadroBLL.cs    # convidar/remover membro + checar permissão
    ├── QuadroBLL.cs
    ├── ListaBLL.cs
    ├── CartaoBLL.cs
    ├── EtiquetaBLL.cs
    ├── ComentarioBLL.cs
    └── MembroQuadroBLL.cs
```

### 6.1 CartaoBLL — a regra mais "interessante" do projeto (mover/reordenar)

```csharp
public interface ICartaoBLL
{
    Task<RetornoDTO<CartaoDTO>> CriarAsync(CartaoCriarRequestDTO request);
    Task<RetornoDTO<CartaoDTO>> EditarAsync(int id, CartaoEditarRequestDTO request);

    // Drag-and-drop: move o cartão pra outra lista e/ou outra posição
    // Recalcula a "ordem" dos cartões afetados na lista de origem e destino
    Task<RetornoDTO<bool>> MoverAsync(int cartaoId, CartaoMoverRequestDTO request);

    Task<RetornoDTO<bool>> ExcluirAsync(int id);
}
```

> Essa é a primeira tarefa de nível "intermediário" do projeto — bom gancho pra ensinar que uma ação simples na UI (arrastar um card) pode exigir uma regra de negócio não trivial no backend (recalcular ordem sem duplicar posição).

---

## 7. TaskNow.API

```
TaskNow.API/
├── Controllers/
│   ├── AuthController.cs           # registrar, login
│   ├── QuadrosController.cs
│   ├── ListasController.cs         # inclui POST /reordenar
│   ├── CartoesController.cs        # inclui POST /{id}/mover
│   ├── EtiquetasController.cs
│   ├── ComentariosController.cs
│   └── MembrosController.cs        # convidar/remover/listar membros do quadro
├── Hubs/
│   └── QuadroHub.cs                # SignalR — grupo por QuadroId, notifica CartaoMovido/CartaoCriado/etc.
├── Middlewares/
│   └── ExceptionMiddleware.cs      # Erros globais → RetornoDTO.Fail
├── Mapper/
│   └── MappingProfile.cs           # AutoMapper — Entity ↔ DTO
├── Program.cs
└── appsettings.json
```

### 7.1 Program.cs — registros essenciais

```csharp
// EF Core + Identity
builder.Services.AddDbContext<TaskNowDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<TaskNowDbContext>()
    .AddDefaultTokenProviders();

// JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => { /* configurar */ });

// SignalR
builder.Services.AddSignalR();

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// DALs
builder.Services.AddScoped<IQuadroDAL, QuadroDAL>();
builder.Services.AddScoped<IListaDAL, ListaDAL>();
builder.Services.AddScoped<ICartaoDAL, CartaoDAL>();
// ...

// BLLs
builder.Services.AddScoped<IQuadroBLL, QuadroBLL>();
builder.Services.AddScoped<IListaBLL, ListaBLL>();
builder.Services.AddScoped<ICartaoBLL, CartaoBLL>();
// ...

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUsuarioContexto, UsuarioContexto>();

// ...

app.MapHub<QuadroHub>("/hubs/quadro");
```

---

## 8. TaskNow.XUnitTest

```
TaskNow.XUnitTest/
└── BLL/
    ├── QuadroBLLTests.cs
    ├── ListaBLLTests.cs
    ├── CartaoBLLTests.cs          # cobre principalmente MoverAsync (reordenação)
    └── MembroQuadroBLLTests.cs    # cobre permissão (quem pode ver/editar o quê)
```

---

## 9. Modelagem das Entidades-Chave

### Quadro

```csharp
public class Quadro
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string? Descricao { get; set; }
    public string DonoId { get; set; }          // ApplicationUser.Id
    public DateTime CriadoEm { get; set; }

    public List<Lista> Listas { get; set; }
    public List<MembroQuadro> Membros { get; set; }
}
```

### Lista

```csharp
public class Lista
{
    public int Id { get; set; }
    public int QuadroId { get; set; }
    public string Nome { get; set; }
    public int Ordem { get; set; }              // posição da coluna no quadro

    public List<Cartao> Cartoes { get; set; }
}
```

### Cartao

```csharp
public class Cartao
{
    public int Id { get; set; }
    public int ListaId { get; set; }
    public string Titulo { get; set; }
    public string? Descricao { get; set; }
    public int Ordem { get; set; }              // posição do cartão dentro da lista
    public DateTime? Prazo { get; set; }
    public string? ResponsavelId { get; set; }  // ApplicationUser.Id, nullable

    public List<CartaoEtiqueta> Etiquetas { get; set; }
    public List<Comentario> Comentarios { get; set; }
}
```

### Etiqueta

```csharp
public class Etiqueta
{
    public int Id { get; set; }
    public int QuadroId { get; set; }
    public string Nome { get; set; }
    public string Cor { get; set; }             // hex, ex.: "#E5739B"
}
```

### Comentario

```csharp
public class Comentario
{
    public int Id { get; set; }
    public int CartaoId { get; set; }
    public string AutorId { get; set; }
    public string Texto { get; set; }
    public DateTime CriadoEm { get; set; }
}
```

### MembroQuadro

```csharp
public class MembroQuadro
{
    public int Id { get; set; }
    public int QuadroId { get; set; }
    public string UsuarioId { get; set; }
    public PapelMembro Papel { get; set; }      // enum: Dono, Membro
}
```

---

## 10. Ordem de Implementação Sugerida (trilha de aprendizado)

Esta tabela **é o plano de tarefas** — cada fase vira um lote de tarefas pra distribuir. Uma pessoa não precisa terminar uma fase inteira sozinha; dá pra dividir por feature dentro da mesma fase.

| Fase | Escopo | Nível |
|---|---|---|
| **0 — Base** | Solution, projetos, DbContext, Identity, JWT, BaseDAL, RetornoDTO, PagedResultDTO | Setup (fazer junto, mostrando o porquê de cada peça) |
| **1 — Quadros e Listas** | CRUD de Quadro e Lista, sem drag-and-drop ainda (só criar/editar/excluir/listar) | Iniciante |
| **2 — Autenticação** | Registro, login (JWT), quadro passa a pertencer a um usuário | Iniciante |
| **3 — Cartões + Drag-and-drop** | CRUD de Cartão + `CartaoBLL.MoverAsync` (mudar de lista/posição) | Intermediário |
| **4 — Comentários e Etiquetas** | CRUD de Comentário e Etiqueta, associar etiqueta a cartão | Intermediário |
| **5 — Filtros e busca** | Filtrar cartões por etiqueta/responsável/prazo; buscar por texto | Intermediário |
| **6 — Tempo real (SignalR)** | `QuadroHub`: notificar outros membros quando um cartão é criado/movido/editado | Avançado |
| **7 — Permissões** | `MembroQuadroBLL`: convidar/remover membro, checar papel (Dono só pode excluir quadro, etc.) | Avançado |
| **8 — Deploy** | Docker, variáveis de ambiente, CI básico (build + test no PR) | Avançado |

---

## 11. Pacotes NuGet Principais

| Pacote | Projeto | Uso |
|---|---|---|
| `Microsoft.AspNetCore.Identity.EntityFrameworkCore` | DAO | Identity + EF |
| `Npgsql.EntityFrameworkCore.PostgreSQL` | DAO | Postgres (ou `Microsoft.EntityFrameworkCore.Sqlite` pra rodar 100% local sem instalar nada) |
| `AutoMapper.Extensions.Microsoft.DependencyInjection` | BLL/DAL | Mapeamento |
| `Microsoft.AspNetCore.Authentication.JwtBearer` | API | JWT |
| `Microsoft.AspNetCore.SignalR` | API | Tempo real (já vem no SDK do ASP.NET Core) |
| `xunit` + `Moq` | XUnitTest | Testes |

> Sugestão pra ambiente de aprendizado: comece com **SQLite** (zero setup de banco, arquivo local) e migre pra Postgres na Fase 8 (deploy) — assim ninguém trava a primeira semana instalando banco.

---

## 12. Convenções do Projeto

- **Nomes em português** para métodos de negócio e entidades de domínio.
- **Async/await** em toda a cadeia.
- **RetornoDTO.Fail(string)** para todos os erros de negócio — nunca deixar exceção crua estourar pro cliente.
- **`[Authorize]`** em todo controller, exceto `AuthController` (registro/login).
- **Migrations** versionadas por feature: `dotnet ef migrations add Feat_Cartao`.
- **Ordem (`Ordem`)** em Lista e Cartao é sempre recalculada no backend, nunca confiada ao valor que o front manda cru (o front manda "mover cartão X pra posição Y da lista Z"; quem decide o número final de `Ordem` é a BLL).
- **Toda checagem de permissão** (é dono? é membro?) mora na BLL, nunca no Controller e nunca confiada só ao front.

---

## 13. Pacote inicial de tarefas (Fase 0 e 1) — pronto pra distribuir

Sugestão de primeiras tarefas, uma por pessoa, pra já sair distribuindo:

1. **Setup da solution + Fase 0** — pode ser feito junto com você guiando (não delega ainda; é onde eles aprendem a arquitetura).
2. **Dev A**: CRUD de `Quadro` completo (DAO→DTO→DAL→BLL→API→Teste), seguindo a seção 14.6.
3. **Dev B**: CRUD de `Lista` completo, incluindo `ObterPorQuadroOrdenadoAsync` — depende do `Quadro` existir, então pode começar assim que o Dev A tiver o DAO/DTO de `Quadro` pronto (nem precisa esperar a API).

---

## 14. Guia para Devs Júnior e Estagiários

Este projeto existe pra ensinar, então esta seção não é só um apêndice — é o roteiro principal. Leia antes de pegar a primeira tarefa.

### 14.1 Antes de começar (setup local)

1. Clonar o repo, `dotnet restore` na solution.
2. Se estiver usando SQLite (recomendado pra começar), não precisa instalar banco — só rodar as migrations: `dotnet ef database update --project TaskNow.DAO --startup-project TaskNow.API`.
3. Subir a API: `dotnet run --project TaskNow.API` e abrir o Swagger (`/swagger`) — brinque com os endpoints existentes antes de escrever qualquer linha.
4. Rodar os testes: `dotnet test` — precisam passar 100% antes de você tocar no código.
5. Ler as seções 1 a 9 (visão geral, camadas, entidades) antes de pegar a primeira tarefa.

### 14.2 Regras de ouro (não fazer sem perguntar antes)

- Não alterar `TaskNowDbContext` ou gerar uma Migration sem mostrar pro mentor antes — mudança de schema afeta o banco de todo mundo.
- Não colocar lógica de negócio dentro do Controller (ex.: calcular a nova `Ordem` de um cartão) — isso é trabalho da BLL.
- Não confiar em dado que o front manda sem validar de novo no backend (ex.: `QuadroId` de um cartão — sempre confira se o usuário logado realmente tem acesso àquele quadro).
- Não commitar segredo (connection string, chave JWT) — sempre via `appsettings.Development.json` (gitignored) ou variável de ambiente.
- Regra incerta? Pergunte antes de assumir — errar rápido e perguntar é mais barato que retrabalho depois do PR.

### 14.3 Fluxo Git

- Uma branch por tarefa: `feature/nome-curto-da-tarefa` (ex.: `feature/crud-quadro`).
- Commits pequenos e descritivos, em português, no imperativo: `adiciona endpoint de criação de quadro`.
- PR sempre revisado pelo mentor antes do merge — é aqui que o aprendizado mais acontece, então espere comentários e não leve pra o lado pessoal.
- Nunca `git push --force` na branch principal.

### 14.4 Checklist de Pull Request

- [ ] Segue a arquitetura em camadas (API → BLL → DAL → DAO) — nenhuma lógica de negócio no Controller.
- [ ] Toda operação de I/O é `async`/`await`.
- [ ] Retorno da API usa `RetornoDTO<T>`.
- [ ] Erros de regra de negócio usam `RetornoDTO.Fail("mensagem clara")`.
- [ ] Endpoint novo protegido por `[Authorize]` e checa se o usuário tem permissão sobre o recurso (é dono/membro do quadro?).
- [ ] Migration gerada e testada localmente.
- [ ] Teste unitário novo/atualizado cobrindo a regra de negócio.
- [ ] `dotnet build` e `dotnet test` passam sem warning novo.
- [ ] Sem `Console.WriteLine`, código comentado ou `TODO` esquecido.

### 14.5 Erros comuns a evitar

| Erro | Por que evitar | Alternativa correta |
|---|---|---|
| Chamar o `DbContext` direto no Controller | Quebra a separação de camadas | Sempre passar pelo DAL |
| Confiar no `Ordem` que o front manda sem recalcular | Duas pessoas movendo cartões ao mesmo tempo geram posição duplicada | BLL recalcula a ordem final dos cartões afetados |
| Esquecer de checar se o usuário é membro do quadro antes de deixar editar | Qualquer usuário logado poderia editar quadro de outra pessoa | Checar `MembroQuadroDAL.EhMembroAsync` na BLL antes de qualquer escrita |
| Fazer N+1 query (loop chamando o banco) | Lento conforme o quadro cresce | Usar `Include`/`ThenInclude` ou uma query única |
| `try/catch` vazio | Esconde o erro real | Retornar `RetornoDTO.Fail` com mensagem útil |

### 14.6 Receita: como adicionar uma entidade/feature nova do zero

Checklist genérico pra qualquer tarefa tipo "CRUD de X". Exemplo com `Etiqueta`:

1. **DAO** — `TaskNow.DAO/Entities/Etiqueta.cs` → registrar `DbSet<Etiqueta>` em `TaskNowDbContext` → `dotnet ef migrations add Feat_Etiqueta` → `dotnet ef database update`.
2. **DTO** — `TaskNow.DTO/Entities/EtiquetaDTO.cs` + `Requests/Etiqueta/EtiquetaCriarRequestDTO.cs`.
3. **DAL** — `IEtiquetaDAL.cs` + `EtiquetaDAL.cs` (herda CRUD do `BaseDAL`).
4. **BLL** — `IEtiquetaBLL.cs` + `EtiquetaBLL.cs` (valida, ex.: nome não vazio, cor em formato hex válido).
5. **API** — `EtiquetasController.cs` com os endpoints REST, sempre retornando `RetornoDTO<T>`.
6. **Mapper** — `Etiqueta ↔ EtiquetaDTO` em `MappingProfile.cs`.
7. **DI** — registrar `IEtiquetaDAL`/`EtiquetaDAL` e `IEtiquetaBLL`/`EtiquetaBLL` no `Program.cs`.
8. **Teste** — `EtiquetaBLLTests.cs` cobrindo criação válida/inválida, edição, exclusão.
9. Testar manualmente no Swagger antes de abrir o PR.

### 14.7 Como pedir ajuda

Travou por mais de ~30 minutos sem sair do lugar? Pare e chame. Ao pedir ajuda, mande: o que está tentando fazer, o que já tentou, e o erro exato (texto completo, não "não funciona").

### 14.8 Trilha de tarefas sugerida por nível

| Nível | Tipo de tarefa |
|---|---|
| Iniciante | CRUD simples sem relacionamento complexo (Fase 1: Quadro, Lista) |
| Intermediário | Features com regra de negócio e relacionamento (Fase 3: mover cartão; Fase 4: etiquetas/comentários; Fase 5: filtros) |
| Avançado | Tempo real (SignalR), permissões, deploy (Fases 6 a 8) |

A ideia é que cada um passe pelas fases na ordem — a Fase 3 (drag-and-drop) só faz sentido depois de entender bem o CRUD básico da Fase 1, e a Fase 6 (tempo real) só depois de já ter mexido em BLL/API o suficiente pra entender o fluxo síncrono primeiro.
