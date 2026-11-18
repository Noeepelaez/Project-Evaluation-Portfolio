# Erasmus Expense Tracker

## Project idea

As an Erasmus student I need to keep track of what I spend every month (rent, food,
transport, trips...). **Erasmus Expense Tracker** is an application to record personal
expenses, search them and see a monthly summary per category.

The same idea is developed step by step during the semester. Each folder is a
runnable milestone that shows how the application evolved as new backend and database
concepts were learned.

| Folder | Milestone | Status |
|---|---|---|
| `Milestone01_BasicConsole` | Basic console application | ✅ Done |
| `Milestone02_OOP` | OOP and persistence | ⏳ |
| `Milestone03_LayeredConsole` | Layered multi-user console app | ⏳ |
| `Milestone04_Database` | Relational database (SQLite + Dapper) | ⏳ |
| `Milestone05_WebAPI` | ASP.NET Core Web API | ⏳ |

## Requirements

- [.NET SDK 8](https://dotnet.microsoft.com/download) or newer
- Visual Studio Code with the **C# Dev Kit** extension (or Visual Studio)

---

## Milestone 01 – Basic Console Application

### What it demonstrates

A single-file console application (`Program.cs`) with a text menu:

1. Add expense (description, amount, category, date)
2. List all expenses (sorted by date, with total)
3. Find expenses by text (description or category)
4. Delete an expense by id (with confirmation)
5. Monthly summary: total per category, percentage and a small bar chart
0. Exit

### Concepts used

| Concept | Where |
|---|---|
| Variables and basic types | `string`, `int`, `decimal`, `bool`, `DateTime`, constants `DataFile`, `DateFormat` |
| Conditions | `if/else`, `switch` in the menu, input validation |
| Loops | `while` menu loop, `for` to find an id, `foreach` for totals and searches |
| Methods | `AddExpense`, `ListExpenses`, `ReadAmount`, `ReadDate`, `ShowMonthlySummary`… |
| Collections | `List<Dictionary<string, string>>` for expenses, `string[]` for categories |
| Add / List / Find / Delete | Menu options 1–4 |
| JSON persistence | `System.Text.Json` – data is loaded at start-up and saved after each change in `expenses.json` |

### Design decision / limitations

In this first version an expense is just a `Dictionary<string, string>`
(keys `Id`, `Date`, `Description`, `Category`, `Amount`). No custom classes are used yet.
This works, but it has clear problems that motivate the next milestone:

- everything is a `string`, so the amount must be parsed every time (`GetAmount`);
- a typo in a key (`expense["Ammount"]`) compiles but crashes at runtime;
- nothing protects the data: any code can put an invalid amount in the dictionary;
- all logic (menu, rules, file access) is mixed in one file.

### How to run

```bash
cd Milestone01_BasicConsole
dotnet run
```

The file `expenses.json` is created in the folder where the app is run.
