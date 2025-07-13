# 📂 FileUploaderDocspider

Teste técnico para a vaga de desenvolvedor na **Docspider**.

A aplicação desenvolvida tem como objetivo realizar o **upload de documentos** com funcionalidades de estilo CRUD e listar as tecnologias utilizadas no seu desenvolvimento.

### ✅ Regras de Negócio

- ❌ Não deve permitir cadastrar mais de um documento com o mesmo título  
- 🔠 O título deve possuir no máximo **100 caracteres**  
- 📝 A descrição deve possuir no máximo **2000 caracteres**  
- 📁 Não deve permitir o upload de arquivos com extensões: `.exe`, `.zip`, `.bat`

---

### 🧰 Tech Stack

| Tecnologia     | Ícone |
|----------------|-------|
| .NET Core      | ![.NET](https://cdn.simpleicons.org/dotnet) |
| PostgreSQL     | ![PostgreSQL](https://cdn.simpleicons.org/postgresql) |
| Bootstrap      | ![Bootstrap](https://cdn.simpleicons.org/bootstrap) |
| JavaScript     | ![JavaScript](https://cdn.simpleicons.org/javascript) |
| jQuery         | ![jQuery](https://cdn.simpleicons.org/jquery) |

---

### 📸 Project Preview

**Início**  
![Início](https://github.com/user-attachments/assets/595d3c3c-8f80-4160-ab9b-bdea338ab3d2)

**Sobre**  
![Sobre](https://github.com/user-attachments/assets/a019fd5a-ee3a-4132-b77b-408392a8292c)

**Meus Documentos**  
![Meus Documentos](https://github.com/user-attachments/assets/ebddca8a-43db-459b-b895-9ea60d1aaa99)

**Detalhes**  
![Detalhes](https://github.com/user-attachments/assets/e2e92d66-2b9a-4e7c-9775-30c24a07b9c6)

**Editar**  
![Editar](https://github.com/user-attachments/assets/69d4e45a-f0b7-46fb-9cfd-253c15e579c1)

**Deletar**  
![Deletar](https://github.com/user-attachments/assets/9b337a4c-d1b9-4ded-89bd-2ce32c9b7dd0)

---

### 🧩 Dependências

#### **Web Layer** (`FileUploaderDocspider.Web`)
- `Microsoft.EntityFrameworkCore` — ORM do Entity Framework Core  
- `Microsoft.EntityFrameworkCore.Design` — Ferramentas de design-time para EF Core  
- `Microsoft.EntityFrameworkCore.Tools` — Ferramentas adicionais para EF Core  
- `Npgsql.EntityFrameworkCore.PostgreSQL` — Provider PostgreSQL para EF Core  

---

#### **Application Layer** (`FileUploaderDocspider.Application`)
- [`NetDevPack.SimpleMediator`](https://www.nuget.org/packages/NetDevPack.SimpleMediator) — Implementação simples do padrão Mediator para .NET

---

#### **Infrastructure Layer** (`FileUploaderDocspider.Infrastructure`)
- `Microsoft.EntityFrameworkCore`  
- `Microsoft.EntityFrameworkCore.Design`  
- `Npgsql.EntityFrameworkCore.PostgreSQL`  
- `Microsoft.AspNetCore.App` — Referência do framework ASP.NET Core

---

#### **Testes Unitários**  
(`FileUploaderDocspider.Application.UnitTests` e `FileUploaderDocspider.Web.UnitTests`)

- [`coverlet.collector`](https://www.nuget.org/packages/coverlet.collector) — Ferramenta de cobertura de código para .NET  
- [`Microsoft.NET.Test.Sdk`](https://www.nuget.org/packages/Microsoft.NET.Test.Sdk) — SDK de testes para .NET  
- [`Moq`](https://www.nuget.org/packages/Moq) — Biblioteca de mocks para .NET  
- [`xunit`](https://www.nuget.org/packages/xunit) — Framework de testes  
- [`xunit.runner.visualstudio`](https://www.nuget.org/packages/xunit.runner.visualstudio) — Executor de testes para Visual Studio

---

### 📚 Referências

- [📄 Upload files in ASP.NET Core (Microsoft Docs)](https://learn.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-3.1)  
- [📄 ASP.NET Core MVC - Upload de arquivos (Macoratti)](https://www.macoratti.net/18/11/aspn_upload1.htm)
