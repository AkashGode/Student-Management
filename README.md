# 🎓 Student Management System API

ASP.NET Core 8 Web API project to manage student data with authentication, logging, and Docker support.

---

## 📌 Overview

This project provides APIs to:
- Manage student records (create, update, delete, fetch)
- Secure endpoints using JWT authentication
- Handle exceptions globally
- Log API activity using Serilog
- Run using Docker (API + SQL Server)

---

## ✅ Features

- CRUD APIs for Students  
- JWT-based authentication  
- Global exception handling middleware  
- Request/response logging using Serilog  
- Swagger UI with authorization support  
- Clean layered architecture  
- Entity Framework Core with SQL Server  
- Input validation and duplicate email check  
- Docker support (API + DB)  
- Unit tests using xUnit  

---

## 🏗 Architecture

```
Client (Swagger / UI)
        ↓
Controllers (API Layer)
        ↓
Services (Business Logic)
        ↓
Repositories (Data Access)
        ↓
SQL Server (Database)
```

---

## 🛠 Tech Stack

- ASP.NET Core 8 Web API  
- Entity Framework Core + SQL Server  
- JWT Authentication  
- Serilog  
- Swagger  
- Docker / Docker Compose  
- xUnit + Moq  

---

## 🐳 Run with Docker

Make sure Docker Desktop is running.

```bash
git clone https://github.com/AkashGode/Student-Management.git
cd StudentManagement

docker-compose up --build
```

Open in browser:

http://localhost:5000

---

## 💻 Run Locally

### 1. Update connection string

StudentManagement.API/appsettings.Development.json

```
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=StudentManagementDB;Trusted_Connection=True;"
}
```

### 2. Apply migrations

```
cd StudentManagement.API
dotnet ef database update --project ../StudentManagement.Infrastructure
```

### 3. Run API

```
dotnet run
```

Open Swagger:

https://localhost:7001

---

## 📡 API Endpoints

### Auth
- POST /api/auth/login

### Students
- GET /api/students
- GET /api/students/{id}
- POST /api/students
- PUT /api/students/{id}
- DELETE /api/students/{id}

---

## 🔐 Authentication

### Login

```
{
  "username": "admin",
  "password": "Admin@123"
}
```

---

### Demo Users

| Username | Password |
|----------|----------|
| admin    | Admin@123 |
| user     | User@123  |

---

## 🧪 Run Tests

```
dotnet test StudentManagement.Tests/
```

---

## 📁 Project Structure

```
StudentManagement/
 ├── API
 ├── Core
 ├── Infrastructure
 ├── Tests
 ├── docker-compose.yml
 └── Dockerfile
```

---

## 🗄 Database Info

When running with Docker:

- SQL Server runs inside container  
- Connection uses: Server=sqlserver  

External access:

Server: localhost,1433  
User: sa  
Password: Admin@12345  

---

## 📝 Logging

Logs are stored in:

/logs/studentmgmt-*.log

---

## 👨‍💻 Author

Akash Gode  
Full Stack .NET Developer
