# 🌾 Satna Mandi Management System (Mini ERP)

A highly secure, high-performance Full-Stack Web Application built from scratch using **.NET Core MVC** and **PostgreSQL**. This application serves as a Digital Inventory and Billing system designed specifically for the wholesale fruit and vegetable merchants of Satna Mandi, Madhya Pradesh.

---

## 🚀 Key Features

*   🔐 **Secure Authentication System:** Implemented native **Cookie Authentication** with automated route protection via `[Authorize]` attributes.
*   🔑 **Enterprise-Grade Security:** User passwords are encrypted and secured using **BCrypt Hashing** (`BCrypt.Net-Next`) preventing clear-text database exposure.
*   📊 **Business Intelligence Dashboard:** Dynamic KPI summary cards calculating **Total Product Varieties**, **Total Available Stock (Quintals)**, and **Total Estimated Valuation (INR)** in real-time via optimized LINQ queries.
*   🧺 **Complete Inventory CRUD:** Full capability to Create, Read, Update, and Delete mandi stock items with strict asynchronous data bindings.
*   🛡️ **Accident-Proof Deletions:** Structured with a dedicated **Delete Confirmation Page** (`Delete.cshtml`) to safeguard critical business transactions from accidental single-click losses.
*   🖨️ **Native PDF Invoice Generation:** Features a zero-dependency, lightning-fast native printing engine that generates colorful, watermark-backed official digital invoices directly from the browser viewport.

---

## 🛠️ Technology Stack Used

*   **Backend Framework:** ASP.NET Core 8.0 MVC (C#)
*   **Database:** PostgreSQL Management System
*   **ORM Driver:** Entity Framework Core (EF Core) via `Npgsql.EntityFrameworkCore.PostgreSQL`
*   **Architecture Pattern:** Repository-Ready Architecture with `IDesignTimeDbContextFactory` implementation for standard configuration management.
*   **Frontend Technologies:** HTML5, CSS3, JavaScript (ES6), and Bootstrap 5 for responsiveness.
*   **Security Libraries:** BCrypt.Net-Next

---

## ⚙️ How to Run This Project Locally

### 1. Prerequisites
Ensure you have the following installed on your machine:
*   Visual Studio 2022 (with ASP.NET and web development workload)
*   PostgreSQL Desktop Server & pgAdmin 4
*   .NET 8.0 SDK

### 2. Database Configuration
Open your `appsettings.json` file and replace the connection string with your local PostgreSQL credentials:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=UserLoginDb;Username=postgres;Password=YOUR_PASSWORD"
  }
}
```

### 3. Apply Entity Framework Migrations
Open the **Package Manager Console** inside Visual Studio and execute the database schema builder:
```powershell
Update-Database
```

### 4. Fire It Up!
Press `F5` or click **Run** inside Visual Studio. The application will safely secure the entry points and launch the **Registration/Login Screen**.

---

## 💡 Developer's Insight (The Comeback Journey)
> "This project marks a definitive milestone. After a career transition gap since my B.E. Computer Science graduation in 2012, this enterprise application was coded continuously within single-day debugging sprints in 2026. Every database factory, security layer, and state mechanism was built independently by learning through official Microsoft documentations and resolving design-time constraints."
