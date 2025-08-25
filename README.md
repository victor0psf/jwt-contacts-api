# 📇 Contacts API

API REST para gerenciamento de contatos com autenticação **JWT**, permitindo que usuários se registrem, façam login e, após autenticados, possam adicionar e gerenciar contatos em uma agenda.

---

## 🚀 Tecnologias utilizadas

- [.NET 9](https://dotnet.microsoft.com/) — Framework principal da aplicação  
- [PostgreSQL](https://www.postgresql.org/) — Banco de dados relacional  
- [Docker](https://www.docker.com/) — Containerização  
- [Swagger / OpenAPI](https://swagger.io/) — Documentação interativa da API  
- [JWT (JSON Web Token)](https://jwt.io/) — Autenticação e autorização  

---

## ⚙️ Funcionalidades

- Registro de usuários  
- Login com geração de token JWT  
- Autenticação e autorização via Bearer Token  
- CRUD de contatos (agenda)  
- Documentação interativa com Swagger  

---

## 🐳 Como rodar o projeto com Docker

### Pré-requisitos
- [Docker](https://www.docker.com/get-started) instalado  

### Passos

1. **Clonar o repositório**
   ```bash
   git clone https://github.com/seu-usuario/contacts-api.git
   cd contacts-api
   ```
2. Criar o arquivo .env
   ```bash
    POSTGRES_USER=postgres
    POSTGRES_PASSWORD=changeme
    POSTGRES_DB=gerenciador_contatos
    JWT_KEY=changeme_jwt_key
    ```
3. Subir os containers
   ```bash
   docker compose up --build
    ```
4. Acessar a API
  ```bash
  Swagger: http://localhost:8080/swagger
  API: http://localhost:8080/api/...
   ```
## 👨‍💻 Autor

*Desenvolvido por* [Paulo Victor dos Santos Fonseca](https://github.com/victor0psf)  
*Email:* pvictorsf07@outlook.com
