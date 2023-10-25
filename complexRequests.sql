--Объединение всей информации об заказе--
CREATE VIEW FullOrders AS
SELECT o.Id, 
cor.Name AS CorposeName, cor.Surname AS CorposeSurname, cor.NumPassport AS CoproseNumPassport,
h.Id AS HallNumber, h.Capacity AS HallCapacity, h.Price AS HallPrice,
urn.Name AS UrnName, urn.Price AS UrnPrice,
cof.Id AS CoffinId, cof.Name AS CoffinName, cof.Price AS CoffinPrice,
u.UserId AS UserId, u.Name, u.Surname, u.EmailAdress, u.NumPassport, u.UserRole,
c.Id AS CeremonyId, c.Contact AS CeremonyContact, c.NameOfCompany
FROM Orders AS o
LEFT JOIN Corpose AS cor ON cor.Id=o.CorposeId
LEFT JOIN Hall AS h ON h.Id=o.HallId
LEFT JOIN RitualUrn AS urn ON urn.Id=o.UrnId
LEFT JOIN Coffin AS cof ON cof.Id=o.CoffinId
LEFT JOIN UserWithRole AS u ON u.UserId=o.UserId
LEFT JOIN OrderCeremony AS oc ON oc.OrderId=o.Id
LEFT JOIN Ceremony AS c ON oc.CeremonyId=c.Id;

--Получение пользователя с ролью текстом--
CREATE VIEW UserWithRole AS
SELECT u.Id AS UserId, u.Name, u.Surname, u.EmailAdress, u.NumPassport,
    CASE
        WHEN u.RoleCode IS NOT NULL THEN
            (SELECT r.Name FROM Role AS r WHERE r.Code=u.RoleCode)
        ELSE 'Нет роли'
    END AS UserRole
FROM User AS u;

--Аналогичное получение пользователя с ролями через LEFT OUTER JOIN--
SELECT u.Id AS UserId, u.Name, u.Surname, u.EmailAdress, u.NumPassport,
    CASE
        WHEN u.RoleCode IS NOT NULL THEN r.Name
        ELSE 'Нет роли'
    END AS UserRole
FROM User AS u
LEFT OUTER JOIN Role AS r ON u.ROleCode=r.Code;

--Получение пользователей и количества заказов каждого--
CREATE VIEW NumberOfOrders AS
SELECT u.Id AS UserId, u.Name, u.Surname, COUNT(o.Id) AS NumberOfOrders
FROM User AS u 
LEFT JOIN Orders AS o ON u.Id=o.UserId
GROUP BY u.Id;

--Получение церемоний у конкретного заказа--
SELECT o.Id AS OrderId, c.Id AS CeremonyId, c.Contact AS CeremonyContact, c.NameOfCompany
FROM Orders AS o
JOIN OrderCeremony AS oc ON oc.OrderId=o.Id
JOIN Ceremony AS c ON oc.CeremonyId=c.Id;

--Получение пользователей, у которых заказов болбше 0--
SELECT u.UserId, u.Name, u.Surname, u.NumberOfOrders
FROM NumberOfOrders AS u 
WHERE u.NumberOfOrders > 0
--ИЛИ--
SELECT u.Id AS UserId, u.Name, u.Surname, COUNT(o.Id) AS NumberOfOrders
FROM User AS u 
LEFT JOIN Orders AS o ON u.Id=o.UserId
GROUP BY u.Id
HAVING NumberOfOrders > 0;

--Выбрать всех людей, "побывавших" в крематории--
SELECT u.Name AS Name, u.Surname AS Surname, u.NumPassport AS NumPassport
FROM User AS u
UNION
SELECT c.Name AS Name, c.Surname AS Surname, c.NumPassport AS NumPassport
FROM Corpose AS c;

--Получение стоимости каждого заказа--
SELECT o.Id, h.Price + cof.Price + urn.Price AS TotalSum
FROM Orders AS o
JOIN Hall AS h ON h.Id=o.HallId
JOIN Coffin AS cof ON cof.Id=o.CoffinId
JOIN RitualUrn AS urn ON urn.Id=o.UrnId;

--Получить пользователей, у которых есть хоть 1 заказ--
SELECT u.UserId, u.Name, u.Surname, u.EmailAdress
FROM User AS u
WHERE EXISTS (
    SELECT 1
    FROM Orders AS o
    WHERE o.UserId=u.Id
);

--Сколько потратил каждый пользователь--
SELECT u.Id, u.Name, u.Surname,
    CASE
        WHEN OrSum.TotalSum IS NOT NULL THEN OrSum.TotalSum
        ELSE 0
    END AS TotalSum
FROM User AS u
LEFT JOIN (
    SELECT o.Id, o.UserId, h.Price + cof.Price + urn.Price AS TotalSum
    FROM Orders AS o
    JOIN Hall AS h ON h.Id=o.HallId
    JOIN Coffin AS cof ON cof.Id=o.CoffinId
    JOIN RitualUrn AS urn ON urn.Id=o.UrnId
)  
AS OrSum ON u.Id=OrSum.UserId
ORDER BY TotalSum DESC;

--Таблица пользователей со своими заказами--
CREATE VIEW UsersWithOrders AS
SELECT DISTINCT u.Id AS UserId, o.Id AS OrderId, o.HallPrice, o.UrnPrice, o.CoffinPrice
FROM User AS u
JOIN FullOrders AS o ON u.Id=o.UserId;

--Использование PARTITION + оконные функции--
SELECT o.UserId, o.OrderId, o.HallPrice, o.UrnPrice, o.CoffinPrice,
AVG(o.UrnPrice) OVER (PARTITION BY o.UserId) AS AVG_URN,
LAG(o.CoffinPrice) OVER (ORDER BY o.UserId) AS PREV_CofPrice,
ROW_NUMBER() OVER (PARTITION BY o.UserId ORDER BY o.HallPrice DESC) AS ROW_NUM_HallPrice 
FROM UsersWithOrders AS o;

--Проверим запрос--
EXPLAIN SELECT u.Id AS UserId, u.Name, u.Surname, u.EmailAdress, u.NumPassport,
    CASE
        WHEN u.RoleCode IS NOT NULL THEN
            (SELECT r.Name FROM Role AS r WHERE r.Code=u.RoleCode)
        ELSE 'Нет роли'
    END AS UserRole
FROM User AS u;