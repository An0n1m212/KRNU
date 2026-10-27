-- Лабораторна робота № 6. Представлення
-- MS SQL Server, бази даних Sales111 і LibraryDB

USE Sales111;
GO

-- Видалення представлень, якщо вони вже існують
IF OBJECT_ID(N'dbo.TopCustomers', N'V') IS NOT NULL DROP VIEW dbo.TopCustomers;
IF OBJECT_ID(N'dbo.SellersByCity', N'V') IS NOT NULL DROP VIEW dbo.SellersByCity;
IF OBJECT_ID(N'dbo.Commissions', N'V') IS NOT NULL DROP VIEW dbo.Commissions;
IF OBJECT_ID(N'dbo.OrdersWithNames', N'V') IS NOT NULL DROP VIEW dbo.OrdersWithNames;
IF OBJECT_ID(N'dbo.AboveAvgOrders', N'V') IS NOT NULL DROP VIEW dbo.AboveAvgOrders;
GO

-- 1.1. Створити представлення, яке показує всіх замовників з найвищими рейтингами.
CREATE VIEW TopCustomers AS
SELECT ID_CM AS Код_замовника,
       NAME_CM AS Замовник,
       CITY_CM AS Місто,
       BONUS_CM AS Рейтинг
FROM Customers
WHERE BONUS_CM = (SELECT MAX(BONUS_CM) FROM Customers);
GO

SELECT * FROM TopCustomers;
GO

-- 1.2. Створити представлення, яке показує кількість продавців у кожному місті.
CREATE VIEW SellersByCity AS
SELECT CITY_SP AS Місто,
       COUNT(*) AS Кількість_продавців
FROM Salespeople
GROUP BY CITY_SP;
GO

SELECT * FROM SellersByCity;
GO

-- 1.3.1. Створити представлення Commissions (Комісійні), яке включає тільки поля COMM_SP і ID_SP і дозволяє змінювати комісійні в межах від 0,10 до 0,20.
CREATE VIEW Commissions AS
SELECT ID_SP, COMM_SP
FROM Salespeople
WHERE COMM_SP BETWEEN 0.10 AND 0.20
WITH CHECK OPTION;
GO

SELECT * FROM Commissions;
GO

-- 1.3.2. Змінити комісійні продавця 1003 на 0,14 через представлення Commissions.
UPDATE Commissions
SET COMM_SP = 0.14
WHERE ID_SP = 1003;

SELECT * FROM Commissions;
GO

-- 1.3.3. Перевірити дію WITH CHECK OPTION: спроба встановити комісійні 0,25.
UPDATE Commissions
SET COMM_SP = 0.25
WHERE ID_SP = 1003;
GO

-- 1.4. Створити представлення з прізвищами замовників і продавців для кожного замовлення та вивести замовлення клієнта Гора.
CREATE VIEW OrdersWithNames AS
SELECT o.ID_OD AS Код_замовлення,
       c.NAME_CM AS Замовник,
       s.NAME_SP AS Продавець,
       o.AMOUNT_OD AS Сума,
       o.DATE_OD AS Дата
FROM Orders o
JOIN Customers c ON c.ID_CM = o.ID_CM
JOIN Salespeople s ON s.ID_SP = o.ID_SP;
GO

SELECT * FROM OrdersWithNames
WHERE Замовник = N'Гора';
GO

-- 1.5. Створити представлення, яке показує всі замовлення із сумами, вищими за середню.
CREATE VIEW AboveAvgOrders AS
SELECT ID_OD AS Код_замовлення,
       DATE_OD AS Дата,
       AMOUNT_OD AS Сума,
       ID_CM AS Код_замовника,
       ID_SP AS Код_продавця
FROM Orders
WHERE AMOUNT_OD > (SELECT AVG(AMOUNT_OD) FROM Orders);
GO

SELECT * FROM AboveAvgOrders
ORDER BY Сума DESC;
GO

-- Повернення початкового значення комісійних після демонстрації
UPDATE Salespeople SET COMM_SP = 0.10 WHERE ID_SP = 1003;
GO

USE LibraryDB;
GO

-- Видалення представлень, якщо вони вже існують
IF OBJECT_ID(N'dbo.BooksCheap', N'V') IS NOT NULL DROP VIEW dbo.BooksCheap;
IF OBJECT_ID(N'dbo.BooksModern', N'V') IS NOT NULL DROP VIEW dbo.BooksModern;
IF OBJECT_ID(N'dbo.LoansReturned', N'V') IS NOT NULL DROP VIEW dbo.LoansReturned;
IF OBJECT_ID(N'dbo.LoansOpen', N'V') IS NOT NULL DROP VIEW dbo.LoansOpen;
GO

-- 2.1. Створити модифіковане представлення BooksCheap для книг ціною менше 400 грн.
CREATE VIEW BooksCheap AS
SELECT id_book, title, id_author, id_publisher, year_pub, price
FROM Book
WHERE price < 400
WITH CHECK OPTION;
GO

SELECT * FROM BooksCheap;
GO

-- 2.2. Створити модифіковане представлення BooksModern для книг, виданих починаючи з 2020 року.
CREATE VIEW BooksModern AS
SELECT id_book, title, id_author, id_publisher, year_pub, price
FROM Book
WHERE year_pub >= 2020
WITH CHECK OPTION;
GO

SELECT * FROM BooksModern;
GO

-- 2.3. Створити модифіковане представлення LoansReturned для повернутих книг.
CREATE VIEW LoansReturned AS
SELECT id_loan, id_book, id_reader, loan_date, return_date
FROM BookLoan
WHERE return_date IS NOT NULL;
GO

SELECT * FROM LoansReturned;
GO

-- 2.4. Створити модифіковане представлення LoansOpen для неповернутих книг.
CREATE VIEW LoansOpen AS
SELECT id_loan, id_book, id_reader, loan_date, return_date
FROM BookLoan
WHERE return_date IS NULL
WITH CHECK OPTION;
GO

SELECT * FROM LoansOpen;
GO

-- 2.5. Додати нову книгу через представлення BooksCheap (команда INSERT).
INSERT INTO BooksCheap (title, id_author, id_publisher, year_pub, price)
VALUES (N'Тіні забутих предків', 2, 2, 2023, 300);

SELECT * FROM BooksCheap;
GO

-- 2.6. Змінити дату повернення книги через представлення LoansReturned (команда UPDATE).
UPDATE LoansReturned
SET return_date = '2026-09-16'
WHERE id_loan = 1;

SELECT * FROM LoansReturned;
GO

-- 2.7. Перевірити дію WITH CHECK OPTION у представленні LoansOpen.
UPDATE LoansOpen
SET return_date = '2026-10-01'
WHERE id_loan = 4;
GO

-- 2.8. Видалити видачу через представлення LoansOpen (команда DELETE).
DELETE FROM LoansOpen
WHERE id_loan = 2;

SELECT * FROM LoansOpen;
GO

-- Відновлення даних після демонстрації команд INSERT, UPDATE і DELETE
DELETE FROM Book WHERE id_book > 5 AND title = N'Тіні забутих предків';
DBCC CHECKIDENT ('Book', RESEED, 5);
UPDATE BookLoan SET return_date = '2026-09-15' WHERE id_loan = 1;
IF NOT EXISTS (SELECT 1 FROM BookLoan WHERE id_loan = 2)
BEGIN
    SET IDENTITY_INSERT BookLoan ON;
    INSERT INTO BookLoan (id_loan, id_book, id_reader, loan_date, return_date)
    VALUES (2, 2, 1, '2026-09-02', NULL);
    SET IDENTITY_INSERT BookLoan OFF;
END;
GO