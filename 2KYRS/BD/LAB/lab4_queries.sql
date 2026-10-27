-- Лабораторна робота № 4. Створення вкладених запитів
-- MS SQL Server, бази даних Sales111 і LibraryDB

USE Sales111;
GO

-- Тестовий замовник без замовлень (для запитів 1.13 і 1.14)
IF NOT EXISTS (SELECT 1 FROM Customers WHERE ID_CM = 2005)
    INSERT INTO Customers (ID_CM, NAME_CM, CITY_CM, BONUS_CM)
    VALUES (2005, N'Шевченко', N'Чернігів', 150);
GO

-- 1.1. Визначити дати замовлень, що перевищили за сумою середнє значення, і вказати для цих замовлень перевищення над середнім рівнем.
SELECT DATE_OD AS Дата_замовлення,
       AMOUNT_OD AS Сума,
       CAST(AMOUNT_OD - (SELECT AVG(AMOUNT_OD) FROM Orders)
            AS DECIMAL(10, 2)) AS Перевищення
FROM Orders
WHERE AMOUNT_OD > (SELECT AVG(AMOUNT_OD) FROM Orders)
ORDER BY DATE_OD, AMOUNT_OD;
GO

-- 1.2. Визначити ім'я продавця, у якого було зроблено найбільше за сумою замовлення.
SELECT NAME_SP AS Продавець
FROM Salespeople
WHERE ID_SP = (SELECT TOP 1 ID_SP
               FROM Orders
               ORDER BY AMOUNT_OD DESC);
GO

-- 1.3. Визначити замовників, у замовленнях яких сума відрізняється від максимальної не більш ніж на 10 %.
SELECT c.ID_CM AS Код_замовника,
       c.NAME_CM AS Замовник,
       o.AMOUNT_OD AS Сума
FROM Orders o
JOIN Customers c ON c.ID_CM = o.ID_CM
WHERE o.AMOUNT_OD >= (SELECT MAX(AMOUNT_OD) FROM Orders) * 0.9;
GO

-- 1.4. Визначити дати, коли середня сума замовлення за день виявилася більшою за середній показник за всіма замовленнями взагалі.
SELECT DATE_OD AS Дата,
       COUNT(*) AS Кількість_замовлень,
       CAST(AVG(AMOUNT_OD) AS DECIMAL(10, 2)) AS Середня_за_день
FROM Orders
GROUP BY DATE_OD
HAVING AVG(AMOUNT_OD) > (SELECT AVG(AMOUNT_OD) FROM Orders)
ORDER BY DATE_OD;
GO

-- 1.5. Визначити список продавців, що будь-коли здійснювали продажі.
SELECT s.ID_SP AS Код_продавця,
       s.NAME_SP AS Продавець
FROM Salespeople s
WHERE EXISTS (SELECT 1
              FROM Orders o
              WHERE o.ID_SP = s.ID_SP);
GO

-- 1.6. Визначити замовлення, які зробили замовники, що мають рейтинг вище середнього.
SELECT o.ID_OD AS Код_замовлення,
       o.DATE_OD AS Дата,
       o.AMOUNT_OD AS Сума,
       o.ID_CM AS Код_замовника
FROM Orders o
WHERE o.ID_CM IN (SELECT ID_CM
                  FROM Customers
                  WHERE BONUS_CM > (SELECT AVG(BONUS_CM) FROM Customers))
ORDER BY o.DATE_OD, o.ID_OD;
GO

-- 1.7. Визначити продавців, замовлення у яких здійснюють тільки замовники, що мають рейтинг вище середнього, і ніхто інший.
SELECT s.ID_SP AS Код_продавця,
       s.NAME_SP AS Продавець
FROM Salespeople s
WHERE EXISTS (SELECT 1
              FROM Orders o
              WHERE o.ID_SP = s.ID_SP)
  AND NOT EXISTS (SELECT 1
                  FROM Orders o
                  JOIN Customers c ON c.ID_CM = o.ID_CM
                  WHERE o.ID_SP = s.ID_SP
                    AND c.BONUS_CM <= (SELECT AVG(BONUS_CM) FROM Customers));
GO

-- 1.8. Визначити продавців, замовлення у яких жодного разу не зробили замовники, що мають рейтинг нижче середнього.
SELECT s.ID_SP AS Код_продавця,
       s.NAME_SP AS Продавець
FROM Salespeople s
WHERE NOT EXISTS (SELECT 1
                  FROM Orders o
                  JOIN Customers c ON c.ID_CM = o.ID_CM
                  WHERE o.ID_SP = s.ID_SP
                    AND c.BONUS_CM < (SELECT AVG(BONUS_CM) FROM Customers));
GO

-- 1.9. Визначити дати оформлення замовлень, сума яких перевищує 500 грн, замовниками, що мають рейтинг вище середнього.
SELECT o.DATE_OD AS Дата,
       o.AMOUNT_OD AS Сума,
       o.ID_CM AS Код_замовника
FROM Orders o
WHERE o.AMOUNT_OD > 500
  AND o.ID_CM IN (SELECT ID_CM
                  FROM Customers
                  WHERE BONUS_CM > (SELECT AVG(BONUS_CM) FROM Customers))
ORDER BY o.DATE_OD;
GO

-- 1.10. Визначити замовників, що виконали замовлення останніми.
SELECT c.ID_CM AS Код_замовника,
       c.NAME_CM AS Замовник,
       o.DATE_OD AS Дата
FROM Orders o
JOIN Customers c ON c.ID_CM = o.ID_CM
WHERE o.DATE_OD = (SELECT MAX(DATE_OD) FROM Orders);
GO

-- 1.11. Знайти замовника, який зробив замовлення на найбільшу суму.
SELECT c.ID_CM AS Код_замовника,
       c.NAME_CM AS Замовник,
       o.AMOUNT_OD AS Сума
FROM Orders o
JOIN Customers c ON c.ID_CM = o.ID_CM
WHERE o.AMOUNT_OD = (SELECT MAX(AMOUNT_OD) FROM Orders);
GO

-- 1.12. Знайти замовників, сума в замовленнях яких перевищує такий самий показник хоча б в одному замовленні тих замовників, що мають рейтинг вище середнього.
SELECT DISTINCT c.ID_CM AS Код_замовника,
       c.NAME_CM AS Замовник
FROM Orders o
JOIN Customers c ON c.ID_CM = o.ID_CM
WHERE o.AMOUNT_OD > ANY (SELECT o2.AMOUNT_OD
                         FROM Orders o2
                         WHERE o2.ID_CM IN (SELECT ID_CM
                                            FROM Customers
                                            WHERE BONUS_CM > (SELECT AVG(BONUS_CM)
                                                              FROM Customers)));
GO

-- 1.13. Визначити замовників, що зробили хоча б одне замовлення.
SELECT c.ID_CM AS Код_замовника,
       c.NAME_CM AS Замовник
FROM Customers c
WHERE EXISTS (SELECT 1
              FROM Orders o
              WHERE o.ID_CM = c.ID_CM)
ORDER BY c.ID_CM;
GO

-- 1.14. Визначити замовників, які ще не зробили жодного замовлення.
SELECT c.ID_CM AS Код_замовника,
       c.NAME_CM AS Замовник,
       c.CITY_CM AS Місто,
       c.BONUS_CM AS Рейтинг
FROM Customers c
WHERE NOT EXISTS (SELECT 1
                  FROM Orders o
                  WHERE o.ID_CM = c.ID_CM);
GO

-- 1.15. Визначити продавців, що виконали найбільше за сумою замовлення, серед продавців з рейтингом нижче середнього.
SELECT s.ID_SP AS Код_продавця,
       s.NAME_SP AS Продавець,
       SUM(o.AMOUNT_OD) AS Сума_замовлень
FROM Salespeople s
JOIN Orders o ON o.ID_SP = s.ID_SP
WHERE s.COMM_SP < (SELECT AVG(COMM_SP) FROM Salespeople)
GROUP BY s.ID_SP, s.NAME_SP
HAVING SUM(o.AMOUNT_OD) = (
    SELECT MAX(t.Сума)
    FROM (SELECT SUM(o2.AMOUNT_OD) AS Сума
          FROM Orders o2
          JOIN Salespeople s2 ON s2.ID_SP = o2.ID_SP
          WHERE s2.COMM_SP < (SELECT AVG(COMM_SP)
                              FROM Salespeople)
          GROUP BY s2.ID_SP) AS t);
GO

USE LibraryDB;
GO

-- 2.1. Книги, ціна яких перевищує середню ціну всіх книг (скалярний підзапит)
SELECT title AS Назва,
       price AS Ціна,
       CAST(price - (SELECT AVG(price) FROM Book) AS DECIMAL(10, 2)) AS Перевищення
FROM Book
WHERE price > (SELECT AVG(price) FROM Book)
ORDER BY price DESC;
GO

-- 2.2. Книги з максимальною ціною (скалярний підзапит з MAX)
SELECT id_book AS Код_книги,
       title AS Назва,
       price AS Ціна
FROM Book
WHERE price = (SELECT MAX(price) FROM Book);
GO

-- 2.3. Книги, ціна яких перевищує середню ціну книг свого видавництва (корельований підзапит)
SELECT b.title AS Назва,
       b.price AS Ціна,
       p.publisher_name AS Видавництво,
       CAST((SELECT AVG(b2.price)
             FROM Book b2
             WHERE b2.id_publisher = b.id_publisher)
            AS DECIMAL(10, 2)) AS Сер_ціна_вид
FROM Book b
JOIN Publisher p ON p.id_publisher = b.id_publisher
WHERE b.price > (SELECT AVG(b2.price)
                 FROM Book b2
                 WHERE b2.id_publisher = b.id_publisher)
ORDER BY p.publisher_name;
GO

-- 2.4. Книги, ціна яких перевищує ціну кожної книги видавництва «А-БА-БА-ГА-ЛА-МА-ГА» (ALL)
SELECT b.title AS Назва,
       b.price AS Ціна
FROM Book b
WHERE b.price > ALL (SELECT b2.price
                     FROM Book b2
                     JOIN Publisher p ON p.id_publisher = b2.id_publisher
                     WHERE p.publisher_name = N'А-БА-БА-ГА-ЛА-МА-ГА');
GO

-- 2.5. Книги, ціна яких перевищує ціну хоча б однієї книги видавництва «КСД» (ANY)
SELECT b.title AS Назва,
       b.price AS Ціна
FROM Book b
WHERE b.price > ANY (SELECT b2.price
                     FROM Book b2
                     JOIN Publisher p ON p.id_publisher = b2.id_publisher
                     WHERE p.publisher_name = N'КСД');
GO

-- 2.6. Автори, які мають хоча б одну книгу у фонді (EXISTS)
SELECT a.id_author AS Код_автора,
       a.author_name AS Автор,
       a.country AS Країна
FROM Author a
WHERE EXISTS (SELECT 1
              FROM Book b
              WHERE b.id_author = a.id_author)
ORDER BY a.author_name;
GO

-- 2.7. Автори, які не мають книг, виданих після 2021 року (NOT EXISTS)
SELECT a.id_author AS Код_автора,
       a.author_name AS Автор
FROM Author a
WHERE NOT EXISTS (SELECT 1
                  FROM Book b
                  WHERE b.id_author = a.id_author
                    AND b.year_pub > 2021)
ORDER BY a.author_name;
GO

-- 2.8. Книги, які не видавалися читачеві з кодом 5 (NOT IN)
SELECT b.id_book AS Код_книги,
       b.title AS Назва,
       b.price AS Ціна
FROM Book b
WHERE b.id_book NOT IN (SELECT l.id_book
                        FROM BookLoan l
                        WHERE l.id_reader = 5)
ORDER BY b.id_book;
GO
