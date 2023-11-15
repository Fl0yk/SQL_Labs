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
    FROM Users AS u WHERE u.Id = NEW.Id;

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
    surnameArg VARCHAR,
    numPassportArg CHAR
) RETURNS BOOLEAN AS $$
DECLARE isExists BOOLEAN;
BEGIN
    SELECT EXISTS(
        SELECT 1 FROM Users AS u 
        WHERE u.Surname = surnameArg AND u.numPassport = numPassportArg)
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

INSTEAD OF -- вместо операции (только на уровне строк представлений)
FOR EACH STATEMEN -- на событие, а не на строку