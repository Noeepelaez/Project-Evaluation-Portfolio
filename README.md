# Expense Tracker

An app to write down my expenses during my Erasmus and see how much I spend
each month. The same idea is improved step by step in each milestone.

To run any milestone (needs .NET 8 or newer):

```
cd <milestone folder>
dotnet run
```


## Milestone 01 - Basic Console

- Console menu: add, list, find, delete expenses and monthly summary
- Variables, conditions, loops, methods
- Each expense is a Dictionary inside a List
- Data saved in expenses.json

Problem: everything is text and all the code is in one file.


## Milestone 02 - OOP

Changes:

- Expense class with properties and a constructor
- Category enum
- Encapsulation: private setters that check the data
- ExpenseManager for the operations and ExpenseFileStorage for the JSON
- New option: edit expense

Problem: only one user and the classes depend directly on each other.


## Milestone 03 - Layers and Users

Changes:

- Code split in layers: Presentation, Services, Repositories, Models
- Repositories behind interfaces (IUserRepository, IExpenseRepository)
- Register, login and logout, with a current user
- Each user only sees their own expenses (UserId)
- Passwords saved as a hash
- Validation rules in the services

Data saved in users.json and expenses.json.


## Milestone 04 - Database

Changes:

- SQLite database (expenses.db) with Dapper instead of JSON
- Only the repositories changed, the services and the menus are the same
- Search, monthly total and summary are SQL queries

Tables (created by Database/schema.sql):

- Users (Id, Username, PasswordHash, PasswordSalt, CreatedAt)
- Categories (Id, Name)
- Expenses (Id, UserId, CategoryId, Description, Amount, Date)

Relationships:

- Users 1 - N Expenses (UserId)
- Categories 1 - N Expenses (CategoryId)

SQL used: CREATE TABLE, PRIMARY KEY, FOREIGN KEY, UNIQUE, CHECK, INSERT,
SELECT, UPDATE, DELETE, JOIN, WHERE, LIKE, SUM, COUNT, GROUP BY, ORDER BY.


## Milestone 05 - Web API

Changes:

- ASP.NET Core Web API instead of the console menu
- Same database and repositories as Milestone 04
- DTOs for the requests and responses, validated with attributes
- Login returns a JWT token, the expense endpoints need it
- Each user only gets their own expenses (another user's expense gives 404)
- Swagger to test the API

Endpoints:

- POST /api/auth/register - 201, 400, 409
- POST /api/auth/login - 200, 400, 401
- GET /api/auth/me - 200, 401
- GET /api/categories - 200
- GET /api/expenses - 200, 400 (optional: search, year, month)
- GET /api/expenses/{id} - 200, 404
- POST /api/expenses - 201, 400
- PUT /api/expenses/{id} - 200, 400, 404
- PATCH /api/expenses/{id} - 200, 400, 404
- DELETE /api/expenses/{id} - 204, 404
- GET /api/expenses/summary?year=2026&month=11 - 200, 400

Without a valid token the expense endpoints return 401.
Dates are sent and returned as dd-MM-yyyy, e.g. "19-11-2026".

To test it: run it, open http://localhost:5080/swagger, register, login,
copy the token, press Authorize and paste it.
