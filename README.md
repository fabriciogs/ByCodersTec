# Desafio ByCodersTec

Aplicação .NET Core 8 para processar arquivos CNAB, normalizar dados, armazenar em SQL Server com Dapper, e exibir transações por loja com saldo total. Usa Clean Architecture, sem MediatR.

## Pré-requisitos
- .NET 8 SDK
- SQL Server (local ou Docker)
- Docker (opcional, para Docker Compose)

## Setup
1. Clone o repositório:
   ```bash
   git clone <seu-repositorio>
   
2. Configure a connection string em appsettings.json:json

"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=DesafioDb;Trusted_Connection=True;TrustServerCertificate=True;"
}

3. Crie o banco e a tabela:bash

sqlcmd -S localhost -U sa -P YourStrong@Passw0rd -d DesafioDb -i setup.sql

4. Restaure pacotes e rode:bash

cd DesafioByCodersTec.Presentation
dotnet restore
dotnet run

5. Acesse: http://localhost:8080 (ou porta configurada).



Com DockerRode:bash

docker-compose up --build

Acesse: http://localhost:8080.

Endpoints da APIPOST /api/transactions/upload: Faz upload do arquivo CNAB.Content-Type: multipart/form-data
Exemplo: curl -X POST -F "file=@CNAB.txt" http://localhost:8080/api/transactions/upload

GET /api/transactions/store/{storeName}: Lista transações por loja.
GET /api/transactions/store/{storeName}/balance: Retorna saldo da loja.
GET /api/transactions/stores: Lista todas as lojas.

Testes

cd DesafioByCodersTec.Tests
dotnet test

### 6. Estrutura

Domain: Entidades e interfaces.
Application: Lógica de negócio, parsing de CNAB, validações.
Infrastructure: Dapper, acesso ao SQL Server.
Presentation: API REST e interface MVC.
Tests: Testes unitários com xUnit.

### 7. Tecnologias
.NET 8 (C#)
SQL Server
Dapper
FluentValidation
xUnit, Moq, FluentAssertions
Docker Compose (opcional)

---

### 8. Como Consumir a API
- **Upload**: Use Postman ou `curl` para enviar o arquivo `CNAB.txt` para `/api/transactions/upload`.
- **Listar Transações**: `GET /api/transactions/store/BAR%20DO%20JOÃO`.
- **Saldo**: `GET /api/transactions/store/BAR%20DO%20JOÃO/balance`.
- **Lojas**: `GET /api/transactions/stores`.

---

### 9. Notas de Avaliação
- **Requisitos**: Atende upload, parsing, armazenamento, exibição e totalização.
- **Documentação**: README detalhado, API com Swagger.
- **Commits**: Estruture commits atômicos (ex.: "Add CNAB parsing", "Implement UI", "Add tests").
- **Testes**: Cobertura para parsing, validações, e consultas.
- **Extras**: Docker Compose, CSS puro, API documentada.

Se precisar de ajustes ou mais testes (ex.: para controllers), avise!