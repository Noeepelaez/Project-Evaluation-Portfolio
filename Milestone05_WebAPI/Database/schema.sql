CREATE TABLE IF NOT EXISTS Users (
    Id           INTEGER PRIMARY KEY AUTOINCREMENT,
    Username     TEXT    NOT NULL UNIQUE COLLATE NOCASE,
    PasswordHash TEXT    NOT NULL,
    PasswordSalt TEXT    NOT NULL,
    CreatedAt    TEXT    NOT NULL DEFAULT (datetime('now'))
);

CREATE TABLE IF NOT EXISTS Categories (
    Id   INTEGER PRIMARY KEY,
    Name TEXT    NOT NULL UNIQUE
);

INSERT OR IGNORE INTO Categories (Id, Name) VALUES
    (1, 'Food'),
    (2, 'Rent'),
    (3, 'Transport'),
    (4, 'Travel'),
    (5, 'Leisure'),
    (6, 'Studies'),
    (7, 'Sport'),
    (8, 'Other');

CREATE TABLE IF NOT EXISTS Expenses (
    Id          INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId      INTEGER NOT NULL,
    CategoryId  INTEGER NOT NULL,
    Description TEXT    NOT NULL CHECK (length(Description) BETWEEN 1 AND 100),
    Amount      REAL    NOT NULL CHECK (Amount > 0),
    Date        TEXT    NOT NULL,
    FOREIGN KEY (UserId)     REFERENCES Users(Id) ON DELETE CASCADE,
    FOREIGN KEY (CategoryId) REFERENCES Categories(Id)
);

CREATE INDEX IF NOT EXISTS IX_Expenses_UserId_Date ON Expenses (UserId, Date);
