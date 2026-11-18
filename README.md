# Expense Tracker

## Project idea

An application to record personal expenses during my Erasmus and see how much
I spend each month and on what.

The same project is improved step by step during the semester. Each folder is
one milestone and can be run on its own.

- Milestone01_BasicConsole - basic console application
- Milestone02_OOP - classes and objects (coming)
- Milestone03_LayeredConsole - layers and users (coming)
- Milestone04_Database - relational database (coming)
- Milestone05_WebAPI - ASP.NET Core Web API (coming)

## Requirements

- .NET SDK 8 or newer
- Visual Studio Code with the C# Dev Kit extension, or Visual Studio


## Milestone 01 - Basic Console Application

### What it does

A console menu with these options:

1. Add an expense (description, amount, category and date)
2. List all expenses, sorted by date, with the total
3. Find expenses by text in the description or category
4. Delete an expense by its id
5. Monthly summary: total per category for a month
0. Exit

The menu also shows how much has been spent in the current month.

Dates are written as dd-MM-yyyy (18-11-2026) and months as MM-yyyy (11-2026).
Categories: Food, Rent, Transport, Travel, Leisure, Studies, Sport, Other.

### Concepts used

- Variables and basic types: string, int, decimal, bool, DateTime
- Conditions: if/else and switch
- Loops: while, for and foreach
- Methods: each menu option is its own method, plus helper methods to read input
- Collections: List, Dictionary and arrays
- JSON persistence: expenses are saved in expenses.json after every change and
  loaded when the program starts

### Limitations

Each expense is stored as a Dictionary<string, string>. Because of this,
everything is text (the amount and the date must be converted every time),
a typo in a key is only detected when the program runs, and all the code is
in one file. Milestone 02 fixes this by using classes.

### How to run

```
cd Milestone01_BasicConsole
dotnet run
```
