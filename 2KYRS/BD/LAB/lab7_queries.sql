-- Лабораторна робота № 7. Збережені процедури
-- MS SQL Server, бази даних Sales111 і LibraryDB

USE Sales111;
GO

-- Видалення процедур, якщо вони вже існують
IF OBJECT_ID(N'dbo.ChangeOrderDate', N'P') IS NOT NULL DROP PROCEDURE dbo.ChangeOrderDate;
IF OBJECT_ID(N'dbo.DeleteCustomerOrders', N'P') IS NOT NULL DROP PROCEDURE dbo.DeleteCustomerOrders;
IF OBJECT_ID(N'dbo.IncreaseKyivBonus', N'P') IS NOT NULL DROP PROCEDURE dbo.IncreaseKyivBonus;
IF OBJECT_ID(N'dbo.ReassignSellerOrders', N'P') IS NOT NULL DROP PROCEDURE dbo.ReassignSellerOrders;
IF OBJECT_ID(N'dbo.DeleteCustomersWithoutOrders', N'P') IS NOT NULL DROP PROCEDURE dbo.DeleteCustomersWithoutOrders;
IF OBJECT_ID(N'dbo.DeleteSellersWithoutOrders', N'P') IS NOT NULL DROP PROCEDURE dbo.DeleteSellersWithoutOrders;
IF OBJECT_ID(N'dbo.IncreaseTopSellerCommission', N'P') IS NOT NULL DROP PROCEDURE dbo.IncreaseTopSellerCommission;
IF OBJECT_ID(N'dbo.LongestCustomerName', N'P') IS NOT NULL DROP PROCEDURE dbo.LongestCustomerName;
GO

-- 1.1. Створити процедуру для зміни дати оформлення замовлень за 10.03.2024 на 11.03.2024 для всіх рядків таблиці Orders.
IF NOT EXISTS (SELECT 1 FROM Orders WHERE ID_OD = 3012)
                INSERT INTO Orders (ID_OD, AMOUNT_OD, DATE_OD, ID_CM, ID_SP)
                VALUES (3012, 1250.00, '2024-03-10', 2006, 1001);
GO
CREATE PROCEDURE ChangeOrderDate
AS
BEGIN
    UPDATE Orders
    SET DATE_OD = '2024-03-11'
    WHERE DATE_OD = '2024-03-10';
END;
GO

EXEC ChangeOrderDate;

SELECT ID_OD, DATE_OD
FROM Orders
WHERE ID_OD = 3012;
GO
DELETE FROM Orders WHERE ID_OD = 3012;
GO

-- 1.2. Створити процедуру для видалення всіх замовлень замовника Клименко з таблиці Orders.
CREATE PROCEDURE DeleteCustomerOrders
    @customer NVARCHAR(50) = N'Клименко'
AS
BEGIN
    DELETE FROM Orders
    WHERE ID_CM = (SELECT ID_CM FROM Customers
                   WHERE NAME_CM = @customer);
END;
GO

EXEC DeleteCustomerOrders @customer = N'Клименко';

SELECT COUNT(*) AS Залишилося_замовлень
FROM Orders
WHERE ID_CM = 2006;
GO
IF NOT EXISTS (SELECT 1 FROM Orders WHERE ID_OD = 3008)
                INSERT INTO Orders (ID_OD, AMOUNT_OD, DATE_OD, ID_CM, ID_SP)
                VALUES (3008, 4723.00, '2024-10-05', 2006, 1001);
                IF NOT EXISTS (SELECT 1 FROM Orders WHERE ID_OD = 3011)
                INSERT INTO Orders (ID_OD, AMOUNT_OD, DATE_OD, ID_CM, ID_SP)
                VALUES (3011, 9891.88, '1990-06-10', 2006, 1001);
GO

-- 1.3. Створити процедуру для збільшення бонусного балу всіх замовників у Києві на 100.
CREATE PROCEDURE IncreaseKyivBonus
    @amount INT = 100
AS
BEGIN
    UPDATE Customers
    SET BONUS_CM = BONUS_CM + @amount
    WHERE CITY_CM = N'Київ';
END;
GO

EXEC IncreaseKyivBonus @amount = 100;

SELECT ID_CM, NAME_CM, CITY_CM, BONUS_CM
FROM Customers
WHERE CITY_CM = N'Київ'
ORDER BY ID_CM;
GO
UPDATE Customers SET BONUS_CM = 100 WHERE ID_CM IN (2001, 2006);
GO

-- 1.4. Створити процедуру для перепризначення замовлень одного продавця (Чепуренко) іншому продавцеві (Маренич).
CREATE PROCEDURE ReassignSellerOrders
    @from NVARCHAR(50),
    @to NVARCHAR(50)
AS
BEGIN
    DECLARE @from_id INT, @to_id INT;

    SELECT @from_id = ID_SP FROM Salespeople WHERE NAME_SP = @from;
    SELECT @to_id = ID_SP FROM Salespeople WHERE NAME_SP = @to;

    IF @from_id IS NULL OR @to_id IS NULL
    BEGIN
        RAISERROR(N'Продавця не знайдено', 16, 1);
        RETURN;
    END;

    UPDATE Orders
    SET ID_SP = @to_id
    WHERE ID_SP = @from_id;
END;
GO

EXEC ReassignSellerOrders @from = N'Чепуренко', @to = N'Маренич';

SELECT o.ID_OD, c.NAME_CM AS Замовник,
       s.NAME_SP AS Продавець, o.AMOUNT_OD AS Сума
FROM Orders o
JOIN Customers c ON c.ID_CM = o.ID_CM
JOIN Salespeople s ON s.ID_SP = o.ID_SP
WHERE o.ID_OD IN (3002, 3005, 3007, 3010)
ORDER BY o.ID_OD;
GO
UPDATE Orders SET ID_SP = 1002 WHERE ID_OD IN (3005, 3007, 3010);
GO

-- 1.5. Створити процедуру для видалення всіх замовників, що не мають поточних замовлень.
IF NOT EXISTS (SELECT 1 FROM Customers WHERE ID_CM = 2005)
                INSERT INTO Customers (ID_CM, NAME_CM, CITY_CM, BONUS_CM)
                VALUES (2005, N'Шевченко', N'Чернігів', 150);
GO
CREATE PROCEDURE DeleteCustomersWithoutOrders
AS
BEGIN
    DELETE FROM Customers
    WHERE NOT EXISTS (
        SELECT 1 FROM Orders o
        WHERE o.ID_CM = Customers.ID_CM
    );
END;
GO

EXEC DeleteCustomersWithoutOrders;

SELECT COUNT(*) AS Залишилося
FROM Customers
WHERE ID_CM = 2005;
GO
IF NOT EXISTS (SELECT 1 FROM Customers WHERE ID_CM = 2005)
                INSERT INTO Customers (ID_CM, NAME_CM, CITY_CM, BONUS_CM)
                VALUES (2005, N'Шевченко', N'Чернігів', 150);
GO

-- 1.6. Створити процедуру для видалення всіх продавців, що не мають поточних замовлень.
IF NOT EXISTS (SELECT 1 FROM Salespeople WHERE ID_SP = 1005)
                INSERT INTO Salespeople (ID_SP, NAME_SP, CITY_SP, COMM_SP)
                VALUES (1005, N'Ковальчук', N'Полтава', 0.09);
GO
CREATE PROCEDURE DeleteSellersWithoutOrders
AS
BEGIN
    DELETE FROM Salespeople
    WHERE NOT EXISTS (
        SELECT 1 FROM Orders o
        WHERE o.ID_SP = Salespeople.ID_SP
    );
END;
GO

EXEC DeleteSellersWithoutOrders;

SELECT COUNT(*) AS Залишилося
FROM Salespeople
WHERE ID_SP = 1005;
GO
IF NOT EXISTS (SELECT 1 FROM Salespeople WHERE ID_SP = 1005)
                INSERT INTO Salespeople (ID_SP, NAME_SP, CITY_SP, COMM_SP)
                VALUES (1005, N'Ковальчук', N'Полтава', 0.09);
GO

-- 1.7. Створити процедуру для збільшення на 20% комісійних продавців, сума поточних замовлень яких перевищує 3000.
CREATE PROCEDURE IncreaseTopSellerCommission
    @min_total MONEY = 3000,
    @percent DECIMAL(5, 2) = 20
AS
BEGIN
    UPDATE Salespeople
    SET COMM_SP = COMM_SP * (1 + @percent / 100)
    WHERE ID_SP IN (
        SELECT ID_SP
        FROM Orders
        GROUP BY ID_SP
        HAVING SUM(AMOUNT_OD) > @min_total
    );
END;
GO

EXEC IncreaseTopSellerCommission @min_total = 3000, @percent = 20;

SELECT s.ID_SP, s.NAME_SP,
       SUM(o.AMOUNT_OD) AS Сума_замовлень,
       s.COMM_SP
FROM Salespeople s
JOIN Orders o ON o.ID_SP = s.ID_SP
GROUP BY s.ID_SP, s.NAME_SP, s.COMM_SP
HAVING SUM(o.AMOUNT_OD) > 3000
ORDER BY s.ID_SP;
GO
UPDATE Salespeople SET COMM_SP = 0.12 WHERE ID_SP = 1001;
UPDATE Salespeople SET COMM_SP = 0.13 WHERE ID_SP = 1002;
GO

-- 1.8. Створити процедуру для визначення найдовшого прізвища замовника, що починається із вказаної літери.
CREATE PROCEDURE LongestCustomerName
    @letter NCHAR(1)
AS
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM Customers WHERE NAME_CM LIKE @letter + N'%'
    )
    BEGIN
        RAISERROR(N'Замовників із такої літери не знайдено', 16, 1);
        RETURN;
    END;

    SELECT TOP 1 NAME_CM AS Найдовше_прізвище
    FROM Customers
    WHERE NAME_CM LIKE @letter + N'%'
    ORDER BY LEN(NAME_CM) DESC, NAME_CM;
END;
GO

EXEC LongestCustomerName @letter = N'К';
GO
GO

-- Остаточне відновлення даних Sales111
DELETE FROM Orders WHERE ID_OD = 3012;
IF NOT EXISTS (SELECT 1 FROM Orders WHERE ID_OD = 3008)
    INSERT INTO Orders (ID_OD, AMOUNT_OD, DATE_OD, ID_CM, ID_SP)
    VALUES (3008, 4723.00, '2024-10-05', 2006, 1001);
IF NOT EXISTS (SELECT 1 FROM Orders WHERE ID_OD = 3011)
    INSERT INTO Orders (ID_OD, AMOUNT_OD, DATE_OD, ID_CM, ID_SP)
    VALUES (3011, 9891.88, '1990-06-10', 2006, 1001);
UPDATE Orders SET ID_SP = 1002 WHERE ID_OD IN (3005, 3007, 3010);
UPDATE Customers SET BONUS_CM = 100 WHERE ID_CM IN (2001, 2006);
IF NOT EXISTS (SELECT 1 FROM Customers WHERE ID_CM = 2005)
    INSERT INTO Customers (ID_CM, NAME_CM, CITY_CM, BONUS_CM)
    VALUES (2005, N'Шевченко', N'Чернігів', 150);
IF NOT EXISTS (SELECT 1 FROM Salespeople WHERE ID_SP = 1005)
    INSERT INTO Salespeople (ID_SP, NAME_SP, CITY_SP, COMM_SP)
    VALUES (1005, N'Ковальчук', N'Полтава', 0.09);
UPDATE Salespeople SET COMM_SP = 0.12 WHERE ID_SP = 1001;
UPDATE Salespeople SET COMM_SP = 0.13 WHERE ID_SP = 1002;
GO

USE LibraryDB;
GO

-- Видалення процедур, якщо вони вже існують
IF OBJECT_ID(N'dbo.AddBook', N'P') IS NOT NULL DROP PROCEDURE dbo.AddBook;
IF OBJECT_ID(N'dbo.ReturnBook', N'P') IS NOT NULL DROP PROCEDURE dbo.ReturnBook;
IF OBJECT_ID(N'dbo.ReaderLoanReport', N'P') IS NOT NULL DROP PROCEDURE dbo.ReaderLoanReport;
GO

-- 2.1. Створити процедуру AddBook для додавання нової книги з перевіркою вхідних даних.
DELETE FROM Book WHERE title = N'Тіні забутих предків' AND id_book > 5;
DBCC CHECKIDENT ('Book', RESEED, 5);
GO
CREATE PROCEDURE AddBook
    @title NVARCHAR(100),
    @author INT,
    @publisher INT,
    @year INT,
    @price MONEY
AS
BEGIN
    IF @price <= 0
    BEGIN
        RAISERROR(N'Ціна книги повинна бути додатною', 16, 1);
        RETURN;
    END;

    IF NOT EXISTS (SELECT 1 FROM Author WHERE id_author = @author)
    BEGIN
        RAISERROR(N'Автора з таким кодом не існує', 16, 1);
        RETURN;
    END;

    IF NOT EXISTS (SELECT 1 FROM Publisher WHERE id_publisher = @publisher)
    BEGIN
        RAISERROR(N'Видавництва з таким кодом не існує', 16, 1);
        RETURN;
    END;

    INSERT INTO Book (title, id_author, id_publisher, year_pub, price)
    VALUES (@title, @author, @publisher, @year, @price);
END;
GO

EXEC AddBook @title = N'Тіні забутих предків', @author = 2,
             @publisher = 2, @year = 2023, @price = 300;

SELECT * FROM Book WHERE title = N'Тіні забутих предків';
GO
DELETE FROM Book WHERE title = N'Тіні забутих предків' AND id_book > 5;
DBCC CHECKIDENT ('Book', RESEED, 5);
GO

-- 2.2. Створити процедуру ReturnBook для повернення книги за номером видачі.
UPDATE BookLoan SET return_date = NULL WHERE id_loan = 2;
GO
CREATE PROCEDURE ReturnBook
    @loan INT,
    @date DATE = NULL
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM BookLoan WHERE id_loan = @loan)
    BEGIN
        RAISERROR(N'Видачу з таким номером не знайдено', 16, 1);
        RETURN;
    END;

    IF EXISTS (SELECT 1 FROM BookLoan
               WHERE id_loan = @loan AND return_date IS NOT NULL)
    BEGIN
        RAISERROR(N'Книга за цією видачею вже повернута', 16, 1);
        RETURN;
    END;

    IF @date IS NULL
        SET @date = CAST(GETDATE() AS DATE);

    UPDATE BookLoan
    SET return_date = @date
    WHERE id_loan = @loan;
END;
GO

EXEC ReturnBook @loan = 2, @date = '2026-09-30';

SELECT * FROM BookLoan WHERE id_loan = 2;
GO
UPDATE BookLoan SET return_date = NULL WHERE id_loan = 2;
GO

-- 2.3. Створити процедуру ReaderLoanReport, яка повертає кількість видач і повернутих книг читача через вихідні параметри.
CREATE PROCEDURE ReaderLoanReport
    @reader INT,
    @total INT OUTPUT,
    @returned INT OUTPUT
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM Reader WHERE id_reader = @reader)
    BEGIN
        RAISERROR(N'Читача з таким кодом не знайдено', 16, 1);
        RETURN;
    END;

    SELECT @total = COUNT(*),
           @returned = SUM(CASE WHEN return_date IS NOT NULL THEN 1 ELSE 0 END)
    FROM BookLoan
    WHERE id_reader = @reader;
END;
GO

DECLARE @total INT, @returned INT;
EXEC ReaderLoanReport @reader = 1,
     @total = @total OUTPUT, @returned = @returned OUTPUT;
SELECT @total AS Усього_видач, @returned AS Повернуто_книг;
GO
GO

-- Остаточне відновлення даних LibraryDB
DELETE FROM Book WHERE title = N'Тіні забутих предків' AND id_book > 5;
DBCC CHECKIDENT ('Book', RESEED, 5);
UPDATE BookLoan SET return_date = NULL WHERE id_loan = 2;
GO