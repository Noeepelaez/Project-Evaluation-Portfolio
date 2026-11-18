# Expense Tracker

## Project idea

An application to record personal expenses during my Erasmus and see how much
I spend each month and on what.

The same project is improved step by step during the semester. Each folder is
one milestone and can be run on its own.

- Milestone01_BasicConsole - basic console application
- Milestone02_OOP - classes and objects
- Milestone03_LayeredConsole - layers and users
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


## Milestone 02 - OOP and Persistence

### What changed from Milestone 01

The program does the same things, but the code is organised in classes instead
of one file with dictionaries. A new option was added: 4) Edit expense.

- Models/Category.cs - an enum with the categories. Before it was a string[],
  so any text could be saved as a category.
- Models/Expense.cs - the Expense class. Each field has its real type
  (int, string, decimal, Category, DateTime) instead of being a string.
- ExpenseManager.cs - keeps the list of expenses and has the operations:
  add, search, edit, delete and monthly totals.
- ExpenseFileStorage.cs - loads and saves the list in expenses.json.
- Program.cs - only the menu and reading input from the user.

### Concepts used

- Classes and objects: Expense, ExpenseManager, ExpenseFileStorage
- Properties: Id, Description, Amount, Category, Date
- Constructors: an Expense can only be created with valid data, and
  ExpenseManager receives the storage it uses
- Encapsulation: the fields are private and the setters are private.
  The setters check the data (empty description or amount <= 0 throws an
  error). The only way to change an expense is the Update method, and the
  list of expenses inside ExpenseManager is private.
- Enum for the categories
- JSON persistence with System.Text.Json, saving the categories as text
  ("Food") and using the Expense constructor when loading

### Limitations

There is only one user and anyone who runs the program sees all the expenses.
The classes still depend directly on each other (ExpenseManager creates and
saves through ExpenseFileStorage) and the input validation is in Program.cs.
Milestone 03 separates the code into layers and adds users.

### How to run

```
cd Milestone02_OOP
dotnet run
```

The expenses.json of this milestone has a different format from Milestone 01
(real numbers and dates instead of text), so each milestone has its own file.


## Milestone 03 - Layered Multi-user Console Application

### What changed from Milestone 02

The application now has users. Each user registers, logs in and only sees
and changes their own expenses. The code is split into layers, each one in
its own folder:

- Presentation - ConsoleUI (menus) and ConsoleInput (reading input).
  It only talks to the services.
- Services - AuthService (register, login, logout, current user),
  ExpenseService (operations on the expenses of the current user),
  PasswordHasher and ValidationException. All the validation rules are here.
- Repositories - IUserRepository and IExpenseRepository (interfaces) and
  their JSON implementations. They only load and save data.
- Models - User, Expense and Category. Simple classes with only data.

Program.cs creates the repositories, the services and the UI and connects them.

### Users

- Register: username and password (asked twice). The password is shown as ***.
- Login: the logged user becomes the current user (AuthService.CurrentUser).
- Logout: option 7 in the main menu. The app goes back to the login menu.
- Passwords are not saved in plain text. They are saved as a hash with a salt
  (PBKDF2 with SHA-256).
- Each expense has a UserId. ExpenseService always filters by the current
  user, so a user cannot see, edit or delete the expenses of another user,
  even if they type the id of that expense.

### Validation rules

- Username: 3 to 20 characters, only letters, numbers and _, and not taken
- Password: at least 6 characters and at least one number
- Description: not empty, maximum 100 characters
- Amount: greater than 0 and maximum 10000
- Date: cannot be in the future

If a rule is broken the service throws a ValidationException and the
presentation layer shows the message.

### Why the changes

In Milestone 02 the validation was inside the Expense class and ExpenseManager
saved directly to the file. Now each layer has one job. The services do not
know how data is saved because they use the repository interfaces, so in
Milestone 04 the JSON repositories can be replaced by database repositories
without changing the services or the UI.

### How to run

```
cd Milestone03_LayeredConsole
dotnet run
```

Data is saved in users.json and expenses.json in this folder.
