# UserIdentity API

🚀 **UserIdentity** é uma API REST desenvolvida em **.NET 6** que gerencia usuários, papéis, sistemas e departamentos. Ela oferece autenticação via **JWT**, envio de e-mail e adoção do padrão *Repository* para facilitar manutenção e testes.

## Recursos Principais

- 🔑 Autenticação de usuários com geração de tokens JWT
- 👥 Cadastro e gerenciamento de usuários
- 🛡️ Controle de papéis e permissões
- 🏢 Organização por departamentos
- 🖥️ Associação de usuários a diferentes sistemas
- 📧 Envio de e-mails utilizando templates personalizados

## Estrutura do Projeto

```
UserIdentity/
├── Controllers       # Endpoints da API
├── Data              # Contexto do Entity Framework
├── Models            # Entidades e DTOs
├── Repository        # Implementações de acesso a dados
├── Services          # Regras de negócio
├── Helper            # Utilidades (e-mail, gerador de senha, ...)
└── Program.cs        # Configuração da aplicação
```

## Pré‑requisitos

- [.NET 6 SDK](https://dotnet.microsoft.com/download) (ou Docker caso prefira containers)
- SQL Server ou outro banco compatível configurado na `ConnectionStrings`

## Configuração

1. Clone o repositório

```bash
git clone <repo-url>
cd UserIdentity
```

2. Ajuste o arquivo **`UserIdentity/appsettings.json`** com suas credenciais de banco, chaves JWT e dados de e-mail.

3. (Opcional) Utilize o Docker para buildar a aplicação:

```bash
docker build -t useridentity-api -f UserIdentity/Dockerfile .
docker run -p 5000:80 useridentity-api
```

## Executando localmente

Se estiver com o SDK instalado:

```bash
cd UserIdentity
 dotnet run
```

A API ficará disponível em `https://localhost:5001` (ou porta configurada).

A documentação interativa pode ser acessada via Swagger em `https://localhost:5001/swagger`.

## Contribuição

Contribuições são bem-vindas! Abra uma *issue* ou envie um *pull request* com melhorias, correções ou novas funcionalidades.

## Licença

Este projeto está sob a licença MIT - consulte o arquivo [LICENSE](LICENSE) para mais detalhes.
