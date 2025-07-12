# FileUploaderDocspider
Teste técnico para a vaga de desenvolvedor na Docspider. 
A aplicação desenvolvida tem como objetivo realizar o upload de documentos, no estilo CRUD e listar as tecnologias utilizadas para o desenvolvimento dela. Além disso, segue as seguintes regras de negócio: 
- Não deve permitir cadastrar mais de um documento com mesmo título
- O título deve possuir no máximo 100 caracteres
- A descrição deve possuir no máximo 2000 caracteres
- Não deve permitir realizar o upload de arquivos dos tipos: .exe, .zip e .bat

### 🧰 Tech Stack

<div style="display: flex; gap: 10px;">
    <img height="32" width="32" src="https://cdn.simpleicons.org/dotnet" alt=".NET" title=".NET" />
    <img height="32" width="32" src="https://cdn.simpleicons.org/postgresql" alt="PostgreSQL" title="PostgreSQL" />
    <img height="32" width="32" src="https://cdn.simpleicons.org/bootstrap" alt="Bootstrap" title="Bootstrap" />
    <img height="32" width="32" src="https://cdn.simpleicons.org/javascript" alt="JavaScript" title="JavaScript" />
    <img height="32" width="32" src="https://cdn.simpleicons.org/jquery" alt="JQuery" title="JQuery" />
</div>

### 📸 Project Preview

<div style="display: flex; gap: 20px; flex-wrap: wrap;">
  <div>
    <strong>Início</strong><br/>
    <img src="https://github.com/user-attachments/assets/595d3c3c-8f80-4160-ab9b-bdea338ab3d2" alt="" width="650"/>
  </div>
  <div>
    <strong>Meus Documentos</strong><br/>
    <img src="https://github.com/user-attachments/assets/ebddca8a-43db-459b-b895-9ea60d1aaa99" alt="" width="650"/>
  </div>
   <div>
    <strong>Detalhes</strong><br/>
    <img src="https://github.com/user-attachments/assets/e2e92d66-2b9a-4e7c-9775-30c24a07b9c6" alt="" width="650"/>
  </div>
    <div>
    <strong>Editar</strong><br/>
    <img src="https://github.com/user-attachments/assets/69d4e45a-f0b7-46fb-9cfd-253c15e579c1" alt="" width="650"/>
  </div>
   <div>
    <strong>Deletar</strong><br/>
    <img src="https://github.com/user-attachments/assets/9b337a4c-d1b9-4ded-89bd-2ce32c9b7dd0" alt="" width="650"/>
  </div>
</div>

### 🧩 Dependências

#### **Web Layer** (`FileUploaderDocspider.Web`)
- `Microsoft.EntityFrameworkCore` — Entity Framework Core ORM.
- `Microsoft.EntityFrameworkCore.Design` — Design-time tools for EF Core.
- `Microsoft.EntityFrameworkCore.Tools` — Tools for EF Core.
- `Npgsql.EntityFrameworkCore.PostgreSQL` — PostgreSQL provider for EF Core.

---

#### **Application Layer** (`FileUploaderDocspider.Application`)
- [`NetDevPack.SimpleMediator`](https://www.nuget.org/packages/NetDevPack.SimpleMediator) — Simple implementation of the Mediator pattern for .NET.

---

#### **Infrastructure Layer** (`FileUploaderDocspider.Infrastructure`)
- `Microsoft.EntityFrameworkCore` — Entity Framework Core ORM.
- `Microsoft.EntityFrameworkCore.Design` — Design-time tools for EF Core.
- `Npgsql.EntityFrameworkCore.PostgreSQL` — PostgreSQL provider for EF Core.
- `Microsoft.AspNetCore.App` (Framework Reference) — Shared ASP.NET Core framework components.

---

#### **Testes Unitários** (`FileUploaderDocspider.Application.UnitTests` e `FileUploaderDocspider.Web.UnitTests`)
- [`coverlet.collector`](https://www.nuget.org/packages/coverlet.collector) — Cross-platform code coverage for .NET.
- [`Microsoft.NET.Test.Sdk`](https://www.nuget.org/packages/Microsoft.NET.Test.Sdk) — Test SDK for .NET.
- [`Moq`](https://www.nuget.org/packages/Moq) — Mocking library for .NET.
- [`xunit`](https://www.nuget.org/packages/xunit) — Unit testing tool for .NET.
- [`xunit.runner.visualstudio`](https://www.nuget.org/packages/xunit.runner.visualstudio) — Visual Studio test runner for xUnit.net.


### 📚 Referências
[Upload files in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-3.1) \
[ASP .NET Core MVC - Upload de arquivos](https://www.macoratti.net/18/11/aspn_upload1.htm)
