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

## 🧩 Fluxo de Dados — “Submit Search”

### 1️⃣ Endpoint

`POST /api/v1/searches/submit`

### 2️⃣ Entrada esperada

```json
{
  "username": "renan.pires",
  "configVersion": "SIN_prod_2025_09",
  "terminais": ["RJTRIO_PL1", "PAXNGB_PL2"],
  "resolucao": { "agg": "100ms" }
}
```

### 3️⃣ Saída esperada

```json
{
  "requestId": "2fa0be42-7f2b-43a1-bcc3-308cf833e3c2",
  "username": "renan.pires",
  "configVersion": "SIN_prod_2025_09",
  "terminais": ["RJTRIO_PL1", "PAXNGB_PL2"],
  "mode": "agg",
  "agg": "100ms",
  "selectRate": null
}
```

### 4️⃣ Lógica

1. O **Controller** recebe a requisição e valida o modelo (`SubmitSearchRequest`).
2. A requisição é enviada ao **Handler (`SubmitSearchHandler`)** que:

   * Cria um `RequestId`;
   * Normaliza os parâmetros;
   * Gera a resposta (`SubmitSearchResponse`).
3. A resposta é retornada ao cliente com `HTTP 200`.

---

## 🧩 Camadas Principais

### **Contracts**

Define os objetos trocados entre cliente e servidor.

```csharp
public sealed class SubmitSearchRequest
{
    public required string Username { get; init; }
    public required string ConfigVersion { get; init; }
    public required IReadOnlyList<string> Terminais { get; init; }
    public required ResolutionDto Resolucao { get; init; }
}

public sealed class ResolutionDto
{
    public string? Agg { get; init; }
    public int? SelectRate { get; init; }
}
```

---

### **Application**

Contém a lógica de negócio — aqui entra o **validador** e o **handler**.

```csharp
public static class SubmitSearchValidator
{
    public static (bool ok, string? error, (string mode, string? agg, int? rate) normalized)
        Validate(SubmitSearchRequest req, int maxTerminais = 64)
    {
        if (string.IsNullOrWhiteSpace(req.Username))
            return (false, "Username não informado.", default);

        if (req.Terminais.Count > maxTerminais)
            return (false, "Máximo de 64 terminais permitido.", default);

        bool hasAgg = !string.IsNullOrWhiteSpace(req.Resolucao.Agg);
        bool hasRate = req.Resolucao.SelectRate.HasValue;

        if (hasAgg && hasRate)
            return (false, "Informe apenas Agg ou SelectRate (não ambos).", default);
        if (!hasAgg && !hasRate)
            return (false, "Informe Agg ou SelectRate.", default);

        return hasAgg
            ? (true, null, ("agg", req.Resolucao.Agg, null))
            : (true, null, ("rate", null, req.Resolucao.SelectRate));
    }
}
```

---

### **Controller**

Controlador principal que expõe o endpoint HTTP.

```csharp
[ApiController]
[Route("api/v1/[controller]")]
public class SearchesController : ControllerBase
{
    private readonly ICommandHandler<SubmitSearchCommand, SubmitSearchResponse> _handler;

    public SearchesController(ICommandHandler<SubmitSearchCommand, SubmitSearchResponse> handler)
    {
        _handler = handler;
    }

    [HttpPost("submit")]
    public async Task<IActionResult> Submit([FromBody] SubmitSearchRequest request)
    {
        var (ok, error, normalized) = SubmitSearchValidator.Validate(request);
        if (!ok)
            return BadRequest(new { error });

        var command = new SubmitSearchCommand
        {
            RequestId = Guid.NewGuid().ToString(),
            Payload = request,
            Mode = normalized.mode,
            Agg = normalized.agg,
            SelectRate = normalized.rate
        };

        var response = await _handler.Handle(command, CancellationToken.None);
        return Ok(response);
    }
}
```

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

```csharp
[Fact]
public void Should_accept_agg_100ms()
{
    var req = MakeReq(agg: "100ms", rate: null);
    var (ok, err, norm) = SubmitSearchValidator.Validate(req, maxTerminais: 64);

    ok.Should().BeTrue();
    norm.mode.Should().Be("agg");
    norm.agg.Should().Be("100ms");
}
```

---

### 🌐 `SearchesControllerTests`

Executa requisições HTTP reais contra a API usando `WebApplicationFactory<Program>`.

```csharp
[Fact]
public async Task Submit_should_return_200_with_agg()
{
    var client = _factory.CreateClient();
    var request = new SubmitSearchRequest
    {
        Username = "renan",
        ConfigVersion = "SIN_prod_2025_09",
        Terminais = new[] { "RJTRIO_PL1" },
        Resolucao = new ResolutionDto { Agg = "100ms" }
    };

    var response = await client.PostAsJsonAsync("/api/v1/searches/submit", request);
    response.StatusCode.Should().Be(HttpStatusCode.OK);
}
```

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
https://localhost:5001
```

---

## 🧱 Roadmap

| Etapa                         | Descrição                                           | Status       |
| ----------------------------- | --------------------------------------------------- | ------------ |
| 🧩 Migração da camada “Busca” | Reestruturação para API Web                         | ✅ Concluída  |
| 🔐 Integração com Keycloak    | Autenticação JWT (realm: openplot)                  | 🕓 Planejado |
| 📂 Persistência               | PostgreSQL / diretórios de saída por usuário        | 🕓 Planejado |
| 🔄 Endpoint `run-and-save`    | Execução e armazenamento das buscas                 | 🕓 Em breve  |
| 📊 Exportação de resultados   | Endpoint `/api/v1/searches/export/{id}`             | 🕓 Futuro    |
| 🔍 Front-end Web              | Interface de busca e visualização (React ou Blazor) | 🕓 Futuro    |
| 📦 Deploy                     | Docker + Nginx Reverse Proxy                        | 🕓 Futuro    |

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

## 👨‍💻 Autor

**Renan Duarte Pires**
Engenharia Elétrica — UFSC
Laboratório de Planejamento de Sistemas Elétricos (LabPlan)
Projetos: *openPlot*, *MedPlot*, *RGOOSE*, *openPDC Integration*
