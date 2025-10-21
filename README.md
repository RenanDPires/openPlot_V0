# 🧩 openPlot — Backend Web (.NET 8 MVC)

## 📘 Visão Geral

**openPlot** é a refatoração e migração do **MedPlot**, um sistema legado em C# WinForms, para uma arquitetura moderna baseada em **Web API .NET 8**, escalável, multiusuário e voltada para integração via HTTP.

O sistema tem como objetivo principal **consultar, processar e visualizar dados de sincrofasores**, utilizando o histórico de dados (não em tempo real) para análises e validações.

---

## 🧱 Arquitetura

O projeto adota uma arquitetura **limpa (Clean Architecture)** com princípios **SOLID** e separação clara entre camadas.

```plaintext
openPlot.sln
├── openPlot/                 # Projeto principal (Web API)
│   ├── Application/          # Camada de domínio e lógica de negócios
│   │   ├── Abstractions/     # Interfaces base (ex: ICommandHandler)
│   │   ├── Commands/         # Comandos (SubmitSearchCommand)
│   │   ├── Handlers/         # Implementações dos comandos
│   │   └── Validation/       # Regras de validação (SubmitSearchValidator)
│   ├── Contracts/            # DTOs e contratos de requisição/resposta
│   │   ├── Requests/
│   │   └── Responses/
│   ├── Controllers/          # Endpoints HTTP (MVC Controllers)
│   │   └── V1/SearchesController.cs
│   ├── Program.cs            # Configuração de inicialização da API
│   └── appsettings.json
│
└── openPlot.Tests/           # Projeto de testes (xUnit + FluentAssertions + Moq)
    ├── Integration/          # Testes de integração
    │   ├── CustomWebAppFactory.cs
    │   └── SearchesControllerTests.cs
    └── Validators/           # Testes unitários de validação
        └── SubmitSearchValidatorTests.cs
```

---

## ⚙️ Tecnologias Utilizadas

| Categoria               | Tecnologias                                                              |
| ----------------------- | ------------------------------------------------------------------------ |
| **Framework**           | [.NET 8](https://learn.microsoft.com/dotnet/core/whats-new/dotnet-8)     |
| **Padrão arquitetural** | MVC + Clean Architecture (Camadas Application / Contracts / Controllers) |
| **Testes**              | xUnit, FluentAssertions, Moq, Microsoft.AspNetCore.Mvc.Testing           |
| **Configuração futura** | Keycloak (autenticação), PostgreSQL (armazenamento)                      |
| **Estilo e qualidade**  | Nullable enabled, Immutability (`init`), SOLID principles                |

---


### 1️⃣ Endpoint



### 2️⃣ Entrada esperada



### 3️⃣ Saída esperada



### 4️⃣ Lógica



## 🧩 Camadas Principais

### **Contracts**

Define os objetos trocados entre cliente e servidor.



---

### **Application**

Contém a lógica de negócio — aqui entra o **validador** e o **handler**.



---

### **Controller**

Controlador principal que expõe o endpoint HTTP.



---

## 🧪 Testes Automatizados

### 📘 Estrutura

```plaintext
openPlot.Tests/
├── Integration/
│   ├── CustomWebAppFactory.cs
│   └── SearchesControllerTests.cs
└── Validators/
    └── SubmitSearchValidatorTests.cs
```

---

### ✅ `SubmitSearchValidatorTests`

Testa todas as combinações possíveis de parâmetros.



---

### 🌐 `SearchesControllerTests`

Executa requisições HTTP reais contra a API usando `WebApplicationFactory<Program>`.


---

## 🦯 Como Executar

### 🔧 Build

```bash
dotnet build
```

### 🧩 Testes

```bash
dotnet test --logger "console;verbosity=detailed"
```

### 🛠️ Rodar o servidor local

```bash
cd openPlot
dotnet run
```

A API ficará acessível em:

```
https://localhost:7271
```

---



## 🛠️ Convenções de Código

* Segue o padrão **C# 12 + .NET 8**.
* Todos os DTOs usam propriedades **`init`** (imutáveis).
* Todos os comandos seguem **CQRS** (`ICommandHandler<TCommand, TResult>`).
* Testes usam **xUnit + FluentAssertions + Moq**.
* Mock de handlers configurado via `CustomWebAppFactory`.

---

## 📄 Commit Inicial

```bash
git add .
git commit -m "feat(api): implementa endpoint POST /api/v1/searches/submit, arquitetura MVC limpa e testes completos"
```

---

## 👨‍💻 Autores

**Beatriz**
**Renan**
**Tatiana**
