# ToDoApp API

A modern, robust, and scalable ToDo RESTful API built with **.NET 10** using **Clean Architecture** principles. This project demonstrates enterprise-level backend development, featuring secure authentication, global exception handling, and structural logging.

## 🚀 Technologies & Tools

* **Framework:** .NET 10.0
* **Architecture:** Clean Architecture (N-Tier)
* **Database:** Entity Framework Core & SQL Server
* **Authentication:** JWT (JSON Web Token)
* **Object Mapping:** AutoMapper
* **Validation:** FluentValidation
* **Logging:** Serilog (Console & File Rolling)
* **API Documentation:** Scalar UI & Swagger UI

## 🏗️ Architecture Design (Clean Architecture)

The project is divided into highly decoupled layers to ensure maintainability and separation of concerns:

* **1. Domain Layer:** Contains core entities (AppUser, ToDoItems, Category). Zero dependencies on external libraries.
* **2. Application Layer:** Contains Business Logic, DTOs, Service Interfaces, AutoMapper Profiles, FluentValidation rules, and Custom Exceptions.
* **3. Infrastructure Layer:** Handles cross-cutting concerns like JWT Token Generation and security operations.
* **4. Persistence Layer:** Manages Database Context (AppDbContext), EF Core Migrations, Generic Repositories, and the Unit of Work pattern.
* **5. API Layer:** The entry point. Contains Controllers, Global Exception Handler Middleware, Custom Action Filters, and Serilog configuration.

## ✨ Key Features

* **Custom Action Filters:** Includes specialized attributes like `[GuestOnly]` to prevent authenticated users from accessing login/register endpoints, keeping the business logic clean.
* **Global Exception Handling:** Built-in `.NET ProblemDetails` integrated with a custom `GlobalExceptionHandler` to catch all errors centrally and return standardized JSON error responses.
* **Unit of Work & Repository Pattern:** Abstracts database operations, ensuring transactions are committed safely and efficiently.
* **Dual API Documentation:** Powered by both the classic **Swagger UI** and the modern **Scalar API Reference** using .NET 10's native OpenAPI generation.

## 🛠️ How to Run the Project

**1. Clone the repository**
```bash
git clone [https://github.com/sincesu/ToDoApp.git](https://github.com/sincesu/ToDoApp.git)
