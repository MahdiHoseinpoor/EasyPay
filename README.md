# EasyPay: Modern Financial Platform API

[![.NET](https://img.shields.io/badge/.NET-9-blueviolet.svg)](https://dotnet.microsoft.com/en/download/dotnet/9.0)
[![CQRS](https://img.shields.io/badge/Architecture-CQRS%20&%20DDD-orange.svg)](https://martinfowler.com/bliki/CQRS.html)
[![Clean Code](https://img.shields.io/badge/Code%20Style-Clean%20Code-brightgreen.svg)](https://www.amazon.com/Clean-Code-Handbook-Software-Craftsmanship/dp/0132350882)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

EasyPay is a showcase project demonstrating a robust, scalable, and maintainable financial application backend. It is built using modern .NET technologies and adheres strictly to **Domain-Driven Design (DDD)**, **CQRS**, and **Clean Architecture** principles. This project serves as a testament to my expertise in building enterprise-grade systems with a focus on code quality and architectural integrity.

`![EasyPay Dashboard](./docs/images/dashboard.gif)`

---

## ✨ Key Features

EasyPay is more than just a CRUD application; it's a feature-rich platform designed to handle core financial operations securely and efficiently.

#### 👤 **User & Identity Management**
- **Secure Onboarding:** Phone number-based registration with OTP verification (`NaturalUserRegisterPhoneCommand`).
- **JWT Authentication:** Stateless and secure authentication using JSON Web Tokens (`JwtTokenService`).
- **Multi-Step Registration:** A guided flow for new users from phone verification to profile completion (`CompleteNaturalUserProfileCommand`).
- **Role-Based Access Control (RBAC):** Granular permission management using ASP.NET Core Identity Roles and Claims. Features a custom MediatR pipeline behavior (`AuthorizationBehavior`) to protect commands based on permissions like `Permissions.Accounts.HardDelete`.
- **Admin & User Roles:** Clear separation between `SuperAdmin` and `BasicUser` roles.

#### 🏦 **Account Management**
- **Dynamic Account Creation:** Users can create accounts based on predefined `AccountType` entities.
- **Document-Driven Requirements:** Account creation is dynamically gated by user-submitted and admin-approved documents (`CreateAccountCommandHandler`).
- **Unique Account Number Generation:** A robust service (`AccountNumberService`) ensures no two accounts share the same number.
- **Admin-Managed Account Types:** Admins can define different types of accounts, each with its own rules and document requirements.

#### 💳 **Financial Transactions**
- **Internal Transfers:** Securely transfer funds between EasyPay accounts, with operations wrapped in EF Core transactions to ensure data integrity (`TransferMoneyCommandHandler`).
- **External Deposits:** Integrated with a payment gateway (Zarinpal) for funding accounts. The implementation uses a factory pattern (`IPaymentGatewayFactory`) to support multiple gateways in the future.
- **Withdrawals:** Users can withdraw funds to their saved bank cards.
- **Comprehensive History:** Paginated transaction history for user accounts (`GetMyTransactionHistoryQuery`).

#### 🛡️ **KYC & Verification System**
- **Document Submission:** Users can submit documents and information (`AuthItemValue`) required for account verification.
- **Admin Verification Panel:** Admins have a dedicated workflow to view, approve, or reject pending user submissions (`GetPendingSubmissionsQuery`, `ApproveAuthItemValueCommand`).

---

## 🏛️ Architectural Overview

The project is built upon the principles of **Clean Architecture**, ensuring a clear separation of concerns, high cohesion, and low coupling between layers.

```
+-------------------+      +--------------------+      +----------------------+      +---------------------+
|   Domain          | <--- |   Application      | <--- |   Infrastructure     | <--- |    Presentation     |
| (Entities, VO)    |      | (CQRS, DTOs,       |      | (EF Core,            |      | (API Controllers,   |
|                   |      | Interfaces)        |      | Repositories)        |      | Blazor Client)      |
+-------------------+      +--------------------+      +----------------------+      +---------------------+
```

### Domain-Driven Design (DDD)
The core of the application is the `EasyPay.Domain` layer, which contains the business logic, free from any infrastructure concerns.
- **Entities:** Rich domain models like `Account`, `ApplicationUser`, `Transaction`, and `AuthItemValue` encapsulate state and behavior.
- **Value Objects:** Immutable objects like `TransactionMetadata` ensure data consistency and validity.
- **Repositories:** Domain-centric data access interfaces are defined in `Infrastructure` and used by the `Application` layer.

### CQRS (Command Query Responsibility Segregation)
The `EasyPay.Application` layer uses **MediatR** to implement the CQRS pattern, separating read operations (Queries) from write operations (Commands).

- **Commands:** Represent an intent to change the state of the system. Each command has a single, dedicated handler.
  - _Example:_ `TransferMoneyCommand` is handled by `TransferMoneyCommandHandler`, which contains all the logic for validating and executing a fund transfer.
- **Queries:** Represent an intent to read data from the system. Queries return DTOs (`EasyPay.Shared`) to prevent leaking domain models to the UI.
  - _Example:_ `GetMyAccountsQuery` is handled by `GetMyAccountsQueryHandler` to fetch a list of accounts for the current user.

### MediatR Pipeline Behaviors
To keep handlers clean and focused, cross-cutting concerns are handled gracefully in the MediatR pipeline:
- **`ValidationBehavior<TRequest, TResponse>`:** Automatically validates incoming commands using **FluentValidation** before they reach the handler. This keeps validation logic separate and reusable.
- **`AuthorizationBehavior<TRequest, TResponse>`:** A custom behavior that checks if the authenticated user has the required permission to execute a command. This provides a declarative and powerful way to secure application features right at the source.

---

## 💻 Technology Stack & Key Libraries

| Category             | Technology / Library                                                                |
| -------------------- | ----------------------------------------------------------------------------------- |
| **Backend Framework**  | .NET 9, ASP.NET Core Web API                                                      |
| **Architecture**     | MediatR, AutoMapper, FluentValidation                                               |
| **Data Access**      | Entity Framework Core 9                                                             |
| **Database**         | SQL Server                                                                          |
| **Authentication**   | ASP.NET Core Identity, JWT Bearer Tokens                                            |
| **API**              | RESTful Principles, API Versioning (`Asp.Versioning.Mvc`)                           |
| **Frontend**         | Blazor WebAssembly                                                                  |
| **Testing**          | xUnit, Moq, FluentAssertions                                                        |
| **Logging**          | Serilog                                                                             |
| **Error Handling**   | Custom Middleware, Custom `Result<T>` pattern                                       |

---

## 🚀 Code Quality & Best Practices

This project was written with a strong emphasis on clean, maintainable, and testable code.

- **Result Pattern:** Instead of throwing exceptions for predictable business rule failures (e.g., insufficient funds), handlers return a custom `Result<T>` object. This makes error handling explicit and robust throughout the application stack, from the API controller down to the Blazor client.
- **Asynchronous Programming:** `async`/`await` is used extensively to ensure the application is scalable and responsive.
- **Dependency Injection:** The solution is heavily reliant on DI, with services registered in `Application` and `Infrastructure` layers for maximum decoupling.
- **Repository Pattern:** A generic `RepositoryBase<TEntity, TKey>` provides common data access logic, while specific repositories (`IAccountRepository`) extend it for domain-specific needs.
- **Unit Testing:** The project includes a dedicated unit test project (`EasyPay.Application.UnitTests`) that demonstrates how to test CQRS handlers in isolation using mocks.
- **Configuration Management:** Utilizes the `IOptions` pattern to inject strongly-typed configuration for JWT, storage, and payment gateway settings.
- **Global Exception Handling:** A custom middleware (`GlobalExceptionHandlingMiddleware`) catches any unhandled exceptions and returns a standardized `ProblemDetails` response.

---

## 🏁 Getting Started

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/en/download/dotnet/9.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (Express, Developer, or full version)
- A C# IDE like [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/).

### Configuration
1.  Open `EasyPay/src/Presentation/EasyPay.Api/appsettings.Development.json`.
2.  Update the `DefaultConnectionString` to point to your SQL Server instance.
3.  The `JwtSettings` and other configurations are pre-filled for local development.

### Running the Application
1.  **Run the API:**
    - Open the solution in Visual Studio.
    - Set `EasyPay.Api` as the startup project.
    - Press `F5` or run `dotnet run` from the `EasyPay.Api` directory.
    - The database will be created and seeded with sample data on the first run.
2.  **Run the Blazor Client:**
    - Set `EasyPay.Web` as the startup project.
    - Press `F5` or run `dotnet run` from the `EasyPay.Web` directory.
    - The Blazor application will open in your browser, connected to the local API.

---

### A Note on the UI

As a dedicated backend developer, my primary focus for this project was on robust architecture, clean code, and powerful server-side functionality. To complement this, the user interface was crafted with the assistance of AI design tools, enabling me to present a functional and visually appealing Blazor WebAssembly client without diverting from my core expertise in the .NET ecosystem.

---

## 📈 Future Improvements

This project provides a solid foundation that can be extended in many ways:
- **Develop Admin Panel:** Create a new Blazor app specifically for administrators.
- **Integration & End-to-End Testing:** Add a test project to cover API endpoints and user flows.
- **Implement Two-Factor Authentication (2FA):** Enhance security for password-based logins.
- **Expand Payment Gateway Support:** Add more implementations to the `IPaymentGatewayFactory`.
- **Real-time Notifications:** Use SignalR to provide real-time updates for transaction statuses.
- **Containerization:** Add Docker support for easier deployment.
