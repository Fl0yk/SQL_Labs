using Crematorium.Domain.Entities;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace CrematoriumPostgre
{
    internal class Requests
    {
        private readonly NpgsqlDataSource _source;
        public Requests()
        {
            string connectionString = "Host=localhost;Username=postgres;Password=moooooo2242003;Database=pgsql";
            _source = NpgsqlDataSource.Create(connectionString);
        }

        #region Hall CRUD
        public async Task<List<Hall>> GetAllHall()
        {

            List<Hall> res = new();
            _source.OpenConnection();
            await using var command = _source.CreateCommand($"SELECT * FROM Hall;");
            
            var reader = command.ExecuteReader();
            while (await reader.ReadAsync())
            {
                var id = (int)reader["Id"];
                var capacity = (int)reader["Capacity"];
                var price = (decimal)reader["Price"];

                res.Add(new Hall() { Id = id, Capacity = capacity, Price = price });
            }

            return res;
        }

        public async Task<List<Hall>> PartialSearchHall(string partialName)
        {
            _source.OpenConnection();
            await using var command = _source.CreateCommand($"SELECT * FROM PartialSearchHall('{partialName}')");

            var result = new List<Hall>();
            var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var id = (int)reader["Id"];
                var capacity = (int)reader["Capacity"];
                var price = (decimal)reader["Price"];

                result.Add(new Hall() { Id = id, Capacity = capacity, Price = price });
            }

            return result;
        }

        public async Task<List<Hall>> GetHallWithDates()
        {
            List<Hall> res = new();
            _source.OpenConnection();

            await using var command = _source.CreateCommand($"SELECT h.Id, h.Capacity, h.Price, d.date " +
                                                            $"FROM Hall AS h " +
                                                            $"LEFT OUTER JOIN Dates AS d ON h.Id=d.HallId " +
                                                            $"ORDER BY h.Id;");

            var reader = command.ExecuteReader();
            int i = -1;
            while (await reader.ReadAsync())
            {
                int id = (int)reader["Id"];
                if(id == i)
                {
                    var tmp = reader["date"];
                    if (tmp is DBNull)
                        continue;
                    var date = DateOnly.FromDateTime((DateTime)tmp);
                    res.Last().FreeDates.Add(date);
                }
                else
                {
                    var capacity = (int)reader["Capacity"];
                    var price = (decimal)reader["Price"];

                    res.Add(new Hall() { Id = id, Capacity = capacity, Price = price, FreeDates = new() });

                    var tmp = reader["date"];
                    if(tmp is DBNull) 
                        continue;
                    var date = DateOnly.FromDateTime((DateTime)tmp);
                    res.Last().FreeDates.Add(date);

                    i = id;
                }
            }

            return res;
        }

        public async Task<bool> CreateHall(int capcity, decimal price)
        {
            _source.OpenConnection();
            await using var command = _source.CreateCommand($"CALL CreateHall('{capcity}', " +
                                                $"'{price.ToString(CultureInfo.GetCultureInfo("en-US"))}');");

            try
            {
                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync("Error: " + ex.Message);
                return false;
            }
        }

        public async Task<bool> AddFreeDate(int id, DateOnly date)
        {
            _source.OpenConnection();
            await using var command = _source.CreateCommand($"INSERT INTO Dates (HallId, Date) VALUES ({id}, date('{date:yyyy-MM-dd}'));");

            try
            {
                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync("Error: " + ex.Message);
                return false;
            }
        }

        public async Task<bool> UpdateHall(int id, int capcity, decimal price)
        {
            _source.OpenConnection();
            await using var command = _source.CreateCommand($"CALL UpdateHall('{id}', '{capcity}', " +
                                                $"'{price.ToString(CultureInfo.GetCultureInfo("en-US"))}');");

            try
            {
                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync("Error: " + ex.Message);
                return false;
            }
        }

        public async Task<bool> DeleteHallById(int id)
        {
            _source.OpenConnection();
            await using var command = _source.CreateCommand($"CALL DeleteHallById('{id}');");

            try
            {
                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync("Error: " + ex.Message);
                return false;
            }
        }
        #endregion

        #region Coffin CRUD

        public async Task<List<Coffin>> GetAllCoffins()
        {
            List<Coffin> result = new List<Coffin>();
            _source.OpenConnection();
            await using var command = _source.CreateCommand("SELECT * FROM Coffin;");

            var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var id = (int)reader["Id"];
                var name = reader["Name"].ToString();
                var price = (decimal)reader["Price"];
                var imagePath = reader["ImagePath"].ToString();

                result.Add(new Coffin() { Id = id, Name = name, Price = price, Image = imagePath });
            }

            return result;
        }

        //Частичный поиск гроба
        public async Task<List<Coffin>> PartialSearchCoffin(string partialName)
        {
            _source.OpenConnection();
            await using var command = _source.CreateCommand($"SELECT * FROM PartialSearchCoffin('{partialName}')");

            var result = new List<Coffin>();
            var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var id = (int)reader["Id"];
                var name = reader["Name"].ToString();
                var price = (decimal)reader["Price"];
                var imagePath = reader["ImagePath"].ToString();

                result.Add(new Coffin() { Id = id, Name = name, Price = price, Image = imagePath });
            }

            return result;
        }

        // Создание гроба
        public async Task<bool> CreateCoffin(string coffinName, decimal coffinPrice, string imagePath)
        {
            _source.OpenConnection();
            await using var command = _source.CreateCommand($"CALL CreateCoffin('{coffinName}', {coffinPrice.ToString(CultureInfo.GetCultureInfo("en-US"))}, '{imagePath}');");

            try
            {
                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync("Error: " + ex.Message);
                return false;
            }
        }

        // Обновление гроба
        public async Task<bool> UpdateCoffin(int coffinId, string newCoffinName, decimal newCoffinPrice, string newImagePath)
        {
            _source.OpenConnection();
            await using var command = _source.CreateCommand($"CALL UpdateCoffin({coffinId}, '{newCoffinName}', {newCoffinPrice.ToString(CultureInfo.GetCultureInfo("en-US"))}, '{newImagePath}');");

            try
            {
                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync("Error: " + ex.Message);
                return false;
            }
        }

        // Удаление гроба по ID
        public async Task<bool> DeleteCoffinById(int coffinId)
        {
            _source.OpenConnection();
            await using var command = _source.CreateCommand($"CALL DeleteCoffinById({coffinId});");

            try
            {
                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync("Error: " + ex.Message);
                return false;
            }
        }
        #endregion

        #region Corpose CRUD

        //получение трупа по номеру паспорта
        public async Task<Corpose> GetCorposeByPassport(string passport)
        {
            _source.OpenConnection();
            await using var command = _source.CreateCommand($"SELECT Id, Name, Surname, NumPassport FROM Corpose " +
                                                            $"WHERE NumPassport='{passport}';");

            Corpose corpose = new();
            try
            {
                var reader = await command.ExecuteReaderAsync();
                if(reader.Read())
                {
                    corpose.Id = (int)reader["Id"];
                    corpose.Name = reader["Name"].ToString();
                    corpose.SurName = reader["Surname"].ToString();
                    corpose.NumPassport = reader["NumPassport"].ToString();
                }
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync("Error: " + ex.Message);
            }

            return corpose;
        }

        // Создание трупа
        public async Task<bool> CreateCorpse(string corpseName, string corpseSurname, string numPassport)
        {
            _source.OpenConnection();
            await using var command = _source.CreateCommand($"CALL CreateCorpse('{corpseName}', '{corpseSurname}', '{numPassport}');");

            try
            {
                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync("Error: " + ex.Message);
                return false;
            }
        }

        // Обновление трупа
        public async Task<bool> UpdateCorpse(int corpseId, string newCorpseName, string newCorpseSurname, string newNumPassport)
        {
            _source.OpenConnection();
            await using var command = _source.CreateCommand($"CALL UpdateCorpse({corpseId}, '{newCorpseName}', '{newCorpseSurname}', '{newNumPassport}');");

            try
            {
                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync("Error: " + ex.Message);
                return false;
            }
        }

        // Удаление трупа по ID
        public async Task<bool> DeleteCorpseById(int corpseId)
        {
            _source.OpenConnection();
            await using var command = _source.CreateCommand($"CALL DeleteCorpseById({corpseId});");

            try
            {
                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync("Error: " + ex.Message);
                return false;
            }
        }
        #endregion

        #region User CRUD

        public async Task<List<User>> GetAllUsers()
        {
            List<User> result = new List<User>();
            _source.OpenConnection();
            await using var command = _source.CreateCommand("SELECT * FROM Users;");

            var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var id = (int)reader["Id"];
                var name = reader["Name"].ToString();
                var surname = reader["Surname"].ToString();
                var emailAddress = reader["EmailAdress"].ToString();
                var roleCode = reader["RoleCode"] != DBNull.Value ? (int)reader["RoleCode"] : 0;
                var numPassport = reader["NumPassport"].ToString();

                result.Add(new User() { Id = id, Name = name, Surname = surname,MailAdress = emailAddress, UserRole = (Role)roleCode, NumPassport = numPassport });
            }

            return result;
        }

        // Создание пользователя
        public async Task<bool> CreateUser(string userName, string userSurname, string emailAddress, int roleCode, string numPassport)
        {
            _source.OpenConnection();
            await using var command = _source.CreateCommand($"CALL CreateUser('{userName}', '{userSurname}', '{emailAddress}', {roleCode}, '{numPassport}');");

            try
            {
                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync("Error: " + ex.Message);
                return false;
            }
        }



        // Обновление пользователя
        public async Task<bool> UpdateUser(int userId, string newUserName, string newUserSurname, string newEmailAddress, int newRoleCode)
        {
            _source.OpenConnection();
            await using var command = _source.CreateCommand($"CALL UpdateUser({userId}, '{newUserName}', '{newUserSurname}', '{newEmailAddress}', {newRoleCode});");

            try
            {
                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync("Error: " + ex.Message);
                return false;
            }
        }

        // Обновление пользователя без изменения роли
        public async Task<bool> UpdateUser(int userId, string newUserName, string newUserSurname, string newEmailAddress)
        {
            _source.OpenConnection();
            await using var command = _source.CreateCommand($"CALL UpdateUserWithoutRole({userId}, '{newUserName}', '{newUserSurname}', '{newEmailAddress}');");

            try
            {
                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync("Error: " + ex.Message);
                return false;
            }
        }

        // Удаление пользователя по ID
        public async Task<bool> DeleteUserById(int userId)
        {
            _source.OpenConnection();
            await using var command = _source.CreateCommand($"CALL DeleteUserById({userId});");

            try
            {
                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync("Error: " + ex.Message);
                return false;
            }
        }

        #endregion

        #region Ritual urn CRUD

        public async Task<List<RitualUrn>> GetAllRitualUrns()
        {
            List<RitualUrn> result = new List<RitualUrn>();
            _source.OpenConnection();
            await using var command = _source.CreateCommand("SELECT * FROM RitualUrn;");

            var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var id = (int)reader["Id"];
                var name = reader["Name"].ToString();
                var price = (decimal)reader["Price"];
                var imagePath = reader["ImagePath"].ToString();

                result.Add(new RitualUrn() { Id = id, Name = name, Price = price, Image = imagePath });
            }

            return result;
        }

        //Частичный поиск по имени урны
        public async Task<List<RitualUrn>> PartialSearchRitualUrn(string partialName)
        {
            _source.OpenConnection();
            await using var command = _source.CreateCommand($"SELECT * FROM PartialSearchRitualUrn('{partialName}')");

            var result = new List<RitualUrn>();
            var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var id = (int)reader["Id"];
                var name = reader["Name"].ToString();
                var price = (decimal)reader["Price"];
                var imagePath = reader["ImagePath"].ToString();

                result.Add(new RitualUrn() { Id = id, Name = name, Price = price, Image = imagePath });
            }

            return result;
        }

        // Создание ритуальной урны
        public async Task<bool> CreateRitualUrn(string urnName, decimal urnPrice, string imagePath)
        {
            _source.OpenConnection();
            await using var command = _source.CreateCommand($"CALL CreateRitualUrn('{urnName}', {urnPrice.ToString(CultureInfo.GetCultureInfo("en-US"))}, '{imagePath}');");

            try
            {
                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync("Error: " + ex.Message);
                return false;
            }
        }

        // Обновление ритуальной урны
        public async Task<bool> UpdateRitualUrn(int urnId, string newUrnName, decimal newUrnPrice, string newImagePath)
        {
            _source.OpenConnection();
            await using var command = _source.CreateCommand($"CALL UpdateRitualUrn({urnId}, '{newUrnName}', {newUrnPrice.ToString(CultureInfo.GetCultureInfo("en-US"))}, '{newImagePath}');");

            try
            {
                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync("Error: " + ex.Message);
                return false;
            }
        }

        // Удаление ритуальной урны по ID
        public async Task<bool> DeleteRitualUrnById(int urnId)
        {
            _source.OpenConnection();
            await using var command = _source.CreateCommand($"CALL DeleteRitualUrnById({urnId});");

            try
            {
                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync("Error: " + ex.Message);
                return false;
            }
        }
        #endregion

        #region Ceremony CRUD

        public async Task<List<Ceremony>> GetAllCeremonies()
        {
            List<Ceremony> result = new List<Ceremony>();
            _source.OpenConnection();
            await using var command = _source.CreateCommand("SELECT * FROM Ceremony;");

            var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var id = (int)reader["Id"];
                var contact = reader["Contact"].ToString();
                var nameOfCompany = reader["NameOfCompany"].ToString();
                var description = reader["Description"].ToString();

                result.Add(new Ceremony() { Id = id, Contact = contact, NameOfCompany = nameOfCompany, Description = description });
            }

            return result;
        }

        //Частичный поиск по названию компании
        public async Task<List<Ceremony>> PartialSearchCeremonyByName(string partialName)
        {
            _source.OpenConnection();
            await using var command = _source.CreateCommand($"SELECT * FROM PartialSearchCeremony('{partialName}')");

            var result = new List<Ceremony>();
            var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var id = (int)reader["Id"];
                var contact = reader["Contact"].ToString();
                var nameOfCompany = reader["NameOfCompany"].ToString();
                var description = reader["Description"].ToString();

                result.Add(new Ceremony() { Id = id, Contact = contact, NameOfCompany = nameOfCompany, Description = description });
            }

            return result;
        }

        // Создание церемонии
        public async Task<bool> CreateCeremony(string contact, string nameOfCompany, string description)
        {
            _source.OpenConnection();
            await using var command = _source.CreateCommand($"CALL CreateCeremony('{contact}', '{nameOfCompany}', '{description}');");

            try
            {
                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync("Error: " + ex.Message);
                return false;
            }
        }

        // Обновление церемонии
        public async Task<bool> UpdateCeremony(int ceremonyId, string newContact, string newNameOfCompany, string newDescription)
        {
            _source.OpenConnection();
            await using var command = _source.CreateCommand($"CALL UpdateCeremony({ceremonyId}, '{newContact}', '{newNameOfCompany}', '{newDescription}');");

            try
            {
                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync("Error: " + ex.Message);
                return false;
            }
        }

        // Удаление церемонии по ID
        public async Task<bool> DeleteCeremonyById(int ceremonyId)
        {
            _source.OpenConnection();
            await using var command = _source.CreateCommand($"CALL DeleteCeremonyById({ceremonyId});");

            try
            {
                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync("Error: " + ex.Message);
                return false;
            }
        }
        #endregion

        #region Order CRUD
        //получение краткой информации о заказах
        public async Task<List<Order>> GetShortOrders()
        {
            _source.OpenConnection();
            await using var command = _source.CreateCommand($"SELECT Id, DateOfActual, StateCode, " +
                                    $"CorposeName, CorposeSurname, " +
                                    $"HallNumber, UrnName, CoffinName, " +
                                    $"UserId, Name, Surname, NumPassport " +
                                    $"FROM ShortOrders; ");

            List<Order> orders = new();
            var reader = command.ExecuteReader();
            try
            {
                while( reader.Read() )
                {
                    Order order = new();

                    order.Id = (int)reader["Id"];
                    order.DateOfActual = DateTime.Parse(reader["DateOfActual"].ToString()!);
                    order.State = (StateOrder)reader["StateCode"];

                    Corpose corpose = new()
                    {
                        Name = reader["CorposeName"].ToString() ?? String.Empty,
                        SurName = reader["CorposeSurname"].ToString() ?? String.Empty
                    };

                    Hall hall = new();
                    if (reader["HallNumber"] is int hallId)
                    {
                        hall.Id = hallId;
                    }

                    RitualUrn urn = new();
                    if (reader["UrnName"] is string urnName)
                    {
                        urn.Name = urnName;
                    }

                    Coffin coffin = new();
                    if (reader["CoffinName"] is string coffinName)
                    {
                        coffin.Name = coffinName;
                    }

                    User user = new();
                    if (reader["UserId"] is int userId)
                    {
                        user.Id = userId;
                        user.Name = reader["Name"].ToString();
                        user.Surname = reader["Surname"].ToString();
                        user.NumPassport = reader["NumPassport"].ToString();
                    }

                    order.Customer = user;
                    order.RitualUrnId = urn;
                    order.CorposeId = corpose;
                    order.HallId = hall;
                    order.CoffinId = coffin;

                    orders.Add(order);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return orders;
        }


        // Создание заказа
        public async Task<bool> CreateOrder(int hallId, DateTime actualDate, int corposeId, int coffinId, int userId, int urnId, int stateCode)
        {
            _source.OpenConnection();
            await using var command = _source.CreateCommand($"CALL CreateOrder(date('{actualDate:yyyy-MM-dd}'), {hallId}, {corposeId}, {coffinId}, {userId}, {urnId}, {stateCode});");

            try
            {
                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync("Error: " + ex.Message);
                return false;
            }
        }

        //Изменение статуса заказа
        public async Task ChangeState(int orderId, StateOrder newState)
        {
            _source.OpenConnection();
            await using var command = _source.CreateCommand($"CALL UpdateOrderStatus({orderId}, {(int)newState});");

            try
            {
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync("Error: " + ex.Message);
            }
        }

        public async Task<StateOrder> GetStateOrder(int orderId)
        {
            _source.OpenConnection();
            await using var command = _source.CreateCommand($"SELECT StateCode FROM Orders WHERE Id={orderId};");
            var res = command.ExecuteScalar();
            return (StateOrder)(res is DBNull ? 0 : res);
        }

        //добавить церемоний к заказу
        public async Task AddCeremoniesToOrder(int orderId, int[] ceremonyIds)
        {
            _source.OpenConnection();
            try
            {
                foreach (var ceremonyId in ceremonyIds)
                {
                    await using var insertCommand = _source
                        .CreateCommand($"INSERT INTO OrderCeremony (OrderId, CeremonyId) VALUES ({orderId}, {ceremonyId})");

                    await insertCommand.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        //Просмотр всей информации о заказе
        public async Task<Order> GetFullOrderById(int orderId)
        {
            _source.OpenConnection();
            await using var command = _source.CreateCommand($"SELECT Id, DateOfActual, StateCode, " +
                $"CorposeName, CorposeSurname, CoproseNumpassport, " +
                $"HallNumber, HallCapacity, HallPrice, " +
                $"UrnName, UrnPrice, " +
                $"CoffinName, CoffinPrice " +
                $"FROM FullOrders WHERE Id = {orderId}; ");


            Order order = new();
            var reader = command.ExecuteReader();
            try
            {
                if(!await reader.ReadAsync())
                {
                    throw new SqlNullValueException("Заказа не существует");
                }

                order.Id = (int)reader["Id"];
                order.DateOfActual = DateTime.Parse(reader["DateOfActual"].ToString()!);
                order.State = (StateOrder)reader["StateCode"];

                Corpose corpose = new() {
                    Name = reader["CorposeName"].ToString() ?? String.Empty,
                    SurName = reader["CorposeSurname"].ToString() ?? String.Empty,
                    NumPassport = reader["CoproseNumpassport"].ToString() ?? String.Empty
                };

                Hall hall = new();                
                if(reader["HallNumber"] is int hallId)
                {
                    hall.Id = hallId;
                    hall.Price = (decimal)reader["HallPrice"];
                    hall.Capacity = (int)reader["HallCapacity"];
                }

                RitualUrn urn = new();
                if (reader["UrnName"] is string urnName)
                {
                    urn.Name = urnName;
                    urn.Price = (decimal)reader["UrnPrice"];
                }

                Coffin coffin = new();
                if (reader["CoffinName"] is string coffinName)
                {
                    coffin.Name = coffinName;
                    coffin.Price = (decimal)reader["CoffinPrice"];
                }

                order.RitualUrnId = urn;
                order.CorposeId = corpose;
                order.HallId = hall;
                order.CoffinId = coffin;

                order.Ceremonies = GetCeremonies(orderId).Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return order;
        }

        private async Task<List<Ceremony>> GetCeremonies(int orderId)
        {
            List<Ceremony> res = new();

            _source.OpenConnection();
            await using var command = _source.CreateCommand($"SELECT o.Id, c.Id AS CeremonyId, c.NameOfCompany AS Name, " +
                                                            $"c.Contact AS Contact, c.Description AS Description " +
                                                            $"FROM Orders AS o " +
                                                            $"LEFT JOIN OrderCeremony AS oc ON oc.OrderId=o.Id " +
                                                            $"LEFT JOIN Ceremony AS c ON oc.CeremonyId=c.Id " +
                                                            $"WHERE o.Id = {orderId}; ");
            try
            {
                var reader = command.ExecuteReader();

                while(reader.Read())
                {
                    var id = reader["CeremonyId"];
                    if (id is DBNull)
                        return res;
                    res.Add(new Ceremony()
                    {
                        Id = (int)id,
                        NameOfCompany = reader["Name"].ToString() ?? string.Empty,
                        Contact = reader["Contact"].ToString() ?? string.Empty,
                        Description = reader["Description"].ToString() ?? string.Empty,
                    });
                }
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync("Error: " + ex.Message);
            }

            return res;
        }

        #endregion

        #region Authorization

        public async Task<bool> IsExist(string name, string numPassport)
        {
            _source.OpenConnection();
            await using var command = _source.CreateCommand($"SELECT * FROM IsExistUser('{name}', '{numPassport}');");
            var res = command.ExecuteScalar();
            return (bool)res;
        }

        public async Task<User> GetUserByNameAndPassport(string name, string passportNumber)
        {
            _source.OpenConnection();

            try
            {
                await using var command = _source.CreateCommand($"SELECT * FROM Users WHERE Name = '{name}' AND NumPassport = '{passportNumber}'");
                var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    var id = (int)reader["Id"];
                    var userName = reader["Name"].ToString();
                    var surname = reader["Surname"].ToString();
                    var emailAddress = reader["EmailAdress"].ToString();
                    var roleCode = reader["RoleCode"] is DBNull ? 0 : (int)reader["RoleCode"];
                    var numPassport = reader["NumPassport"].ToString();

                    return new User
                    {
                        Id = id,
                        Name = userName,
                        Surname = surname,
                        MailAdress = emailAddress,
                        UserRole = (Role)roleCode,
                        NumPassport = numPassport
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return new User();
        }

        #endregion


    }
}
