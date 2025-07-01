# PicPay Challenge - Sistema de Transferências Simplificado

Este projeto é uma implementação do [Desafio Técnico Backend do PicPay](https://github.com/PicPay/picpay-desafio-backend) utilizando **ASP.NET Core 8.0**, **C#**, **Entity Framework Core** e **SQLite**.

## Sobre o Projeto

O PicPay Simplificado é uma plataforma de pagamentos onde é possível depositar e realizar transferências de dinheiro entre usuários. O sistema possui dois tipos de usuários:

- **Usuários Comuns**: Podem enviar e receber dinheiro
- **Lojistas**: Apenas recebem dinheiro, não podem enviar

## Tecnologias Utilizadas

- **ASP.NET Core 8.0** - Framework web para APIs REST
- **C# 11** - Linguagem de programação
- **Entity Framework Core** - ORM para acesso ao banco de dados
- **SQLite** - Banco de dados (fácil para desenvolvimento e testes)
- **Swagger/OpenAPI** - Documentação automática da API
- **HttpClient** - Para integração com serviços externos

## Arquitetura do Projeto

O projeto segue uma arquitetura em camadas bem definida:

```
PicPayChallenge/
├── Controllers/        # Endpoints da API REST
├── Models/            # Entidades do domínio
├── Data/              # Configuração do Entity Framework
├── Migrations/        # Migrações do banco de dados
└── Program.cs         # Configuração da aplicação
```

### Principais Componentes

- **Controllers**: Responsáveis por receber requisições HTTP e orquestrar as operações
- **Models**: Entidades que representam os dados (User, Transaction)
- **DbContext**: Configuração do Entity Framework e relacionamentos

## 📊 Modelo de Dados

### User (Usuário)
- `Id`: Identificador único
- `FullName`: Nome completo
- `CPF`: CPF ou CNPJ (único)
- `Email`: Endereço de e-mail (único)
- `Password`: Senha (armazenada como hash)
- `Balance`: Saldo da carteira
- `UserType`: Tipo (Common = 1, Merchant = 2)

### Transaction (Transação)
- `Id`: Identificador único
- `PayerId`: ID do usuário que envia
- `PayeeId`: ID do usuário que recebe
- `Amount`: Valor da transferência
- `CreatedAt/ProcessedAt`: Timestamps

## Funcionalidades Implementadas

### Regras de Negócio Atendidas

1. **Validação de Dados Únicos**: CPF/CNPJ e e-mail únicos no sistema
2. **Tipos de Usuário**: Diferenciação entre usuários comuns e lojistas
3. **Restrição de Lojistas**: Lojistas não podem enviar transferências
4. **Validação de Saldo**: Verificação de saldo suficiente antes da transferência
5. **Autorização Externa**: Consulta ao serviço mock de autorização
6. **Transações Atômicas**: Operações de transferência são transacionais
7. **Notificações**: Envio de notificação via serviço externo (assíncrono)
8. **API RESTful**: Endpoints seguindo padrões REST

### 🔗 Endpoints da API

#### Usuários
- `POST /api/users` - Criar novo usuário
- `GET /api/users` - Listar todos os usuários

#### Transações
- `GET /api/transactions` - Listar todas as transações

## Como Executar

### Pré-requisitos
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Git

### Passos para Execução

1. **Clone o repositório**
   ```bash
   git clone <https://github.com/cnthigu/DesafioPicPay-backend.git>
   cd PicPayChallenge
   ```

2. **Restaurar dependências**
   ```bash
   dotnet restore
   ```

3. **Aplicar migrações do banco**
   ```bash
   dotnet ef database update
   ```

4. **Executar a aplicação**
   ```bash
   dotnet run
   ```

5. **Acessar a documentação**
   - Swagger UI: `http://localhost:5000`
   - API Base URL: `http://localhost:5000/api`

## Exemplos de Uso

### Criar Usuário Comum
```bash
curl -X POST http://localhost:5000/api/users \
  -H "Content-Type: application/json" \
  -d '{
    "fullName": "Higor Carniato",
    "cpf": "12345678900",
    "email": "higor@example.com",
    "password": "senha123",
    "balance": 500.00,
    "type": 1
  }'
```

### Realizar Transferência
```bash
curl -X POST http://localhost:5000/api/transactions/transfer \
  -H "Content-Type: application/json" \
  -d '{
  "payerId": 1,
  "payeeId": 2,
  "amount": 100.00
  }'
```

## Desenvolvedor

Este projeto foi desenvolvido como parte de um estudo de caso para demonstrar conhecimentos em:

- Desenvolvimento de APIs REST com ASP.NET Core
- Arquitetura em camadas e separação de responsabilidades
- Entity Framework Core e migrações
- Implementação de regras de negócio complexas
- Integração com serviços externos
- Tratamento de erros e validações
- Documentação de APIs com Swagger



## Essa é uma primeira versão, muita coisa precisa se melhorada e implementada.