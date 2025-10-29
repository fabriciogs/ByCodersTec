# ByCoders Challenge - .NET 9 Web API with OAuth Authentication

Forked from [https://github.com/ByCodersTec/desafio.net](https://github.com/ByCodersTec/desafio.net).

A clean, testable, and production-ready solution for the **ByCoders Developer Challenge**, implementing CNAB file parsing, financial transaction storage, and store balance reporting using **.NET 9**, **Clean Architecture**, **Dapper**, and **SQL Server**.

This project meets all required criteria and includes extra points for:
- **Clean Architecture** with dependency inversion
- **Dapper** for high-performance data access
- **Unit tests with greater than 70% code coverage** (xUnit + Moq)
- **Docker Compose** support (extra points)
- **Swagger UI** with full API documentation
- **No external CSS frameworks** (API-only)

---

## Technologies & Tools

| Layer             | Technology                                      |
|------------------|--------------------------------------------------|
| **Language**     | C# 13 (.NET 9)                                   |
| **Architecture** | Clean Architecture (Domain → Application → Infrastructure → Presentation) |
| **Web Framework**| ASP.NET Core Web API                             |
| **Data Access**  | Dapper (lightweight, fast)                       |
| **Database**     | SQL Server (or LocalDB for development)          |
| **Testing**      | xUnit, Moq, Coverlet                             |
| **API Docs**     | Swashbuckle (Swagger UI)                          |
| **Containerization** | Docker Compose                                |
| **Package Manager** | NuGet                                          |
| **IDE Support**  | VS Code, Rider, Visual Studio                    |

---

## Project Structure

ByCoders/
├── ByCoders.Domain/          # Entities, value objects
├── ByCoders.Application/     # Use cases, DTOs, interfaces
├── ByCoders.Infrastructure/  # Dapper repos, EF Identity
├── ByCoders.WebApi/          # Web API, Controllers, Program.cs
├── ByCoders.Presentation/    # Presentation (Html + CSS), Controllers, Views, Program.cs
├── ByCoders.Tests/           # Unit tests (>70% coverage)
├── docker-compose.yml
└── README.md

---

## Prerequisites

| Tool | Version | Install |
|------|--------|--------|
| .NET SDK | 9.0+ | [dotnet.microsoft.com](https://dotnet.microsoft.com/download/dotnet/9.0) |
| SQL Server | 2019+ or LocalDB | `sqlcmd`, Docker, or install locally |
| Git | Any | `git --version` |
| Docker & Docker Compose (optional) | Latest | [docker.com](https://www.docker.com/) |

---

## Setup on Linux or Mac OS

### 1. Clone the Repository

```
git clone https://github.com/fabriciogs/ByCodersTec.git
cd ByCoders
```

### 2. Choose Your SQL Server Option

**Option A:** LocalDB (macOS/Linux via Docker)

```
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong@Passw0rd" \
  -p 1433:1433 --name sqlserver \
  -d mcr.microsoft.com/mssql/server:2022-latest
```
Wait ~30 seconds for SQL Server to start.

**Option B:** SQL Server on Docker (Production-like)

```
docker-compose up -d db
```
**Option C:** Local SQL Server (Linux)

```
# Ubuntu example
sudo apt update && sudo apt install mssql-server
# Follow setup: https://learn.microsoft.com/en-us/sql/linux/sql-server-linux-docker-container-deployment
```

## Database Setup - Create the Database

```
# Open the file: https://github.com/fabriciogs/ByCodersTec/blob/main/src/setup.sql and run the script
```

## Running the Application

**Option 1:** Local Development (.NET CLI)

```
cd ByCoders.WebApi
dotnet run
```

API will be available at:
HTTPS: https://localhost:5001
Swagger UI: https://localhost:5001/swagger

**Option 2:** Docker Compose (Recommended)
```
docker-compose up --build
```

API available at: http://localhost:5000

## API Usage (Swagger)

1. Open Swagger UI: https://localhost:5001/swagger
2. No authentication required
3. Use endpoints directly:

POST /api/transactions/upload
- Upload your CNAB.txt file
- Parses, normalizes (divides value by 100), and saves to DB

GET /api/transactions/stores

-Returns list of stores with:
    - Store name & owner
    - Total balance (with correct signs per transaction type)
    - All transactions


## Testing

```
dotnet test
```

Generate HTML coverage report:

```
dotnet test --collect:"XPlat Code Coverage"

# Install report generator if needed
dotnet tool install -g dotnet-reportgenerator-globaltool

reportgenerator \
  -reports:**/coverage.cobertura.xml \
  -targetdir:coveragereport \
  -reporttypes:Html

open coveragereport/index.html  # macOS
xdg-open coveragereport/index.html  # Linux
```


## Docker Compose File

```
version: '3.8'
services:
  db:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      ACCEPT_EULA: "Y"
      SA_PASSWORD: "YourStrong@Passw0rd"
      MSSQL_PID: "Express"
    ports:
      - "1433:1433"
    healthcheck:
      test: ["CMD", "/opt/mssql-tools/bin/sqlcmd", "-U", "sa", "-P", "YourStrong@Passw0rd", "-Q", "SELECT 1"]
      interval: 10s
      retries: 10

  api:
    build: ./ByCoders.Challenge.Api
    depends_on:
      db:
        condition: service_healthy
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Server=db;Database=bycoders_db;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true;
    ports:
      - "5000:80"
```

Authored by Fabricio Gabrielli da Silva - [GitHub](https://github.com/fabriciogs)