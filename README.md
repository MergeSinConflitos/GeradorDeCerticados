# Gerador de Certificados Online

O **Gerador de Certificados Online** é uma API desenvolvida para automatizar a criação e o envio de certificados de cursos.

A aplicação permite cadastrar usuários, criar cursos, solicitar a geração de certificados para múltiplos alunos e acompanhar o processamento. Para cada aluno, é gerado um certificado individual em formato PDF e, ao final do processamento, os documentos são agrupados em um arquivo ZIP para download.

## Funcionalidades

* Cadastro e autenticação de usuários;
* Autenticação baseada em JWT;
* Proteção de rotas com `Authorization: Bearer {token}`;
* Cadastro e consulta de cursos;
* Solicitação de geração de certificados em lote;
* Geração de certificados individuais em PDF;
* Registro do status de geração de cada certificado;
* Acompanhamento do processamento do lote;
* Geração de arquivo ZIP contendo os certificados;
* Download do arquivo ZIP após a conclusão do processamento.

---

## Módulo de Usuários e Autenticação

### Requisitos funcionais

* Permitir o cadastro de usuários utilizando email e senha;
* Permitir autenticação utilizando email e senha;
* Emitir um token de acesso JWT após uma autenticação válida;
* Validar o token JWT nas rotas protegidas.

### Regras de negócio

* O email deve ser válido e único;
* A senha deve possuir no mínimo 8 caracteres;
* A senha deve conter pelo menos um dígito;
* A senha deve conter pelo menos um caractere não alfanumérico;
* As credenciais não devem ser armazenadas diretamente no código-fonte;
* A chave utilizada para assinatura do JWT não deve ser armazenada no código-fonte;
* As rotas de cadastro e login são públicas;
* As demais rotas exigem autenticação.

### Endpoints

| Método | Rota             | Acesso  | Descrição                          | Sucesso       |
| ------ | ---------------- | ------- | ---------------------------------- | ------------- |
| `POST` | `/auth/cadastro` | Público | Cadastra um usuário                | `201 Created` |
| `POST` | `/auth/login`    | Público | Autentica o usuário e emite um JWT | `200 OK`      |

### Autorização

As rotas protegidas devem receber o token no cabeçalho HTTP:

```http
Authorization: Bearer {token}
```

---

# Módulo de Cursos

## Requisitos funcionais

* Permitir cadastrar cursos;
* Permitir consultar um curso pelo seu identificador;
* Permitir associar uma solicitação de geração de certificados a um curso existente.

## Regras de negócio

Um curso deve possuir:

* Nome obrigatório, com no máximo 200 caracteres;
* Descrição opcional, com no máximo 500 caracteres;
* Carga horária inteira e maior que zero;
* Data de conclusão obrigatória.

Um curso inexistente não pode receber uma solicitação de geração de certificados.

## Endpoints

| Método | Rota                | Acesso      | Descrição                            | Sucesso       |
| ------ | ------------------- | ----------- | ------------------------------------ | ------------- |
| `POST` | `/cursos`           | Autenticado | Cadastra um curso                    | `201 Created` |
| `GET`  | `/cursos/{cursoId}` | Autenticado | Consulta um curso pelo identificador | `200 OK`      |

---

# Módulo de Geração de Certificados

## Requisitos funcionais

O sistema deve permitir solicitar a geração de certificados informando um ou mais alunos.

Para cada aluno informado:

1. Deve ser persistido um certificado;
2. Deve ser gerado um arquivo PDF individual;
3. O PDF deve conter:

   * Nome do aluno;
   * Nome do curso;
   * Carga horária;
   * Data de conclusão;
4. Deve ser registrado o caminho do arquivo gerado;
5. Deve ser registrada a data de geração;
6. O status individual deve ser atualizado para `Gerado` ou `Falha`.

Após o processamento dos certificados, os arquivos PDF devem ser agrupados em um arquivo `.zip`.

## Regras de negócio

* A solicitação deve possuir pelo menos um aluno;
* O nome de cada aluno é obrigatório;
* O nome do aluno deve possuir no máximo 200 caracteres;
* O curso informado deve existir;
* Um curso que possua um processamento de certificados ainda não finalizado não pode receber uma nova solicitação;
* Caso já exista um processamento em andamento para o curso, a API deve retornar `409 Conflict`.

## Status do processamento

O processamento geral de uma solicitação pode possuir os seguintes estados:

| Status                | Descrição                                               |
| --------------------- | ------------------------------------------------------- |
| `Pendente`            | Solicitação criada e aguardando processamento           |
| `GerandoCertificados` | Os certificados individuais estão sendo gerados         |
| `GerandoZip`          | Os PDFs foram gerados e estão sendo agrupados em um ZIP |
| `Concluido`           | Processamento finalizado com sucesso                    |
| `Falha`               | O processamento não pôde ser concluído                  |

### Status individual do certificado

Cada certificado também possui seu próprio status:

| Status     | Descrição                         |
| ---------- | --------------------------------- |
| `Pendente` | Certificado aguardando geração    |
| `Gerado`   | PDF gerado com sucesso            |
| `Falha`    | Ocorreu um erro durante a geração |

---

# Endpoints de Certificados

| Método | Rota                                      | Acesso      | Descrição                               | Sucesso        |
| ------ | ----------------------------------------- | ----------- | --------------------------------------- | -------------- |
| `POST` | `/cursos/{cursoId}/certificados`          | Autenticado | Solicita a geração dos certificados     | `202 Accepted` |
| `GET`  | `/cursos/{cursoId}/status`                | Autenticado | Consulta o status do processamento      | `200 OK`       |
| `GET`  | `/cursos/{cursoId}/certificados`          | Autenticado | Lista os certificados do curso          | `200 OK`       |
| `GET`  | `/cursos/{cursoId}/certificados/download` | Autenticado | Baixa o arquivo ZIP com os certificados | `200 OK`       |

O endpoint de download deve retornar:

```http
Content-Type: application/zip
```

---

# Fluxo de Geração

O processo de geração segue o fluxo:

```text
Solicitação
    │
    ▼
Validar curso
    │
    ├── Curso inexistente ──► 404 Not Found
    │
    ▼
Validar alunos
    │
    ├── Lista vazia ────────► 400 Bad Request
    │
    ▼
Verificar processamento
    │
    ├── Em andamento ───────► 409 Conflict
    │
    ▼
Criar certificados
    │
    ▼
Status: Pendente
    │
    ▼
Status: GerandoCertificados
    │
    ▼
Gerar PDFs individuais
    │
    ├── Sucesso ────────────► Gerado
    │
    └── Erro ───────────────► Falha
    │
    ▼
Status: GerandoZip
    │
    ▼
Agrupar PDFs
    │
    ▼
Gerar arquivo ZIP
    │
    ▼
Status: Concluido
```

Caso ocorra uma falha que impeça a conclusão do processamento, o status geral deve ser atualizado para `Falha`.

---

# Exemplo de Solicitação

### Cadastro de usuário

`POST /auth/cadastro`

```json
{
  "email": "usuario@example.com",
  "senha": "senha@123"
}
```

### Cadastro de curso

`POST /cursos`

```json
{
  "nome": "Curso de Desenvolvimento Web",
  "descricao": "Curso introdutório de desenvolvimento de aplicações web.",
  "cargaHoraria": 40,
  "dataConclusao": "2026-09-15"
}
```

### Solicitação de certificados

`POST /cursos/{cursoId}/certificados`

```json
{
  "alunos": [
    {
      "nome": "João da Silva"
    },
    {
      "nome": "Maria da Silva"
    },
    {
      "nome": "Carlos Oliveira"
    }
  ]
}
```

A solicitação deve retornar:

```http
202 Accepted
```

indicando que o processamento foi aceito para execução.

---

# Exemplo de Consulta de Status

`GET /cursos/{cursoId}/status`

```json
{
  "status": "GerandoCertificados"
}
```

Após a conclusão:

```json
{
  "status": "Concluido"
}
```

---

# Exemplo de Certificado

Cada certificado gerado deve conter, no mínimo:

```text
CERTIFICADO

Certificamos que

João da Silva

concluiu o curso

Curso de Desenvolvimento Web

com carga horária de 40 horas.

Data de conclusão: 15/09/2026
```

O documento deve ser gerado individualmente para cada aluno.

---

# Arquitetura

A aplicação pode ser organizada seguindo uma arquitetura em camadas, separando responsabilidades entre domínio, aplicação, infraestrutura e API.

| Camada           | Responsabilidade                                                |
| ---------------- | --------------------------------------------------------------- |
| `Dominio`        | Entidades, regras de negócio e contratos                        |
| `Aplicacao`      | Casos de uso, comandos, consultas e serviços de aplicação       |
| `Infraestrutura` | Persistência, Identity, geração de arquivos e serviços externos |
| `Api`            | Controllers, autenticação, configuração HTTP e apresentação     |

Uma possível estrutura de solução:

```text
GeradorDeCertificados/
│
├── src/
│   ├── Dominio/
│   │   └── GeradorDeCertificados.Dominio.csproj
│   │
│   ├── Aplicacao/
│   │   └── GeradorDeCertificados.Aplicacao.csproj
│   │
│   ├── Infraestrutura/
│   │   └── GeradorDeCertificados.Infraestrutura.csproj
│   │
│   └── Api/
│       └── GeradorDeCertificados.Api.csproj
│
└── README.md
```

---

# Persistência

O sistema deve persistir as informações necessárias para controlar os cursos, solicitações e certificados.

Uma possível modelagem inclui:

### Usuário

Responsável pela autenticação e identificação do usuário que utiliza a API.

### Curso

Armazena as informações do curso:

* Identificador;
* Nome;
* Descrição;
* Carga horária;
* Data de conclusão.

### Solicitação de Certificados

Representa um lote de geração:

* Identificador;
* Curso;
* Status do processamento;
* Data da solicitação;
* Data de conclusão;
* Caminho do arquivo ZIP, quando disponível.

### Certificado

Representa o certificado individual de um aluno:

* Identificador;
* Solicitação;
* Curso;
* Nome do aluno;
* Status;
* Caminho do PDF;
* Data de geração.

A separação entre **solicitação** e **certificado** permite acompanhar individualmente o resultado da geração de cada aluno.

---

# Geração dos PDFs

Os certificados devem ser gerados individualmente utilizando uma biblioteca de geração de PDF, como o **QuestPDF** ou outra biblioteca equivalente.

Cada arquivo deve conter as informações do aluno e do curso relacionadas à solicitação.

Exemplo de nomenclatura:

```text
Joao_da_Silva.pdf
Maria_da_Silva.pdf
Carlos_Oliveira.pdf
```

O sistema deve evitar que caracteres inválidos para nomes de arquivos causem problemas durante a geração.

---

# Geração do ZIP

Depois que os certificados forem processados, os PDFs gerados devem ser agrupados em um único arquivo `.zip`.

Exemplo:

```text
certificados-curso-01900000.zip
│
├── Joao_da_Silva.pdf
├── Maria_da_Silva.pdf
└── Carlos_Oliveira.pdf
```

O caminho do arquivo ZIP deve ser armazenado para permitir seu download posteriormente.

---

# Autenticação e Segurança

A API utiliza autenticação baseada em **JWT Bearer**.

O fluxo de autenticação é:

```text
Usuário
   │
   ▼
POST /auth/login
   │
   ▼
Validar email e senha
   │
   ├── Inválido ───────► Erro de autenticação
   │
   ▼
Gerar JWT
   │
   ▼
Retornar token
   │
   ▼
Cliente envia:
Authorization: Bearer {token}
   │
   ▼
API valida o JWT
   │
   ▼
Acesso à rota protegida
```

A chave utilizada para assinatura do JWT deve ser fornecida através de configuração segura, como variáveis de ambiente ou Secret Manager, e nunca deve ser armazenada diretamente no código-fonte.

---

# Tratamento de Erros

A API deve utilizar respostas HTTP adequadas para representar erros de validação e regras de negócio.

Exemplos:

| Código                      | Situação                                   |
| --------------------------- | ------------------------------------------ |
| `400 Bad Request`           | Dados de entrada inválidos                 |
| `401 Unauthorized`          | Usuário não autenticado ou token inválido  |
| `404 Not Found`             | Curso ou recurso inexistente               |
| `409 Conflict`              | Curso já possui processamento em andamento |
| `500 Internal Server Error` | Erro inesperado durante o processamento    |

---

# Requisitos Técnicos

O projeto deve utilizar tecnologias adequadas para uma API REST moderna em .NET.

Sugestão de stack:

* .NET 10;
* ASP.NET Core Web API;
* ASP.NET Core Identity;
* JWT Bearer Authentication;
* Entity Framework Core;
* PostgreSQL;
* QuestPDF ou biblioteca equivalente para geração de PDF;
* Sistema de arquivos para armazenamento dos PDFs e ZIPs;
* Swagger/OpenAPI.

---

# Pré-requisitos

Para executar o projeto localmente, é necessário possuir:

* [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0);
* PostgreSQL;
* Banco de dados configurado;
* Chave segura para assinatura do JWT.

---

# Configuração

As informações sensíveis da aplicação não devem ser armazenadas diretamente no código-fonte.

Exemplos de configurações que devem permanecer fora do repositório:

```text
ConnectionStrings:Postgres
Jwt:Key
Jwt:Issuer
Jwt:Audience
```

Durante o desenvolvimento, essas informações podem ser configuradas utilizando o **Secret Manager**, variáveis de ambiente ou outro mecanismo apropriado.

---

# Execução

Na raiz da solução:

```bash
dotnet restore
```

Para compilar:

```bash
dotnet build
```

Para executar a API:

```bash
dotnet run --project src/Api
```

Caso o projeto utilize migrations do Entity Framework Core:

```bash
dotnet ef database update
```

---

# Fluxo completo da aplicação

```text
┌──────────────────────┐
│      Usuário         │
└──────────┬───────────┘
           │
           ▼
┌──────────────────────┐
│   Cadastro / Login   │
│      /auth/*         │
└──────────┬───────────┘
           │
           ▼
       JWT Token
           │
           ▼
┌──────────────────────┐
│      Cursos          │
│ POST /cursos         │
│ GET  /cursos/{id}    │
└──────────┬───────────┘
           │
           ▼
┌────────────────────────────┐
│ Solicitar Certificados     │
│ POST /cursos/{id}/          │
│ certificados                │
└────────────┬───────────────┘
             │
             ▼
┌────────────────────────────┐
│ Gerar certificados PDF     │
│ individualmente por aluno  │
└────────────┬───────────────┘
             │
             ▼
┌────────────────────────────┐
│ Gerar arquivo ZIP          │
└────────────┬───────────────┘
             │
             ▼
┌────────────────────────────┐
│ Download                   │
│ /certificados/download     │
└────────────────────────────┘
```

---

# Status do Projeto

Projeto em desenvolvimento.

Funcionalidades previstas:

* [x] Cadastro de usuários
* [x] Autenticação JWT
* [x] Proteção de endpoints
* [x] Cadastro de cursos
* [x] Consulta de cursos
* [x] Solicitação de certificados
* [x] Geração de PDFs individuais
* [x] Controle de status dos certificados
* [x] Geração de arquivo ZIP
* [x] Consulta do status do processamento
* [x] Download dos certificados

---

# Objetivo

O projeto tem como objetivo aplicar conceitos de desenvolvimento de APIs REST utilizando **.NET**, explorando autenticação, persistência de dados, processamento de arquivos, geração de documentos e processamento de operações em lote.

A aplicação também serve como projeto prático para estudo de arquitetura em camadas, regras de negócio, Entity Framework Core, JWT e integração entre diferentes componentes de uma aplicação backend.
