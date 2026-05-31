# Vertrau API

> API REST de cadastro de usuários desenvolvida como desafio técnico de estágio.

![CI/CD](https://github.com/EdilsonRogerioCuambe/vertrau-backend/actions/workflows/ci.yml/badge.svg)
![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-4169E1?logo=postgresql)
![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?logo=docker)

---

## 🛠️ Stack Tecnológica

| Camada | Tecnologia |
|---|---|
| Framework | ASP.NET Core 8 |
| ORM | Entity Framework Core 8 |
| Banco de Dados | PostgreSQL 16 |
| Validação | FluentValidation 11 |
| Mapeamento | AutoMapper 16 |
| Documentação | Swagger / Swashbuckle |
| Testes | xUnit + FluentAssertions + Moq |
| Containers | Docker + Docker Compose |
| CI/CD | GitHub Actions |

---

## 🏗️ Arquitetura

O projeto segue uma **arquitetura em camadas** limpa e desacoplada:

```
VertrauApi/
├── Controllers/        # Camada de apresentação — endpoints HTTP
├── Services/           # Camada de negócio — regras e validações
├── Repositories/       # Camada de dados — acesso ao banco
├── Models/             # Entidades e DTOs
├── Mappings/           # Perfis AutoMapper
├── Middleware/         # Tratamento global de erros
├── Validations/        # Validadores FluentValidation
├── Enums/              # Enumerações
├── Exceptions/         # Exceções customizadas
└── Data/               # DbContext e Migrations
```

---

## 🚀 Como Executar

### Pré-requisitos
- [Docker](https://www.docker.com/) e Docker Compose
- **ou** .NET 8 SDK + PostgreSQL

### ▶️ Com Docker (recomendado)

```bash
# Clonar o repositório
git clone https://github.com/EdilsonRogerioCuambe/vertrau-backend.git
cd vertrau-backend

# Subir a API e o banco
docker compose up --build
```

A API estará disponível em `http://localhost:8080`.  
O Swagger UI estará em `http://localhost:8080/swagger`.

### ▶️ Sem Docker

```bash
# Configurar a connection string no appsettings.json ou via variável de ambiente
export DATABASE_URL="Host=localhost;Port=5432;Database=vertraudp;Username=seu_usuario;Password=sua_senha"

# Restaurar dependências e rodar
dotnet restore
dotnet run --project VertrauApi
```

---

## 📄 Endpoints

Base URL: `/api/v1`

| Método | Endpoint | Descrição |
|---|---|---|
| `GET` | `/usuarios` | Lista usuários com filtros e paginação |
| `GET` | `/usuarios/{id}` | Retorna um usuário pelo ID |
| `POST` | `/usuarios` | Cria um novo usuário |
| `PUT` | `/usuarios/{id}` | Atualiza um usuário |
| `DELETE` | `/usuarios/{id}` | Remove um usuário |

### 🔍 Query Parameters — `GET /usuarios`

| Parâmetro | Tipo | Padrão | Descrição |
|---|---|---|---|
| `page` | `int` | `1` | Número da página |
| `pageSize` | `int` | `10` | Tamanho da página (máx. 100) |
| `nome` | `string` | — | Filtro por nome (parcial, case-insensitive) |
| `genero` | `string` | — | Filtro por gênero (`Masculino`, `Feminino`, `Outro`) |

### 📥 Corpo da Requisição — `POST /usuarios`

```json
{
  "nome": "João",
  "sobrenome": "Silva",
  "email": "joao.silva@email.com",
  "genero": "Masculino",
  "dataNascimento": "1990-01-15"
}
```

### 📤 Resposta de Sucesso — `201 Created`

```json
{
  "id": 1,
  "nome": "João",
  "sobrenome": "Silva",
  "email": "joao.silva@email.com",
  "genero": "Masculino",
  "dataNascimento": "1990-01-15T00:00:00",
  "criadoEm": "2024-01-01T12:00:00Z"
}
```

### 📦 Resposta de Listagem — `200 OK`

```json
{
  "page": 1,
  "pageSize": 10,
  "totalCount": 42,
  "totalPages": 5,
  "items": [ ... ]
}
```

### ⚠️ Respostas de Erro

| Código | Situação |
|---|---|
| `400 Bad Request` | Dados inválidos (validação) |
| `404 Not Found` | Usuário não encontrado |
| `409 Conflict` | E-mail já cadastrado |
| `500 Internal Server Error` | Erro interno do servidor |

---

## 🧪 Testes

```bash
dotnet test
```

O projeto contém **14 testes** no total:

- **7 testes unitários** (`UsuarioServiceTests`) — validam a lógica de negócio com mocks.
- **7 testes de integração** (`UsuariosControllerIntegrationTests`) — testam os endpoints HTTP com banco SQLite in-memory.

---

## 🔄 CI/CD — GitHub Actions

O pipeline `.github/workflows/ci.yml` executa automaticamente em cada `push` e `pull_request`:

1. **Build** — Compila o projeto em modo `Release`.
2. **Testes** — Executa todos os testes e publica resultados.
3. **Docker Build & Push** *(apenas na branch `main`)* — Builda e publica a imagem no GitHub Container Registry (`ghcr.io`).

---

## 🐳 Variáveis de Ambiente

| Variável | Descrição | Padrão |
|---|---|---|
| `DATABASE_URL` | Connection string do PostgreSQL | *(appsettings.json)* |
| `ASPNETCORE_URLS` | URL de escuta da aplicação | `http://+:8080` |

---

## ✅ Convenções de Commit

Este projeto utiliza [Conventional Commits](https://www.conventionalcommits.org/):

```
feat: adiciona endpoint de listagem de usuários
fix: corrige validação de data de nascimento
docs: atualiza README com exemplos de uso
test: adiciona testes de integração do controller
chore: configura GitHub Actions
```

---

## 👤 Autor

**Edilson Rogério Cuambe**  
[GitHub](https://github.com/EdilsonRogerioCuambe)
