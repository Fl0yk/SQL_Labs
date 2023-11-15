INSERT INTO Role (Name) VALUES
('Customer'),
('Admin'),
('Employee');

INSERT INTO Users (Name, Surname, EmailAdress, RoleCode, NumPassport) VALUES
('Dima', 'Kosach', 'kosach.dmitriy@mail.ru', 2, '11111111111111'),
('Biba', 'Bibkin', 'biba.bibkin@mail.ru', 1, '22222222222222'),
('Boba', 'Bobikov', 'boba_bobikov@mail.ru', 3, '33333333333333');

INSERT INTO RitualUrn (Name, Price, ImagePath) VALUES
('Urn 1', ROUND(12.345, 2), ''),
('Urn 2', ROUND(24, 2), ''),
('Urn 3', ROUND(16.5, 2), ''),
('Urn 4', ROUND(20, 2), '');

INSERT INTO Corpose (Name, Surname, NumPassport) VALUES
('Drowned', 'Corpose', '12222222222222'),
('Suicidal', 'Corpose', '13333333333333'),
('Suicidal2', 'Corpose', '43333333333333'),
('Suicidal3', 'Corpose', '53333333333333');

INSERT INTO Hall (Capacity, Price) VALUES
(120, ROUND(160.99, 2)),
(180, ROUND(220, 2));

INSERT INTO Dates (HallId, Date) VALUES
(1, date(CURRENT_DATE)),
(1, date('2023-10-17')),
(2, date('2023-10-17')),
(2, date('2023-10-18'));

INSERT INTO Coffin (Name, Price, ImagePath) VALUES
('Coffin 1', ROUND(72.345, 2), ''),
('Coffin 2', ROUND(84, 2), ''),
('Coffin 3', ROUND(76.5, 2), ''),
('Coffin 4', ROUND(80, 2), '');

INSERT INTO Ceremony (Contact, NameOfCompany, Description) VALUES
('company1@mail.ru', 'Company 1', 'Description 1'),
('company2@gmail.com', 'Company 2', 'Description 2'),
('company3@mail.ru', 'Company 3', 'Description 3');

INSERT INTO StateOrder (Name) VALUES
('Decorated'),
('Approved'),
('Closed'),
('Cancelled');

INSERT INTO Orders (DateOfActual, HallId, CorposeId, CoffinId, UserId, UrnId, StateCode) VALUES
(CURRENT_DATE, 1, 3, 9, 2, 1, 1),
(date('2023-10-17'), 2, 4, 10, 1, 2, 1);

INSERT INTO OrderCeremony (OrderId, CeremonyId) VALUES
(1, 1),
(1, 2),
(2, 2);