-- Лабораторна робота № 3. Створення простих запитів, використання функцій
-- MS SQL Server, бази даних Sales111 і LibraryDB

USE Sales111;
GO

-- 1.1. Вивести прізвища всіх продавців, міста їхнього проживання та комісійні продавців, використовуючи замінник імен стовпців.
SELECT NAME_SP AS Прізвище,
       CITY_SP AS Місто,
       COMM_SP AS Комісійні
FROM Salespeople;
GO

-- 1.2. Вивести коди усіх замовників та їхні прізвища за алфавітом.
SELECT ID_CM AS Код_замовника,
       NAME_CM AS Прізвище
FROM Customers
ORDER BY NAME_CM ASC;
GO

-- 1.3. Вивести перелік міст, де мешкають замовники.
SELECT DISTINCT CITY_CM AS Місто
FROM Customers
ORDER BY CITY_CM;
GO

-- 1.4. Вивести прізвища усіх продавців, що містять комбінації символів «ee» або «rr».
-- Основний запит: комбінації «ee» або «rr»
SELECT NAME_SP AS Прізвище
FROM Salespeople
WHERE NAME_SP LIKE '%ee%' OR NAME_SP LIKE '%rr%';

-- Адаптований запит: комбінації «ен» або «ов»
SELECT NAME_SP AS Прізвище
FROM Salespeople
WHERE NAME_SP LIKE '%ен%' OR NAME_SP LIKE '%ов%';
GO

-- 1.5. Вивести всю інформацію стосовно замовників, що мешкають у Лондоні.
-- Запит за умовою «Лондон»
SELECT ID_CM, NAME_CM, CITY_CM, BONUS_CM
FROM Customers
WHERE CITY_CM = 'Лондон';

-- Адаптований запит: замовники з Києва
SELECT ID_CM, NAME_CM, CITY_CM, BONUS_CM
FROM Customers
WHERE CITY_CM = 'Київ';
GO

-- 1.6. Вивести коди замовників, замовлення яких були зроблені у період з 10.04.2024 до 10.06.2024.
-- Заданий період 10.04.2024 – 10.06.2024
SELECT DISTINCT ID_CM AS Код_замовника
FROM Orders
WHERE DATE_OD BETWEEN '2024-04-10' AND '2024-06-10';

-- Адаптований період відповідно до наявних замовлень
SELECT DISTINCT ID_CM AS Код_замовника
FROM Orders
WHERE DATE_OD BETWEEN '2024-08-01' AND '2024-10-31';
GO

-- 1.7. Вивести дані про замовників, які мають визначений рейтинг (Rating). Виконати запит кілька разів із різними рейтингами.
-- Замовники з рейтингом 100
SELECT ID_CM, NAME_CM, CITY_CM, BONUS_CM
FROM Customers
WHERE BONUS_CM = 100;

-- Замовники з рейтингом 300
SELECT ID_CM, NAME_CM, CITY_CM, BONUS_CM
FROM Customers
WHERE BONUS_CM = 300;
GO

-- 1.8. Вивести адреси і найменування міст із великої літери (таблиця продавців).
SELECT NAME_SP AS Прізвище,
       'м. ' + UPPER(LEFT(CITY_SP, 1)) +
       LOWER(SUBSTRING(CITY_SP, 2, LEN(CITY_SP))) AS Адреса,
       UPPER(LEFT(CITY_SP, 1)) +
       LOWER(SUBSTRING(CITY_SP, 2, LEN(CITY_SP))) AS Місто
FROM Salespeople;
GO

-- 1.9. Скласти запити, що формують рядки: чотири перших символи «SQL_SERVER», починаючи з другого; вміст стовпця SName таблиці Salespeople, починаючи з другого символу.
SELECT SUBSTRING('SQL_SERVER', 2, 4) AS Результат;

SELECT SUBSTRING(NAME_SP, 2, LEN(NAME_SP)) AS Прізвище_з_2_символу
FROM Salespeople;
GO

-- 1.10. Визначити, скільки прізвищ містять літеру «б» (таблиця замовників).
-- Кількість прізвищ із літерою «б»
SELECT COUNT(*) AS Кількість
FROM Customers
WHERE NAME_CM LIKE '%б%';

-- Адаптовано: кількість прізвищ із літерою «о»
SELECT COUNT(*) AS Кількість
FROM Customers
WHERE NAME_CM LIKE '%о%';
GO

-- 1.11. Написати запит, що дозволяє одержати день тижня через два місяці після поточної дати.
SELECT DATEADD(MONTH, 2, GETDATE()) AS Дата_через_2_місяці,
       DATENAME(WEEKDAY, DATEADD(MONTH, 2, GETDATE())) AS День_тижня;
GO

-- 1.12. Вивести замовлення з мінімальною і максимальною вартістю та середню вартість замовлень.
SELECT MIN(AMOUNT_OD) AS Мінімальна_вартість,
       MAX(AMOUNT_OD) AS Максимальна_вартість,
       CAST(AVG(AMOUNT_OD) AS DECIMAL(10, 2)) AS Середня_вартість
FROM Orders;
GO

-- 1.13. Вивести кількість замовлень, що має кожний замовник.
SELECT ID_CM AS Код_замовника,
       COUNT(*) AS Кількість_замовлень
FROM Orders
GROUP BY ID_CM
ORDER BY Кількість_замовлень DESC, ID_CM;
GO

-- 1.14. Вивести коди тільки тих замовників, які мають більше двох замовлень.
-- Замовники, які мають більше двох замовлень
SELECT ID_CM AS Код_замовника, COUNT(*) AS Кількість_замовлень
FROM Orders
GROUP BY ID_CM
HAVING COUNT(*) > 2;

-- Адаптовано: більше одного замовлення
SELECT ID_CM AS Код_замовника, COUNT(*) AS Кількість_замовлень
FROM Orders
GROUP BY ID_CM
HAVING COUNT(*) > 1;
GO

USE LibraryDB;
GO

-- 2.1. Проста вибірка (вибір усіх значень з таблиці)
-- 2.1.1. Вивести всі записи таблиці Author.
-- 2.1.2. Вивести всі записи таблиці Book.
-- 2.1.3. Вивести всі записи таблиці Reader.
-- 2.1.1. Усі автори
SELECT * FROM Author;

-- 2.1.2. Усі книги
SELECT * FROM Book;

-- 2.1.3. Усі читачі
SELECT * FROM Reader;
GO

-- 2.2. Вибірка обчислюваних значень (множення, додавання, віднімання, ділення)
-- 2.2.1. Обчислити вартість двох примірників кожної книги.
-- 2.2.2. Обчислити ціну книги зі знижкою 10 % та суму економії.
-- 2.2.3. Обчислити вік кожної книги за роком видання.
-- 2.2.1. Вартість двох примірників
SELECT title AS Назва,
       price AS Ціна,
       CAST(price * 2 AS DECIMAL(10, 2)) AS Ціна_двох_примірників
FROM Book;

-- 2.2.2. Ціна зі знижкою 10 %
SELECT title AS Назва,
       price AS Ціна,
       CAST(price * 0.9 AS DECIMAL(10, 2)) AS Ціна_зі_знижкою,
       CAST(price * 0.1 AS DECIMAL(10, 2)) AS Економія
FROM Book;

-- 2.2.3. Вік книги
SELECT title AS Назва,
       year_pub AS Рік,
       YEAR(GETDATE()) - year_pub AS Вік_книги
FROM Book;
GO

-- 2.3. Вибір з умовою (WHERE)
-- 2.3.1. Вивести книги, ціна яких перевищує 300 грн.
-- 2.3.2. Вивести авторів із України.
-- 2.3.3. Вивести книги, видані у 2020 році або пізніше.
-- 2.3.1. Книги дорожчі за 300 грн
SELECT title AS Назва, price AS Ціна
FROM Book
WHERE price > 300;

-- 2.3.2. Автори з України
SELECT author_name AS Автор, country AS Країна
FROM Author
WHERE country = 'Україна';

-- 2.3.3. Книги, видані з 2020 року
SELECT title AS Назва, year_pub AS Рік
FROM Book
WHERE year_pub >= 2020;
GO

-- 2.4. Вибір з упорядкуванням (ORDER BY)
-- 2.4.1. Вивести книги в порядку спадання ціни.
-- 2.4.2. Вивести авторів за алфавітом.
-- 2.4.3. Вивести видачі книг у порядку спадання дати видачі.
-- 2.4.1. Книги за спаданням ціни
SELECT title AS Назва, price AS Ціна
FROM Book
ORDER BY price DESC;

-- 2.4.2. Автори за алфавітом
SELECT author_name AS Автор, country AS Країна
FROM Author
ORDER BY author_name ASC;

-- 2.4.3. Видачі за спаданням дати
SELECT id_loan AS Код_видачі,
       id_book AS Код_книги,
       loan_date AS Дата_видачі
FROM BookLoan
ORDER BY loan_date DESC, id_book ASC;
GO

-- 2.5. Вибірка з використанням оператора BETWEEN
-- 2.5.1. Вивести книги з ціною від 250 до 450 грн.
-- 2.5.2. Вивести книги, видані у період з 2019 до 2021 року.
-- 2.5.3. Вивести видачі за період з 05.09.2026 до 15.09.2026.
-- 2.5.1. Ціна в діапазоні 250–450 грн
SELECT title AS Назва, price AS Ціна
FROM Book
WHERE price BETWEEN 250 AND 450;

-- 2.5.2. Рік видання в діапазоні 2019–2021
SELECT title AS Назва, year_pub AS Рік
FROM Book
WHERE year_pub BETWEEN 2019 AND 2021;

-- 2.5.3. Дати видачі в заданому діапазоні
SELECT id_loan AS Код_видачі,
       loan_date AS Дата_видачі,
       return_date AS Дата_повернення
FROM BookLoan
WHERE loan_date BETWEEN '2026-09-05' AND '2026-09-15';
GO

-- 2.6. Вибірка з використанням шаблона (LIKE)
-- 2.6.1. Вивести авторів, ім’я яких починається на «Іван».
-- 2.6.2. Вивести книги, у назві яких є літера «а».
-- 2.6.3. Вивести читачів, номер телефону яких починається з «+38063».
-- 2.6.1. Автори, ім’я яких починається на «Іван»
SELECT author_name AS Автор
FROM Author
WHERE author_name LIKE 'Іван%';

-- 2.6.2. Книги, у назві яких є літера «а»
SELECT title AS Назва
FROM Book
WHERE title LIKE '%а%';

-- 2.6.3. Читачі з телефоном на «+38063»
SELECT reader_name AS Читач, phone AS Телефон
FROM Reader
WHERE phone LIKE '+38063%';
GO

-- 2.7. Вибірка з використанням групування (GROUP BY)
-- 2.7.1. Підрахувати кількість книг кожного видавництва.
-- 2.7.2. Підрахувати кількість видач для кожного читача.
-- 2.7.3. Підрахувати кількість книг за роками видання.
-- 2.7.1. Кількість книг за видавництвами
SELECT id_publisher AS Код_видавництва,
       COUNT(*) AS Кількість_книг
FROM Book
GROUP BY id_publisher
ORDER BY Кількість_книг DESC;

-- 2.7.2. Кількість видач за читачами
SELECT id_reader AS Код_читача,
       COUNT(*) AS Кількість_видач
FROM BookLoan
GROUP BY id_reader
ORDER BY Кількість_видач DESC;

-- 2.7.3. Кількість книг за роками
SELECT year_pub AS Рік,
       COUNT(*) AS Кількість_книг
FROM Book
GROUP BY year_pub
ORDER BY year_pub;
GO

-- 2.8. Вибірка з використанням агрегатних функцій (COUNT, SUM, AVG)
-- 2.8.1. Обчислити кількість книг, їхню загальну та середню вартість.
-- 2.8.2. Обчислити мінімальну, максимальну та середню ціну книги.
-- 2.8.3. Обчислити кількість і середню ціну книг, виданих з 2020 року.
-- 2.8.1. Кількість, сума та середня ціна книг
SELECT COUNT(*) AS Кількість_книг,
       SUM(price) AS Загальна_вартість,
       CAST(AVG(price) AS DECIMAL(10, 2)) AS Середня_ціна
FROM Book;

-- 2.8.2. Мінімальна, максимальна та середня ціна
SELECT MIN(price) AS Мінімальна_ціна,
       MAX(price) AS Максимальна_ціна,
       CAST(AVG(price) AS DECIMAL(10, 2)) AS Середня_ціна
FROM Book;

-- 2.8.3. Агрегати для книг, виданих з 2020 року
SELECT COUNT(*) AS Кількість_книг,
       CAST(AVG(price) AS DECIMAL(10, 2)) AS Середня_ціна
FROM Book
WHERE year_pub >= 2020;
GO
