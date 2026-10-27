-- Лабораторна робота № 8. Тригери
-- MS SQL Server, бази даних Sales111 і LibraryDB

USE Sales111;
GO

-- Видалення тригерів, якщо вони вже існують
IF OBJECT_ID(N'dbo.trg_Orders_Date', N'TR') IS NOT NULL DROP TRIGGER dbo.trg_Orders_Date;
IF OBJECT_ID(N'dbo.trg_Customers_Delete', N'TR') IS NOT NULL DROP TRIGGER dbo.trg_Customers_Delete;
IF OBJECT_ID(N'dbo.trg_Orders_Commission', N'TR') IS NOT NULL DROP TRIGGER dbo.trg_Orders_Commission;
IF OBJECT_ID(N'dbo.trg_Orders_MinAmount', N'TR') IS NOT NULL DROP TRIGGER dbo.trg_Orders_MinAmount;
GO

CREATE TRIGGER trg_Orders_Date ON Orders
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM inserted WHERE DAY(DATE_OD) > 25)
    BEGIN
        RAISERROR(N'Замовлення можна оформлювати лише з 1 по 25 число місяця', 16, 1);
        ROLLBACK TRANSACTION;
    END
END
GO

CREATE TRIGGER trg_Customers_Delete ON Customers
INSTEAD OF DELETE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (
        SELECT 1
        FROM deleted d
        JOIN Orders o ON o.ID_CM = d.ID_CM
    )
    BEGIN
        RAISERROR(N'Не можна видалити замовника, який має замовлення', 16, 1);
        RETURN;
    END;

    DELETE FROM Customers
    WHERE ID_CM IN (SELECT ID_CM FROM deleted);
END
GO

CREATE TRIGGER trg_Orders_Commission ON Orders
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE s
    SET COMM_SP = COMM_SP + 0.01 * x.cnt
    FROM Salespeople s
    JOIN (
        SELECT ID_SP, COUNT(*) AS cnt
        FROM inserted
        WHERE AMOUNT_OD > 1000
        GROUP BY ID_SP
    ) x ON x.ID_SP = s.ID_SP;
END
GO

CREATE TRIGGER trg_Orders_MinAmount ON Orders
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM inserted WHERE AMOUNT_OD < 10)
    BEGIN
        RAISERROR(N'Сума замовлення не може бути меншою за 10 грн', 16, 1);
        ROLLBACK TRANSACTION;
    END
END
GO

-- 1.1. Створити тригер, який запускається під час вставки або модифікації замовлення і не дозволяє оформити його після 25-го числа місяця.
-- Дозволене замовлення: 15 число
INSERT INTO Orders (ID_OD, AMOUNT_OD, DATE_OD, ID_CM, ID_SP)
VALUES (3013, 500.00, '2024-03-15', 2001, 1003);

SELECT ID_OD, DATE_OD, AMOUNT_OD
FROM Orders
WHERE ID_OD = 3013;

-- Заборонене замовлення: 28 число
INSERT INTO Orders (ID_OD, AMOUNT_OD, DATE_OD, ID_CM, ID_SP)
VALUES (3014, 500.00, '2024-03-28', 2001, 1003);
GO
DELETE FROM Orders WHERE ID_OD IN (3013, 3014);
GO

-- 1.2. Створити тригер, який не дозволяє видалити замовника, якщо він має замовлення.
IF NOT EXISTS (SELECT 1 FROM Customers WHERE ID_CM = 2005)
                INSERT INTO Customers (ID_CM, NAME_CM, CITY_CM, BONUS_CM)
                VALUES (2005, N'Шевченко', N'Чернігів', 150);
GO
-- Клименко має замовлення – видалення заборонено
DELETE FROM Customers WHERE ID_CM = 2006;

-- Шевченко не має замовлень – видалення дозволено
DELETE FROM Customers WHERE ID_CM = 2005;

SELECT ID_CM, NAME_CM
FROM Customers
WHERE ID_CM IN (2005, 2006)
ORDER BY ID_CM;
GO
IF NOT EXISTS (SELECT 1 FROM Customers WHERE ID_CM = 2005)
                INSERT INTO Customers (ID_CM, NAME_CM, CITY_CM, BONUS_CM)
                VALUES (2005, N'Шевченко', N'Чернігів', 150);
GO

-- 1.3. Створити тригер, що збільшує комісійні продавця на 0,01 під час додавання замовлення на суму понад 1000 грн.
DELETE FROM Orders WHERE ID_OD = 3013;
UPDATE Salespeople SET COMM_SP = 0.10 WHERE ID_SP = 1003;
GO
-- Замовлення на 1500 грн
INSERT INTO Orders (ID_OD, AMOUNT_OD, DATE_OD, ID_CM, ID_SP)
VALUES (3013, 1500.00, '2024-03-15', 2001, 1003);

SELECT o.ID_OD, o.AMOUNT_OD, s.NAME_SP, s.COMM_SP
FROM Orders o
JOIN Salespeople s ON s.ID_SP = o.ID_SP
WHERE o.ID_OD = 3013;
GO
DELETE FROM Orders WHERE ID_OD = 3013;
UPDATE Salespeople SET COMM_SP = 0.10 WHERE ID_SP = 1003;
GO

-- 1.4. Створити тригер, який забороняє додавати замовлення на суму, меншу за 10 грн.
-- Замовлення на 5 грн – заборонено
INSERT INTO Orders (ID_OD, AMOUNT_OD, DATE_OD, ID_CM, ID_SP)
VALUES (3014, 5.00, '2024-03-15', 2001, 1003);

SELECT COUNT(*) AS Кількість
FROM Orders
WHERE ID_OD = 3014;
GO
DELETE FROM Orders WHERE ID_OD IN (3013, 3014);
GO

-- Остаточне відновлення даних Sales111
DELETE FROM Orders WHERE ID_OD IN (3013, 3014);
UPDATE Salespeople SET COMM_SP = 0.10 WHERE ID_SP = 1003;
IF NOT EXISTS (SELECT 1 FROM Customers WHERE ID_CM = 2005)
    INSERT INTO Customers (ID_CM, NAME_CM, CITY_CM, BONUS_CM)
    VALUES (2005, N'Шевченко', N'Чернігів', 150);
GO

USE LibraryDB;
GO

-- Службова таблиця журналу змін
IF OBJECT_ID(N'dbo.AuditLog', N'U') IS NULL
CREATE TABLE dbo.AuditLog (
    id_log INT IDENTITY(1,1) PRIMARY KEY,
    table_name NVARCHAR(50) NOT NULL,
    action_name NVARCHAR(10) NOT NULL,
    row_id INT NULL,
    log_date DATETIME NOT NULL DEFAULT GETDATE(),
    details NVARCHAR(300) NULL
);
GO

-- Видалення тригерів, якщо вони вже існують
IF OBJECT_ID(N'dbo.trg_Author_Insert', N'TR') IS NOT NULL DROP TRIGGER dbo.trg_Author_Insert;
IF OBJECT_ID(N'dbo.trg_Author_Update', N'TR') IS NOT NULL DROP TRIGGER dbo.trg_Author_Update;
IF OBJECT_ID(N'dbo.trg_Author_Delete', N'TR') IS NOT NULL DROP TRIGGER dbo.trg_Author_Delete;
IF OBJECT_ID(N'dbo.trg_Publisher_Insert', N'TR') IS NOT NULL DROP TRIGGER dbo.trg_Publisher_Insert;
IF OBJECT_ID(N'dbo.trg_Publisher_Update', N'TR') IS NOT NULL DROP TRIGGER dbo.trg_Publisher_Update;
IF OBJECT_ID(N'dbo.trg_Publisher_Delete', N'TR') IS NOT NULL DROP TRIGGER dbo.trg_Publisher_Delete;
IF OBJECT_ID(N'dbo.trg_Book_Insert', N'TR') IS NOT NULL DROP TRIGGER dbo.trg_Book_Insert;
IF OBJECT_ID(N'dbo.trg_Book_Update', N'TR') IS NOT NULL DROP TRIGGER dbo.trg_Book_Update;
IF OBJECT_ID(N'dbo.trg_Book_Delete', N'TR') IS NOT NULL DROP TRIGGER dbo.trg_Book_Delete;
IF OBJECT_ID(N'dbo.trg_Reader_Insert', N'TR') IS NOT NULL DROP TRIGGER dbo.trg_Reader_Insert;
IF OBJECT_ID(N'dbo.trg_Reader_Update', N'TR') IS NOT NULL DROP TRIGGER dbo.trg_Reader_Update;
IF OBJECT_ID(N'dbo.trg_Reader_Delete', N'TR') IS NOT NULL DROP TRIGGER dbo.trg_Reader_Delete;
IF OBJECT_ID(N'dbo.trg_BookLoan_Insert', N'TR') IS NOT NULL DROP TRIGGER dbo.trg_BookLoan_Insert;
IF OBJECT_ID(N'dbo.trg_BookLoan_Update', N'TR') IS NOT NULL DROP TRIGGER dbo.trg_BookLoan_Update;
IF OBJECT_ID(N'dbo.trg_BookLoan_Delete', N'TR') IS NOT NULL DROP TRIGGER dbo.trg_BookLoan_Delete;
GO

CREATE TRIGGER trg_Author_Insert ON Author
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO AuditLog (table_name, action_name, row_id, details)
    SELECT N'Author', N'INSERT', id_author, N'Додано автора: ' + author_name
    FROM inserted;
END
GO
CREATE TRIGGER trg_Author_Update ON Author
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO AuditLog (table_name, action_name, row_id, details)
    SELECT N'Author', N'UPDATE', i.id_author, N'Країна: ' + ISNULL(d.country, N'—') + N' -> ' + ISNULL(i.country, N'—')
    FROM inserted i JOIN deleted d ON d.id_author = i.id_author;
END
GO
CREATE TRIGGER trg_Author_Delete ON Author
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO AuditLog (table_name, action_name, row_id, details)
    SELECT N'Author', N'DELETE', id_author, N'Видалено автора: ' + author_name
    FROM deleted;
END
GO

CREATE TRIGGER trg_Publisher_Insert ON Publisher
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO AuditLog (table_name, action_name, row_id, details)
    SELECT N'Publisher', N'INSERT', id_publisher, N'Додано видавництво: ' + publisher_name
    FROM inserted;
END
GO
CREATE TRIGGER trg_Publisher_Update ON Publisher
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO AuditLog (table_name, action_name, row_id, details)
    SELECT N'Publisher', N'UPDATE', i.id_publisher, N'Місто: ' + ISNULL(d.city, N'—') + N' -> ' + ISNULL(i.city, N'—')
    FROM inserted i JOIN deleted d ON d.id_publisher = i.id_publisher;
END
GO
CREATE TRIGGER trg_Publisher_Delete ON Publisher
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO AuditLog (table_name, action_name, row_id, details)
    SELECT N'Publisher', N'DELETE', id_publisher, N'Видалено видавництво: ' + publisher_name
    FROM deleted;
END
GO

CREATE TRIGGER trg_Book_Insert ON Book
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO AuditLog (table_name, action_name, row_id, details)
    SELECT N'Book', N'INSERT', id_book, N'Додано книгу: ' + title
    FROM inserted;
END
GO
CREATE TRIGGER trg_Book_Update ON Book
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO AuditLog (table_name, action_name, row_id, details)
    SELECT N'Book', N'UPDATE', i.id_book, N'Ціна: ' + CAST(d.price AS NVARCHAR(20)) + N' -> ' + CAST(i.price AS NVARCHAR(20))
    FROM inserted i JOIN deleted d ON d.id_book = i.id_book;
END
GO
CREATE TRIGGER trg_Book_Delete ON Book
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO AuditLog (table_name, action_name, row_id, details)
    SELECT N'Book', N'DELETE', id_book, N'Видалено книгу: ' + title
    FROM deleted;
END
GO

CREATE TRIGGER trg_Reader_Insert ON Reader
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO AuditLog (table_name, action_name, row_id, details)
    SELECT N'Reader', N'INSERT', id_reader, N'Додано читача: ' + reader_name
    FROM inserted;
END
GO
CREATE TRIGGER trg_Reader_Update ON Reader
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO AuditLog (table_name, action_name, row_id, details)
    SELECT N'Reader', N'UPDATE', i.id_reader, N'Телефон: ' + ISNULL(d.phone, N'—') + N' -> ' + ISNULL(i.phone, N'—')
    FROM inserted i JOIN deleted d ON d.id_reader = i.id_reader;
END
GO
CREATE TRIGGER trg_Reader_Delete ON Reader
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO AuditLog (table_name, action_name, row_id, details)
    SELECT N'Reader', N'DELETE', id_reader, N'Видалено читача: ' + reader_name
    FROM deleted;
END
GO

CREATE TRIGGER trg_BookLoan_Insert ON BookLoan
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO AuditLog (table_name, action_name, row_id, details)
    SELECT N'BookLoan', N'INSERT', id_loan, N'Видано книгу ' + CAST(id_book AS NVARCHAR(10)) + N' читачу ' + CAST(id_reader AS NVARCHAR(10))
    FROM inserted;
END
GO
CREATE TRIGGER trg_BookLoan_Update ON BookLoan
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO AuditLog (table_name, action_name, row_id, details)
    SELECT N'BookLoan', N'UPDATE', i.id_loan, N'Повернення: ' + ISNULL(CONVERT(NVARCHAR(10), d.return_date, 120), N'—') + N' -> ' + ISNULL(CONVERT(NVARCHAR(10), i.return_date, 120), N'—')
    FROM inserted i JOIN deleted d ON d.id_loan = i.id_loan;
END
GO
CREATE TRIGGER trg_BookLoan_Delete ON BookLoan
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO AuditLog (table_name, action_name, row_id, details)
    SELECT N'BookLoan', N'DELETE', id_loan, N'Видалено видачу книги ' + CAST(id_book AS NVARCHAR(10)) + N' читачу ' + CAST(id_reader AS NVARCHAR(10))
    FROM deleted;
END
GO

-- 2.1. Створити тригери вставки, редагування та видалення для таблиці Author.
DELETE FROM AuditLog;
GO
-- Демонстрація: INSERT, UPDATE, DELETE тестового рядка
INSERT INTO Author (author_name, country) VALUES (N'Тестовий автор', N'Україна');
UPDATE Author SET country = N'Польща' WHERE author_name = N'Тестовий автор';
DELETE FROM Author WHERE author_name = N'Тестовий автор';
SELECT table_name, action_name, row_id, details
FROM AuditLog WHERE table_name = N'Author' ORDER BY id_log;
GO
DELETE FROM Author WHERE author_name = N'Тестовий автор';
DBCC CHECKIDENT ('Author', RESEED, 10);
GO

-- 2.2. Створити тригери вставки, редагування та видалення для таблиці Publisher.
DELETE FROM AuditLog;
GO
-- Демонстрація: INSERT, UPDATE, DELETE тестового рядка
INSERT INTO Publisher (publisher_name, city) VALUES (N'Тестове видавництво', N'Київ');
UPDATE Publisher SET city = N'Львів' WHERE publisher_name = N'Тестове видавництво';
DELETE FROM Publisher WHERE publisher_name = N'Тестове видавництво';
SELECT table_name, action_name, row_id, details
FROM AuditLog WHERE table_name = N'Publisher' ORDER BY id_log;
GO
DELETE FROM Publisher WHERE publisher_name = N'Тестове видавництво';
DBCC CHECKIDENT ('Publisher', RESEED, 10);
GO

-- 2.3. Створити тригери вставки, редагування та видалення для таблиці Book.
DELETE FROM AuditLog;
GO
-- Демонстрація: INSERT, UPDATE, DELETE тестового рядка
INSERT INTO Book (title, id_author, id_publisher, year_pub, price) VALUES (N'Тестова книга', 1, 1, 2024, 100);
UPDATE Book SET price = 120 WHERE title = N'Тестова книга';
DELETE FROM Book WHERE title = N'Тестова книга';
SELECT table_name, action_name, row_id, details
FROM AuditLog WHERE table_name = N'Book' ORDER BY id_log;
GO
DELETE FROM Book WHERE title = N'Тестова книга';
DBCC CHECKIDENT ('Book', RESEED, 5);
GO

-- 2.4. Створити тригери вставки, редагування та видалення для таблиці Reader.
DELETE FROM AuditLog;
GO
-- Демонстрація: INSERT, UPDATE, DELETE тестового рядка
INSERT INTO Reader (reader_name, ticket_num, phone) VALUES (N'Тестовий читач', N'T-9999', N'+380000000000');
UPDATE Reader SET phone = N'+380501112233' WHERE ticket_num = N'T-9999';
DELETE FROM Reader WHERE ticket_num = N'T-9999';
SELECT table_name, action_name, row_id, details
FROM AuditLog WHERE table_name = N'Reader' ORDER BY id_log;
GO
DELETE FROM Reader WHERE ticket_num = N'T-9999';
DBCC CHECKIDENT ('Reader', RESEED, 6);
GO

-- 2.5. Створити тригери вставки, редагування та видалення для таблиці BookLoan.
DELETE FROM AuditLog;
GO
-- Демонстрація: INSERT, UPDATE, DELETE тестового рядка
INSERT INTO BookLoan (id_book, id_reader, loan_date) VALUES (1, 1, '2026-10-01');
UPDATE BookLoan SET return_date = '2026-10-05' WHERE id_book = 1 AND id_reader = 1 AND loan_date = '2026-10-01';
DELETE FROM BookLoan WHERE id_book = 1 AND id_reader = 1 AND loan_date = '2026-10-01';
SELECT table_name, action_name, row_id, details
FROM AuditLog WHERE table_name = N'BookLoan' ORDER BY id_log;
GO
DELETE FROM BookLoan WHERE id_book = 1 AND id_reader = 1 AND loan_date = '2026-10-01';
DBCC CHECKIDENT ('BookLoan', RESEED, 20);
GO

-- Остаточне відновлення даних LibraryDB
DELETE FROM BookLoan WHERE id_book = 1 AND id_reader = 1 AND loan_date = '2026-10-01';
DELETE FROM Book WHERE title = N'Тестова книга';
DELETE FROM Reader WHERE ticket_num = N'T-9999';
DELETE FROM Author WHERE author_name = N'Тестовий автор';
DELETE FROM Publisher WHERE publisher_name = N'Тестове видавництво';
DBCC CHECKIDENT ('Author', RESEED, 10);
DBCC CHECKIDENT ('Publisher', RESEED, 10);
DBCC CHECKIDENT ('Reader', RESEED, 6);
DBCC CHECKIDENT ('Book', RESEED, 5);
DBCC CHECKIDENT ('BookLoan', RESEED, 20);
DELETE FROM AuditLog;
GO