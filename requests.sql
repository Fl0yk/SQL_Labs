--Выбор id, имени, фамилии и адреса всех пользователей--
SELECT u.Id, u.Name, u.Surname, u.EmailAdress FROM User AS u;

--Получение зала и затраты на одного человека--
SELECT Id, ROUND(Price / Capacity) AS CostPerPerson FROM Hall;

--Выбор урн стоимостью больше 15--
SELECT * FROM RitualUrn
WHERE Price > 15;

--Номера залов и их свободные даты, которые после сегодняшнего дня--
SELECT h.Id, d.Date FROM Hall AS h, Dates AS d
WHERE h.Id=d.HallId AND d.Date > date('now');

--Выбор 1 зала с пропуском 1--
SELECT * FROM Hall
OFFSET 1
LIMIT 1;

--ВЫбор рун со стоимостью между 15 и 25--
SELECT * FROM RitualUrn
WHERE Price BETWEEN 15 AND 25;

--Выбор трупа, у которого имя начинается suicid--
SELECT * FROM Corpose 
WHERE LOWER(Name) LIKE 'suicid%';

--Выбор трупа, который заканчивается на ed--
SELECT * FROM Corpose 
WHERE LOWER(Name) LIKE '%ed';

--Выбор пользователей, имя большими, фамилии маленьким--
SELECT UPPER(Name), LOWER(Surname) FROM User

--Повышение стоимости гроба, у которого Id = 1, на 3 единицы (инфляция:( )--
UPDATE Coffin
SET Price = Price + 3
WHERE Id = 1;

--Удаление таблицы--
DELETE IF EXISTS RitualUrn;

--Переименоване таблицы--
ALTER TABLE Test
RENAME TO Testik;

--Добавление столбца--
ALTER TABLE Testik
ADD COLUMN NewField TEXT NOT NULL;

--Переименование столбца--
ALTER TABLE Testik
RENAME COLUMN Field TO EditField;

--удаление столбца--
ALTER TABLE Testik
DROP COLUMN NewField;