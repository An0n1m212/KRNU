-- Лабораторна робота № 5. Створення складних запитів
-- MS SQL Server, бази даних Sales111 і LibraryDB

USE Sales111;
GO

-- Тестові записи без замовлень (для запитів 1.1 і 1.17)
IF NOT EXISTS (SELECT 1 FROM Customers WHERE ID_CM = 2005)
    INSERT INTO Customers (ID_CM, NAME_CM, CITY_CM, BONUS_CM)
    VALUES (2005, N'Шевченко', N'Чернігів', 150);

IF NOT EXISTS (SELECT 1 FROM Salespeople WHERE ID_SP = 1005)
    INSERT INTO Salespeople (ID_SP, NAME_SP, CITY_SP, COMM_SP)
    VALUES (1005, N'Ковальчук', N'Полтава', 0.09);
GO

-- 1.1. Вивести номери та прізвища продавців, які не виконали жодного замовлення.
SELECT s.ID_SP AS Код_продавця,
       s.NAME_SP AS Продавець
FROM Salespeople s
LEFT JOIN Orders o ON o.ID_SP = s.ID_SP
WHERE o.ID_OD IS NULL;
GO

-- 1.2. Вивести номери та прізвища замовників, які зробили хоча б одне замовлення.
SELECT DISTINCT c.ID_CM AS Код_замовника,
       c.NAME_CM AS Замовник
FROM Customers c
JOIN Orders o ON o.ID_CM = c.ID_CM
ORDER BY c.ID_CM;
GO

-- 1.3. Вивести в один список прізвища клієнтів та продавців, які мешкають у Лондоні, із зазначенням статусу – продавець чи замовник.
-- Основний запит: мешканці Лондона
SELECT NAME_SP AS Прізвище,
       N'продавець' AS Статус,
       CITY_SP AS Місто
FROM Salespeople
WHERE CITY_SP = N'Лондон'
UNION
SELECT NAME_CM,
       N'замовник',
       CITY_CM
FROM Customers
WHERE CITY_CM = N'Лондон'
ORDER BY Прізвище;

-- Адаптований запит: мешканці Києва
SELECT NAME_SP AS Прізвище,
       N'продавець' AS Статус,
       CITY_SP AS Місто
FROM Salespeople
WHERE CITY_SP = N'Київ'
UNION
SELECT NAME_CM,
       N'замовник',
       CITY_CM
FROM Customers
WHERE CITY_CM = N'Київ'
ORDER BY Прізвище;
GO

-- 1.4. Вивести інформацію про замовників, у яких суми замовлень більші або дорівнюють середньому значенню суми замовлення для їх міста.
SELECT c.ID_CM AS Код_замовника,
       c.NAME_CM AS Замовник,
       c.CITY_CM AS Місто,
       o.AMOUNT_OD AS Сума
FROM Orders o
JOIN Customers c ON c.ID_CM = o.ID_CM
WHERE o.AMOUNT_OD >= (SELECT AVG(o2.AMOUNT_OD)
                      FROM Orders o2
                      JOIN Customers c2 ON c2.ID_CM = o2.ID_CM
                      WHERE c2.CITY_CM = c.CITY_CM)
ORDER BY c.CITY_CM, o.AMOUNT_OD DESC;
GO

-- 1.5. Вивести максимальну суму замовлення для кожного замовника. Упорядкувати дані за зменшенням суми замовлення.
SELECT c.ID_CM AS Код_замовника,
       c.NAME_CM AS Замовник,
       MAX(o.AMOUNT_OD) AS Максимальна_сума
FROM Customers c
JOIN Orders o ON o.ID_CM = c.ID_CM
GROUP BY c.ID_CM, c.NAME_CM
ORDER BY Максимальна_сума DESC;
GO

-- 1.6. Вивести інформацію про продавців, замовлення у яких більші за середню суму серед усіх замовлень. Упорядкувати дані за збільшенням суми замовлення.
SELECT DISTINCT s.ID_SP AS Код_продавця,
       s.NAME_SP AS Продавець,
       o.AMOUNT_OD AS Сума
FROM Salespeople s
JOIN Orders o ON o.ID_SP = s.ID_SP
WHERE o.AMOUNT_OD > (SELECT AVG(AMOUNT_OD) FROM Orders)
ORDER BY o.AMOUNT_OD;
GO

-- 1.7. Вивести кількість замовників у кожного продавця.
SELECT s.ID_SP AS Код_продавця,
       s.NAME_SP AS Продавець,
       COUNT(DISTINCT o.ID_CM) AS Кількість_замовників
FROM Salespeople s
JOIN Orders o ON o.ID_SP = s.ID_SP
GROUP BY s.ID_SP, s.NAME_SP
ORDER BY Кількість_замовників DESC;
GO

-- 1.8. Вивести дані про замовника, що зробив замовлення останнім за часом.
SELECT c.ID_CM AS Код_замовника,
       c.NAME_CM AS Замовник,
       o.DATE_OD AS Дата,
       o.AMOUNT_OD AS Сума
FROM Orders o
JOIN Customers c ON c.ID_CM = o.ID_CM
WHERE o.DATE_OD = (SELECT MAX(DATE_OD) FROM Orders);
GO

-- 1.9. Вивести дані про замовників, суму та дату замовлення.
SELECT c.ID_CM AS Код_замовника,
       c.NAME_CM AS Замовник,
       c.CITY_CM AS Місто,
       o.AMOUNT_OD AS Сума,
       o.DATE_OD AS Дата
FROM Customers c
JOIN Orders o ON o.ID_CM = c.ID_CM
ORDER BY o.DATE_OD, o.ID_OD;
GO

-- 1.10. Розрахувати загальну вартість замовлень для кожного продавця.
SELECT s.ID_SP AS Код_продавця,
       s.NAME_SP AS Продавець,
       SUM(o.AMOUNT_OD) AS Загальна_вартість
FROM Salespeople s
JOIN Orders o ON o.ID_SP = s.ID_SP
GROUP BY s.ID_SP, s.NAME_SP
ORDER BY Загальна_вартість DESC;
GO

-- 1.11. Визначити середню ціну замовлення для кожного замовника.
SELECT c.ID_CM AS Код_замовника,
       c.NAME_CM AS Замовник,
       CAST(AVG(o.AMOUNT_OD) AS DECIMAL(10, 2)) AS Середня_сума
FROM Customers c
JOIN Orders o ON o.ID_CM = c.ID_CM
GROUP BY c.ID_CM, c.NAME_CM
ORDER BY c.ID_CM;
GO

-- 1.12. Визначити, на яку суму було зроблено замовлень по кожному продавцеві за кожний день.
SELECT s.NAME_SP AS Продавець,
       o.DATE_OD AS Дата,
       SUM(o.AMOUNT_OD) AS Сума
FROM Salespeople s
JOIN Orders o ON o.ID_SP = s.ID_SP
GROUP BY s.NAME_SP, o.DATE_OD
ORDER BY o.DATE_OD, s.NAME_SP;
GO

-- 1.13. Підрахувати кількість замовлень, здійснених кожним замовником.
SELECT c.ID_CM AS Код_замовника,
       c.NAME_CM AS Замовник,
       COUNT(*) AS Кількість_замовлень
FROM Customers c
JOIN Orders o ON o.ID_CM = c.ID_CM
GROUP BY c.ID_CM, c.NAME_CM
ORDER BY Кількість_замовлень DESC, c.ID_CM;
GO

-- 1.14. Визначити суму замовлень по кожному замовнику за кожен місяць.
SELECT c.NAME_CM AS Замовник,
       YEAR(o.DATE_OD) AS Рік,
       MONTH(o.DATE_OD) AS Місяць,
       SUM(o.AMOUNT_OD) AS Сума
FROM Customers c
JOIN Orders o ON o.ID_CM = c.ID_CM
GROUP BY c.NAME_CM, YEAR(o.DATE_OD), MONTH(o.DATE_OD)
ORDER BY Рік, Місяць, c.NAME_CM;
GO

-- 1.15. Визначити замовників, у яких загальна кількість замовлень перевищила два.
-- Основний запит: більше двох замовлень
SELECT c.ID_CM AS Код_замовника,
       c.NAME_CM AS Замовник,
       COUNT(*) AS Кількість_замовлень
FROM Customers c
JOIN Orders o ON o.ID_CM = c.ID_CM
GROUP BY c.ID_CM, c.NAME_CM
HAVING COUNT(*) > 2
ORDER BY Кількість_замовлень DESC;

-- Адаптований запит: не менше двох замовлень
SELECT c.ID_CM AS Код_замовника,
       c.NAME_CM AS Замовник,
       COUNT(*) AS Кількість_замовлень
FROM Customers c
JOIN Orders o ON o.ID_CM = c.ID_CM
GROUP BY c.ID_CM, c.NAME_CM
HAVING COUNT(*) >= 2
ORDER BY Кількість_замовлень DESC;
GO

-- 1.16. Вивести список продавців, що виконали замовлень на суму, більшу за 2500 грн.
SELECT s.ID_SP AS Код_продавця,
       s.NAME_SP AS Продавець,
       SUM(o.AMOUNT_OD) AS Сума
FROM Salespeople s
JOIN Orders o ON o.ID_SP = s.ID_SP
GROUP BY s.ID_SP, s.NAME_SP
HAVING SUM(o.AMOUNT_OD) > 2500
ORDER BY Сума DESC;
GO

-- 1.17. Вивести список усіх замовників і кількість зроблених ними замовлень; замовників вивести в порядку зменшення кількості замовлень (у тому числі й тих, у кого ще немає жодного замовлення).
SELECT c.ID_CM AS Код_замовника,
       c.NAME_CM AS Замовник,
       c.CITY_CM AS Місто,
       COUNT(o.ID_OD) AS Кількість_замовлень
FROM Customers c
LEFT JOIN Orders o ON o.ID_CM = c.ID_CM
GROUP BY c.ID_CM, c.NAME_CM, c.CITY_CM
ORDER BY Кількість_замовлень DESC, c.ID_CM;
GO

-- 1.18. Вивести список замовників і кількість здійснених ними замовлень за умови, що замовник проживає в Римі.
-- Основний запит: замовники з Рима
SELECT c.NAME_CM AS Замовник,
       COUNT(o.ID_OD) AS Кількість_замовлень
FROM Customers c
LEFT JOIN Orders o ON o.ID_CM = c.ID_CM
WHERE c.CITY_CM = N'Рим'
GROUP BY c.NAME_CM
ORDER BY c.NAME_CM;

-- Адаптований запит: замовники з Києва
SELECT c.NAME_CM AS Замовник,
       COUNT(o.ID_OD) AS Кількість_замовлень
FROM Customers c
LEFT JOIN Orders o ON o.ID_CM = c.ID_CM
WHERE c.CITY_CM = N'Київ'
GROUP BY c.NAME_CM
ORDER BY c.NAME_CM;
GO

-- 1.19. Знайти продавця, який виконав замовлень на суму, що перевищує 10000 грн.
SELECT s.ID_SP AS Код_продавця,
       s.NAME_SP AS Продавець,
       SUM(o.AMOUNT_OD) AS Сума
FROM Salespeople s
JOIN Orders o ON o.ID_SP = s.ID_SP
GROUP BY s.ID_SP, s.NAME_SP
HAVING SUM(o.AMOUNT_OD) > 10000;
GO

USE LibraryDB;
GO

-- 2.1. Книги з авторами та видавництвами (INNER JOIN трьох таблиць)
SELECT b.title AS Назва,
       a.author_name AS Автор,
       p.publisher_name AS Видавництво,
       b.year_pub AS Рік
FROM Book b
JOIN Author a ON a.id_author = b.id_author
JOIN Publisher p ON p.id_publisher = b.id_publisher
ORDER BY b.title;
GO

-- 2.2. Читачі та книги, які вони брали (INNER JOIN)
SELECT r.reader_name AS Читач,
       b.title AS Книга,
       l.loan_date AS Дата_видачі,
       l.return_date AS Дата_повернення
FROM BookLoan l
JOIN Reader r ON r.id_reader = l.id_reader
JOIN Book b ON b.id_book = l.id_book
ORDER BY r.reader_name, l.loan_date;
GO

-- 2.3. Видавництва та кількість їх книг (LEFT JOIN)
SELECT p.publisher_name AS Видавництво,
       COUNT(b.id_book) AS Кількість_книг
FROM Publisher p
LEFT JOIN Book b ON b.id_publisher = p.id_publisher
GROUP BY p.id_publisher, p.publisher_name
ORDER BY Кількість_книг DESC, p.publisher_name;
GO

-- 2.4. Автори та кількість їх книг (LEFT JOIN)
SELECT a.author_name AS Автор,
       COUNT(b.id_book) AS Кількість_книг
FROM Author a
LEFT JOIN Book b ON b.id_author = a.id_author
GROUP BY a.id_author, a.author_name
ORDER BY Кількість_книг DESC, a.author_name;
GO

-- 2.5. Читачі, які брали книги видавництва «КСД» (JOIN чотирьох таблиць)
SELECT DISTINCT r.id_reader AS Код_читача,
       r.reader_name AS Читач,
       r.phone AS Телефон
FROM Reader r
JOIN BookLoan l ON l.id_reader = r.id_reader
JOIN Book b ON b.id_book = l.id_book
JOIN Publisher p ON p.id_publisher = b.id_publisher
WHERE p.publisher_name = N'КСД'
ORDER BY r.id_reader;
GO

-- 2.6. Книги, видані після 2020 року, з автором і видавництвом (INNER JOIN)
SELECT b.title AS Назва,
       b.year_pub AS Рік,
       a.author_name AS Автор,
       p.publisher_name AS Видавництво
FROM Book b
JOIN Author a ON a.id_author = b.id_author
JOIN Publisher p ON p.id_publisher = b.id_publisher
WHERE b.year_pub > 2020
ORDER BY b.year_pub;
GO

-- 2.7. Прізвища авторів і читачів в одному списку (UNION)
SELECT author_name AS ПІБ,
       N'автор' AS Статус
FROM Author
UNION
SELECT reader_name,
       N'читач'
FROM Reader
ORDER BY ПІБ;
GO

-- 2.8. Книги, видані до 2020 року, та книги дорожчі за 400 грн (UNION)
SELECT title AS Назва,
       year_pub AS Рік,
       price AS Ціна
FROM Book
WHERE year_pub < 2020
UNION
SELECT title,
       year_pub,
       price
FROM Book
WHERE price > 400
ORDER BY Назва;
GO
