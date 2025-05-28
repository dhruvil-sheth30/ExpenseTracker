# ExpenseTracker

A full-stack expense tracking application built with Blazor WebAssembly and ASP.NET Core API.

## Technology Stack

- **Frontend**: Blazor WebAssembly
- **Backend**: ASP.NET Core Web API
- **Database**: Azure SQL Server
- **Authentication**: JWT-based authentication
- **Deployment**:
  - Backend: Render.com
  - Frontend: Vercel

## Features

- User authentication (register and login)
- Expense management (add, edit, delete)
- Category-based expense tracking
- Date range filtering
- Dashboard with expense summaries
- Responsive UI

## Project Structure

- **ExpenseTracker.API**: Backend Web API project
- **ExpenseTracker.Client**: Blazor WebAssembly frontend project
- **ExpenseTracker.Shared**: Shared models and DTOs

## Getting Started

### Prerequisites

- .NET 7.0 SDK or later
- Visual Studio 2022 or VS Code
- Azure SQL Server (or local SQL Server for development)

### Setup

1. Clone this repository
2. Update the connection string in `ExpenseTracker.API/appsettings.json`
3. Run database migrations: `dotnet ef database update --project ExpenseTracker.API`
4. Run the API project: `dotnet run --project ExpenseTracker.API`
5. Run the Client project: `dotnet run --project ExpenseTracker.Client`

## API Endpoints

- **Authentication**
  - POST `/api/auth/register` - Register a new user
  - POST `/api/auth/login` - Login and receive JWT token

- **Expenses** (Protected)
  - GET `/api/expenses` - Get all expenses for logged-in user
  - POST `/api/expenses` - Add a new expense
  - PUT `/api/expenses/{id}` - Update an expense
  - DELETE `/api/expenses/{id}` - Delete an expense