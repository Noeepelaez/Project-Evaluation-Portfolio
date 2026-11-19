# Expense Tracker

## Project idea

An application to record my expenses during my Erasmus and see how much I
spend each month and on what. The same idea is improved in every milestone.

## How to run

Needs .NET 8 or newer. From the root folder:

```
cd Milestone0X_Name
dotnet run
```

| Milestone | Folder | How to use it |

| 01 | Milestone01_BasicConsole | Console menu |
| 02 | Milestone02_OOP | Console menu |
| 03 | Milestone03_LayeredConsole | Console: register, login and menu |
| 04 | Milestone04_Database | Console: register, login and menu |
| 05 | Milestone05_WebAPI | Open http://localhost:5080/swagger |


## Milestone 01 - Basic Console

Demonstrates: a console menu to add, list, find and delete expenses and a
monthly summary per category.

Backend concepts: variables and basic types, conditions, loops, methods,
collections (List of Dictionary).

Persistence: expenses.json.


## Milestone 02 - OOP

Changed: the dictionaries were replaced by classes (Expense, ExpenseManager,
ExpenseFileStorage) and a Category enum. New option to edit an expense.

Why: in Milestone 01 everything was text and any invalid data could be saved.

Backend concepts: classes, objects, properties, constructors, encapsulation
(private setters that validate the data), enums.

Persistence: expenses.json with objects.


## Milestone 03 - Layers and Users

Changed: the code is split into Presentation, Services, Repositories and
Models. Users can register, log in and log out, and each user only sees their
own expenses.

Why: Milestone 02 had only one user and the classes depended directly on each
other.

Backend concepts: layered architecture, repository interfaces, current user,
owner-specific data (UserId), validation rules in the services, password
hashing with salt.

Persistence: users.json and expenses.json.


## Milestone 04 - Database

Changed: the JSON repositories were replaced by SQLite repositories using
Dapper. The services and the menus did not change, thanks to the interfaces.

Why: JSON files have no relationships and every query had to be done in C#.

Database concepts:

- Tables: Users, Categories, Expenses (Database/schema.sql)
- Relationships: Users 1-N Expenses and Categories 1-N Expenses (foreign keys)
- Primary keys, UNIQUE, NOT NULL, CHECK and an index
- Queries: INSERT, SELECT, UPDATE, DELETE, JOIN, LIKE, SUM, COUNT, GROUP BY
- Parameterised queries against SQL injection


## Milestone 05 - Web API

Changed: the console was replaced by an ASP.NET Core Web API. It uses the same
database and repositories as Milestone 04.

Why: a Web API lets any client use the application through HTTP.

Backend concepts: controllers, DTOs, validation attributes, HTTP status codes,
JWT authentication, Swagger.

| Method | Route | Status codes |

| POST | /api/auth/register | 201, 400, 409 |
| POST | /api/auth/login | 200, 400, 401 |
| GET | /api/expenses | 200, 400, 401 |
| GET | /api/expenses/{id} | 200, 401, 404 |
| POST | /api/expenses | 201, 400, 401 |
| PUT | /api/expenses/{id} | 200, 400, 401, 404 |
| PATCH | /api/expenses/{id} | 200, 400, 401, 404 |
| DELETE | /api/expenses/{id} | 204, 401, 404 |
| GET | /api/expenses/summary | 200, 400, 401 |

To test: register, login, copy the token, press Authorize in Swagger and
paste it. Dates use the format dd-MM-yyyy.
