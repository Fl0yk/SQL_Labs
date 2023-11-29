---Триггер проверяет, что заказ изменяют до того, как его одобрили---
CREATE OR REPLACE FUNCTION UpdateOrderCheckState()
RETURNS TRIGGER AS $$
DECLARE 
    maxStateCode INTEGER;
BEGIN
    SELECT INTO maxStateCode s.Code
    FROM StateOrder AS s
    WHERE s.Name = 'Approved';

    IF NEW.StateCode >= maxStateCode AND NEW.StateCode = OLD.StateCode THEN
        RAISE EXCEPTION 'Заказ уже нельзя изменить';
	END IF;

    NEW.DateOfActual := CURRENT_DATE;
	
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE TRIGGER UpdateOrderStateTrigger
BEFORE UPDATE ON Orders
FOR EACH ROW
EXECUTE FUNCTION UpdateOrderCheckState();

UPDATE Orders
SET HallId = 3
WHERE Id = 1;

---триггер проверяет, дата для оформления зала больше ли сегодняшней---
CREATE OR REPLACE FUNCTION CheckDateGreaterThanToday()
RETURNS TRIGGER AS $$
BEGIN
    IF NEW.Date <= CURRENT_DATE THEN
        RAISE EXCEPTION 'Дата должна быть больше текущей';
    END IF;

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER InsertDateGreateThanTodayTrigger
BEFORE INSERT ON Dates
FOR EACH ROW EXECUTE FUNCTION CheckDateGreaterThanToday();

INSERT INTO Dates (HallId, Date) VALUES
(1, '2023-11-11');

---Логи работы с заказами---
CREATE OR REPLACE FUNCTION ProcessOrderAudit()
RETURNS TRIGGER AS $$
DECLARE
    clientPassport CHAR(14);
BEGIN
    SELECT INTO clientPassport u.NumPassport
    FROM Users AS u WHERE u.Id = NEW.UserId;

    IF (TG_OP = 'DELETE') THEN
        INSERT INTO OrdersAudit SELECT 'D', now(), NEW.Id, clientPassport;
    ELSIF (TG_OP = 'UPDATE') THEN
        INSERT INTO OrdersAudit SELECT 'U', now(), NEW.Id, clientPassport;
    ELSIF (TG_OP = 'INSERT') THEN
        INSERT INTO OrdersAudit SELECT 'I', now(), NEW.Id, clientPassport;
    END IF;
    RETURN NULL;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE TRIGGER OrdersAuditTrigger
AFTER INSERT OR UPDATE OR DELETE ON Orders
FOR EACH ROW EXECUTE FUNCTION ProcessOrderAudit();

---Просто вывод старой и новой цены при изменении зала---
CREATE FUNCTION BeforHallUpdate()
RETURNS TRIGGER AS $$
BEGIN
	RAISE NOTICE 'OLD PRICE - %,
				NEW PRICE - %',
				NEW.Price, OLD.Price;
	RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE TRIGGER BeforeHallUpdateTrigger
BEFORE UPDATE ON Hall
FOR EACH ROW EXECUTE FUNCTION BeforHallUpdate();


---Проверяет наличие пользователя---
CREATE OR REPLACE FUNCTION IsExistUser(
    nameArg VARCHAR,
    numPassportArg CHAR
) RETURNS BOOLEAN AS $$
DECLARE isExists BOOLEAN;
BEGIN
    SELECT EXISTS(
        SELECT 1 FROM Users AS u 
        WHERE u.Name = nameArg AND u.numPassport = numPassportArg)
    INTO isExists;

    return isExists;
END;
$$ LANGUAGE plpgsql;

---CRUD для Hall(Capacity, Price)---
CREATE OR REPLACE PROCEDURE CreateHall(capacity INTEGER,
                                        Price NUMERIC(8,2))
AS $$
BEGIN
    IF capacity <= 0 THEN
            RAISE EXCEPTION 'Вместительность должна быть больще 0';
    END IF;
    IF price <= 0 THEN
            RAISE EXCEPTION 'Стоимость должна быть больще 0';
    END IF;

    INSERT INTO Hall (Capacity, Price) 
    VALUES (capacity, price);
END;
$$ LANGUAGE plpgsql;

CALL CreateHall(200, 260);

CREATE OR REPLACE PROCEDURE UpdateHall(hallId INTEGER,
                                        newCapacity INTEGER,
                                        newPrice NUMERIC(8,2))
AS $$
BEGIN
    IF newCapacity <= 0 THEN
            RAISE EXCEPTION 'Вместительность должна быть больще 0';
    END IF;
    IF newPrice <= 0 THEN
            RAISE EXCEPTION 'Стоимость должна быть больще 0';
    END IF;

    UPDATE Hall
    SET Price = newPrice, Capacity = newCapacity
    WHERE Id = hallId;
END;
$$ LANGUAGE plpgsql;

CALL UpdateHall(4, 200, 260);

CREATE OR REPLACE PROCEDURE DeleteHallById(hallId INTEGER)
AS $$
BEGIN
    DELETE FROM Hall 
    WHERE Id = hallId; 
END;
$$ LANGUAGE plpgsql;

CALL DeleteHallById(4);

--CRUD пользователей--

-- Создание пользователя
CREATE OR REPLACE PROCEDURE CreateUser(
    userName CHARACTER VARYING(20),
    userSurname CHARACTER VARYING(20),
    emailAddress TEXT,
    roleCode INTEGER,
    numPassport CHAR(14)
)
AS $$
BEGIN
    IF LENGTH(userName) = 0 THEN
        RAISE EXCEPTION 'Имя пользователя не может быть пустым';
    END IF;
    IF LENGTH(userSurname) = 0 THEN
        RAISE EXCEPTION 'Фамилия пользователя не может быть пустой';
    END IF;
    IF LENGTH(emailAddress) = 0 THEN
        RAISE EXCEPTION 'Email-адрес не может быть пустым';
    END IF;
    IF roleCode IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Role WHERE Code = roleCode) THEN
        RAISE EXCEPTION 'Роль с указанным кодом не существует';
    END IF;
    IF numPassport !~ '^[0-9]{7}[A-Z][0-9]{3}(PB|BI|BA)[0-9]$' THEN
            RAISE EXCEPTION 'Номер паспорта не соответствует формату';
    END IF;

    INSERT INTO Users (Name, Surname, EmailAdress, RoleCode, NumPassport) 
    VALUES (userName, userSurname, emailAddress, roleCode, numPassport);
END;
$$ LANGUAGE plpgsql;

-- Обновление пользователя
CREATE OR REPLACE PROCEDURE UpdateUser(
    userId INTEGER,
    newUserName CHARACTER VARYING(20),
    newUserSurname CHARACTER VARYING(20),
    newEmailAddress TEXT,
    newRoleCode INTEGER
)
AS $$
BEGIN
    IF LENGTH(newUserName) = 0 THEN
        RAISE EXCEPTION 'Имя пользователя не может быть пустым';
    END IF;
    IF LENGTH(newUserSurname) = 0 THEN
        RAISE EXCEPTION 'Фамилия пользователя не может быть пустой';
    END IF;
    IF LENGTH(newEmailAddress) = 0 THEN
        RAISE EXCEPTION 'Email-адрес не может быть пустым';
    END IF;
    IF newRoleCode IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Role WHERE Code = newRoleCode) THEN
        RAISE EXCEPTION 'Роль с указанным кодом не существует';
    END IF;

    UPDATE Users
    SET 
        Name = newUserName,
        Surname = newUserSurname,
        EmailAdress = newEmailAddress
    WHERE Id = userId;
END;
$$ LANGUAGE plpgsql;

-- Обновление пользователя без изменения роли
CREATE OR REPLACE PROCEDURE UpdateUserWithoutRole(
    userId INTEGER,
    newUserName CHARACTER VARYING(20),
    newUserSurname CHARACTER VARYING(20),
    newEmailAddress TEXT
)
AS $$
BEGIN
    IF LENGTH(newUserName) = 0 THEN
        RAISE EXCEPTION 'Имя пользователя не может быть пустым';
    END IF;
    IF LENGTH(newUserSurname) = 0 THEN
        RAISE EXCEPTION 'Фамилия пользователя не может быть пустой';
    END IF;
    IF LENGTH(newEmailAddress) = 0 THEN
        RAISE EXCEPTION 'Email-адрес не может быть пустым';
    END IF;

    UPDATE Users
    SET 
        Name = newUserName,
        Surname = newUserSurname,
        EmailAdress = newEmailAddress
    WHERE Id = userId;
END;
$$ LANGUAGE plpgsql;

-- Удаление пользователя по ID
CREATE OR REPLACE PROCEDURE DeleteUserById(userId INTEGER)
AS $$
BEGIN
    DELETE FROM Users 
    WHERE Id = userId; 
END;
$$ LANGUAGE plpgsql;

--CRUD гробов--

-- Создание гроба
CREATE OR REPLACE PROCEDURE CreateCoffin(
    coffinName CHARACTER VARYING(20),
    coffinPrice NUMERIC(8, 2),
    imagePath TEXT
)
AS $$
BEGIN
    IF LENGTH(coffinName) = 0 THEN
        RAISE EXCEPTION 'Название гроба не может быть пустым';
    END IF;
    IF coffinPrice <= 0 THEN
        RAISE EXCEPTION 'Стоимость гроба должна быть больше 0';
    END IF;
    IF LENGTH(imagePath) = 0 THEN
        RAISE EXCEPTION 'Путь к изображению не может быть пустым';
    END IF;

    INSERT INTO Coffin (Name, Price, ImagePath) 
    VALUES (coffinName, coffinPrice, imagePath);
END;
$$ LANGUAGE plpgsql;

-- Обновление гроба
CREATE OR REPLACE PROCEDURE UpdateCoffin(
    coffinId INTEGER,
    newCoffinName CHARACTER VARYING(20),
    newCoffinPrice NUMERIC(8, 2),
    newImagePath TEXT
)
AS $$
BEGIN
    IF LENGTH(newCoffinName) = 0 THEN
        RAISE EXCEPTION 'Название гроба не может быть пустым';
    END IF;
    IF newCoffinPrice <= 0 THEN
        RAISE EXCEPTION 'Стоимость гроба должна быть больше 0';
    END IF;
    IF LENGTH(newImagePath) = 0 THEN
        RAISE EXCEPTION 'Путь к изображению не может быть пустым';
    END IF;

    UPDATE Coffin
    SET 
        Name = newCoffinName,
        Price = newCoffinPrice,
        ImagePath = newImagePath
    WHERE Id = coffinId;
END;
$$ LANGUAGE plpgsql;

-- Удаление гроба по ID
CREATE OR REPLACE PROCEDURE DeleteCoffinById(coffinId INTEGER)
AS $$
BEGIN
    DELETE FROM Coffin 
    WHERE Id = coffinId; 
END;
$$ LANGUAGE plpgsql;

--CRUD трупа--

-- Создание трупа
CREATE OR REPLACE PROCEDURE CreateCorpse(
    corpseName CHARACTER VARYING(20),
    corpseSurname CHARACTER VARYING(20),
    numPassport CHAR(14)
)
AS $$
BEGIN
    IF LENGTH(corpseName) = 0 THEN
        RAISE EXCEPTION 'Имя трупа не может быть пустым';
    END IF;
    IF LENGTH(corpseSurname) = 0 THEN
        RAISE EXCEPTION 'Фамилия трупа не может быть пустой';
    END IF;
    IF numPassport !~ '^[0-9]{7}[A-Z][0-9]{3}(PB|BI|BA)[0-9]$' THEN
            RAISE EXCEPTION 'Номер паспорта не соответствует формату';
    END IF;

    INSERT INTO Corpose (Name, Surname, NumPassport) 
    VALUES (corpseName, corpseSurname, numPassport);
END;
$$ LANGUAGE plpgsql;

-- Обновление трупа
CREATE OR REPLACE PROCEDURE UpdateCorpse(
    corpseId INTEGER,
    newCorpseName CHARACTER VARYING(20),
    newCorpseSurname CHARACTER VARYING(20),
    newNumPassport CHAR(14)
)
AS $$
BEGIN
    IF LENGTH(newCorpseName) = 0 THEN
        RAISE EXCEPTION 'Имя трупа не может быть пустым';
    END IF;
    IF LENGTH(newCorpseSurname) = 0 THEN
        RAISE EXCEPTION 'Фамилия трупа не может быть пустой';
    END IF;
    IF numPassport !~ '^[0-9]{7}[A-Z][0-9]{3}(PB|BI|BA)[0-9]$' THEN
            RAISE EXCEPTION 'Номер паспорта не соответствует формату';
    END IF;

    UPDATE Corpose
    SET 
        Name = newCorpseName,
        Surname = newCorpseSurname,
        NumPassport = newNumPassport
    WHERE Id = corpseId;
END;
$$ LANGUAGE plpgsql;

-- Удаление трупа по ID
CREATE OR REPLACE PROCEDURE DeleteCorpseById(corpseId INTEGER)
AS $$
BEGIN
    DELETE FROM Corpose 
    WHERE Id = corpseId; 
END;
$$ LANGUAGE plpgsql;

--CRUD ритуальных урн--
-- Создание ритуальной урны
CREATE OR REPLACE PROCEDURE CreateRitualUrn(
    urnName CHARACTER VARYING(20),
    urnPrice NUMERIC(8, 2),
    imagePath TEXT
)
AS $$
BEGIN
    IF LENGTH(urnName) = 0 THEN
        RAISE EXCEPTION 'Название ритуальной урны не может быть пустым';
    END IF;
    IF urnPrice <= 0 THEN
        RAISE EXCEPTION 'Стоимость ритуальной урны должна быть больше 0';
    END IF;
    IF LENGTH(imagePath) = 0 THEN
        RAISE EXCEPTION 'Путь к изображению не может быть пустым';
    END IF;

    INSERT INTO RitualUrn (Name, Price, ImagePath) 
    VALUES (urnName, urnPrice, imagePath);
END;
$$ LANGUAGE plpgsql;

-- Обновление ритуальной урны
CREATE OR REPLACE PROCEDURE UpdateRitualUrn(
    urnId INTEGER,
    newUrnName CHARACTER VARYING(20),
    newUrnPrice NUMERIC(8, 2),
    newImagePath TEXT
)
AS $$
BEGIN
    IF LENGTH(newUrnName) = 0 THEN
        RAISE EXCEPTION 'Название ритуальной урны не может быть пустым';
    END IF;
    IF newUrnPrice <= 0 THEN
        RAISE EXCEPTION 'Стоимость ритуальной урны должна быть больше 0';
    END IF;
    IF LENGTH(newImagePath) = 0 THEN
        RAISE EXCEPTION 'Путь к изображению не может быть пустым';
    END IF;

    UPDATE RitualUrn
    SET 
        Name = newUrnName,
        Price = newUrnPrice,
        ImagePath = newImagePath
    WHERE Id = urnId;
END;
$$ LANGUAGE plpgsql;

-- Удаление ритуальной урны по ID
CREATE OR REPLACE PROCEDURE DeleteRitualUrnById(urnId INTEGER)
AS $$
BEGIN
    DELETE FROM RitualUrn 
    WHERE Id = urnId; 
END;
$$ LANGUAGE plpgsql;

--CRUD церемонии--

-- Создание церемонии
CREATE OR REPLACE PROCEDURE CreateCeremony(
    contact TEXT,
    nameOfCompany TEXT,
    description TEXT
)
AS $$
BEGIN
    IF LENGTH(contact) = 0 THEN
        RAISE EXCEPTION 'Контакт не может быть пустым';
    END IF;
    IF LENGTH(nameOfCompany) = 0 THEN
        RAISE EXCEPTION 'Название компании не может быть пустым';
    END IF;
    IF LENGTH(description) = 0 THEN
        RAISE EXCEPTION 'Описание не может быть пустым';
    END IF;

    INSERT INTO Ceremony (Contact, NameOfCompany, Description) 
    VALUES (contact, nameOfCompany, description);
END;
$$ LANGUAGE plpgsql;

-- Обновление церемонии
CREATE OR REPLACE PROCEDURE UpdateCeremony(
    ceremonyId INTEGER,
    newContact TEXT,
    newNameOfCompany TEXT,
    newDescription TEXT
)
AS $$
BEGIN
    IF LENGTH(newContact) = 0 THEN
        RAISE EXCEPTION 'Контакт не может быть пустым';
    END IF;
    IF LENGTH(newNameOfCompany) = 0 THEN
        RAISE EXCEPTION 'Название компании не может быть пустым';
    END IF;
    IF LENGTH(newDescription) = 0 THEN
        RAISE EXCEPTION 'Описание не может быть пустым';
    END IF;

    UPDATE Ceremony
    SET 
        Contact = newContact,
        NameOfCompany = newNameOfCompany,
        Description = newDescription
    WHERE Id = ceremonyId;
END;
$$ LANGUAGE plpgsql;

-- Удаление церемонии по ID
CREATE OR REPLACE PROCEDURE DeleteCeremonyById(ceremonyId INTEGER)
AS $$
BEGIN
    DELETE FROM Ceremony 
    WHERE Id = ceremonyId; 
END;
$$ LANGUAGE plpgsql;

--CRUD заказов--

-- Создание заказа
CREATE OR REPLACE PROCEDURE CreateOrder(
    dateOfActual DATE,
    shallId INTEGER,
    corposeId INTEGER,
    coffinId INTEGER,
    userId INTEGER,
    urnId INTEGER,
    stateCode INTEGER
)
AS $$
BEGIN
    IF dateOfActual IS NULL THEN
        RAISE EXCEPTION 'Дата заказа не может быть пустой';
    END IF;

    DELETE FROM Dates
    WHERE HallId=shallId AND date=dateOfActual;

    INSERT INTO Orders (DateOfActual, HallId, CorposeId, CoffinId, UserId, UrnId, StateCode) 
    VALUES (dateOfActual, shallId, corposeId, coffinId, userId, urnId, stateCode);
END;
$$ LANGUAGE plpgsql;

CALL CreateOrder(date('2023-12-23'), 1, 8, 9, 4, 1, 1);
--Изменение статуса заказа--

CREATE OR REPLACE PROCEDURE UpdateOrderStatus(orderId INTEGER, newStatusCode INTEGER)
AS $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM Orders WHERE Id = orderId) THEN
        RAISE EXCEPTION 'Заказ с указанным Id не существует';
    END IF;

    IF NOT EXISTS (SELECT 1 FROM StateOrder WHERE Code = newStatusCode) THEN
        RAISE EXCEPTION 'Статус с указанным кодом не существует';
    END IF;

    UPDATE Orders
    SET StateCode = newStatusCode
    WHERE Id = orderId;
END;
$$ LANGUAGE plpgsql;

--Частичный поиск сущностей--

CREATE OR REPLACE FUNCTION PartialSearchCoffin(partialName VARCHAR)
RETURNS TABLE (Id INTEGER, Name VARCHAR(20), Price NUMERIC(8, 2), ImagePath TEXT)
AS $$
BEGIN
    RETURN QUERY
    SELECT c.Id, c.Name, c.Price, c.ImagePath
    FROM Coffin AS c
    WHERE c.Name ILIKE '%' || partialName || '%';
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION PartialSearchRitualUrn(partialName VARCHAR)
RETURNS TABLE (Id INTEGER, Name VARCHAR(20), Price NUMERIC(8, 2), ImagePath TEXT)
AS $$
BEGIN
    RETURN QUERY
    SELECT u.Id, u.Name, u.Price, u.ImagePath
    FROM RitualUrn AS u
    WHERE u.Name ILIKE '%' || partialName || '%';
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION PartialSearchHall(partialName VARCHAR)
RETURNS TABLE (Id INTEGER, Capacity INTEGER, Price NUMERIC(8, 2))
AS $$
BEGIN
    RETURN QUERY
    SELECT h.Id, h.Capacity, h.Price
    FROM Hall AS h
    WHERE h.Name ILIKE '%' || partialName || '%';
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION PartialSearchCeremony(partialName VARCHAR)
RETURNS TABLE (Id INTEGER, Contact TEXT, NameOfCompany TEXT, Description TEXT)
AS $$
BEGIN
    RETURN QUERY
    SELECT c.Id, c.Contact, c.NameOfCompany, c.Description
    FROM Ceremony AS c
    WHERE c.NameOfCompany ILIKE '%' || partialName || '%';
END;
$$ LANGUAGE plpgsql;