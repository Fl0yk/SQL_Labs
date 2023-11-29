using Crematorium.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CrematoriumPostgre
{
    internal class Menu
    {
        public static User CurUser { get; set; } = new();
        private Requests requests = new Requests();
        public void GetMainMenu()
        {


            int select;
            do
            {
                Console.Clear();
                int max = 7;
                Console.WriteLine("1. Завершить программу");
                Console.WriteLine("2. Меню залов");
                Console.WriteLine("3. Меню гробов");
                Console.WriteLine("4. Меню церемоний");
                Console.WriteLine("5. Меню урн");
                Console.WriteLine("6. Авторизация");
                Console.WriteLine("7. Регистрация");

                if (CurUser.UserRole >= Role.Customer)
                {
                    Console.WriteLine("8. Профиль");
                    max = 8;
                }

                if (CurUser.UserRole == Role.Employee || CurUser.UserRole == Role.Admin)
                {
                    Console.WriteLine("9. Меню заказов");
                    max = 9;
                }

                if(CurUser.UserRole == Role.Admin)
                {
                    Console.WriteLine("10. Меню пользователей");
                    max = 10;
                }

                Console.WriteLine();
                select = CheckInput(max);
                Console.WriteLine();
                

                switch (select)
                {
                    case 2:
                        GetHallMenu();
                        break;
                    case 3:
                        GetCoffinMenu();
                        break;
                    case 4:
                        GetCeremonyMenu();
                        break;
                    case 5:
                        GetRitualUrnMenu();
                        break;
                    case 6:
                        Authorize();
                        break;
                    case 7:
                        Registration();
                        break;
                    case 8:
                        GetUserMenu();
                        break;
                    case 9:
                        GetWorkMenu();
                        break;
                    case 10:
                        GetUserChangeMenu();
                        break;
                }
            } while (select != 1);
        }

        #region HallMenu
        public void GetHallMenu()
        {
            Console.Clear();

            int select;

            do
            {
                int max = 2;
                Console.WriteLine($"1. Вернуться");
                Console.WriteLine($"2. Увидеть все залы с датами");
                if (CurUser.UserRole == Role.Admin)
                {
                    Console.WriteLine($"3. Увидеть все залы");
                    Console.WriteLine($"4. Добавить зал");
                    Console.WriteLine($"5. Удалить зал по Id");
                    Console.WriteLine($"6. Обновить зал");
                    Console.WriteLine($"7. Добавить дату");
                    Console.WriteLine($"8. Частичный поиск");
                    max = 8;
                }
                Console.WriteLine();
                select = CheckInput(max);

                switch (select)
                {
                    case 2:
                        foreach (var hall in requests.GetHallWithDates().Result)
                        {
                            Console.WriteLine($"Номер зала(id): {hall.Id} , вместительность {hall.Capacity} человек, цена {hall.Price} руб.");
                            if(hall.FreeDates.Count == 0)
                                Console.WriteLine("\tСвободных дат нет\n");
                            else
                            {
                                foreach (var hallDate in hall.FreeDates)
                                {
                                    Console.WriteLine($"\tСвободная дата: {hallDate}"); 
                                }
                                Console.WriteLine();
                            }
                        }
                        break;
                    case 3:
                        foreach (var hall in requests.GetAllHall().Result)
                        {
                            Console.WriteLine($"Номер зала(id): {hall.Id} , вместительность {hall.Capacity} человек, цена {hall.Price} руб.");
                        }
                        break;
                    case 4:
                        SetHallParam(out int cap, out decimal price);
                        requests.CreateHall(cap, price).Wait();
                        break;
                    case 5:
                        int id = GetId();
                        requests.DeleteHallById(id).Wait();
                        break;
                    case 6:
                        id = GetId();
                        SetHallParam(out cap, out price);
                        requests.UpdateHall(id, cap, price).Wait();
                        break;
                    case 7:
                        id = GetId();
                        DateOnly date;
                        Console.WriteLine("Введите дату формата YYYY-MM-DD");
                        while(!DateOnly.TryParse(Console.ReadLine(), out date))
                        {
                            Console.WriteLine("Некорректный ввод. Попробуйте еще раз: ");
                        }
                        requests.AddFreeDate(id, date).Wait();
                        break;
                    case 8:
                        Console.WriteLine("Введите часть имени:");
                        string part = CheckString();
                        foreach (var hall in requests.PartialSearchHall(part).Result)
                        {
                            Console.WriteLine($"Номер зала(id): {hall.Id} , вместительность {hall.Capacity} человек, цена {hall.Price} руб.");
                        }
                        break;
                }
                Console.WriteLine("==========================================================================\n");
            } while (select != 1);

            Console.Clear();
        }

        private void SetHallParam(out int capacity, out decimal price)
        {
            Console.WriteLine("Введите вместительность зала: ");
            while(!int.TryParse(Console.ReadLine(), out capacity))
            {
                Console.WriteLine("Некорректный ввод. Повторите попытку: ");
            }

            Console.WriteLine("Введите стоимость зала: ");
            while (!decimal.TryParse(Console.ReadLine(), out price))
            {
                Console.WriteLine("Некорректный ввод. Повторите попытку: ");
            }
        }

        #endregion

        #region CoffinMenu
        public void GetCoffinMenu()
        {
            Console.Clear();

            int select;

            do
            {
                Console.WriteLine($"1. Вернуться");
                Console.WriteLine($"2. Увидеть все гробы");
                if (CurUser.UserRole == Role.Admin)
                {
                    Console.WriteLine($"3. Добавить гроб");
                    Console.WriteLine($"4. Удалить гроб по Id");
                    Console.WriteLine($"5. Обновить гроб");
                    Console.WriteLine($"6. Частичный поиск");
                }
                Console.WriteLine();
                select = CheckInput(6);

                switch (select)
                {
                    case 2:
                        foreach (var coffin in requests.GetAllCoffins().Result)
                        {
                            Console.WriteLine($"Номер гроба(id): {coffin.Id}, название: {coffin.Name}, цена: {coffin.Price} руб.");
                        }
                        break;
                    case 3:
                        SetCoffinParam(out string name, out decimal price);
                        requests.CreateCoffin(name, price, "C:").Wait();
                        break;
                    case 4:
                        int id = GetId();
                        requests.DeleteCoffinById(id).Wait();
                        break;
                    case 5:
                        id = GetId();
                        SetCoffinParam(out name, out price);
                        requests.UpdateCoffin(id, name, price, "C:").Wait();
                        break;
                    case 6:
                        Console.WriteLine("Введите часть имени:");
                        string part = CheckString();
                        foreach (var coffin in requests.PartialSearchCoffin(part).Result)
                        {
                            Console.WriteLine($"Номер гроба(id): {coffin.Id}, название: {coffin.Name}, цена: {coffin.Price} руб.");
                        }
                        break;

                }
                Console.WriteLine("==========================================================================\n");
            } while (select != 1);

            Console.Clear();
        }

        private void SetCoffinParam(out string name, out decimal price)
        {
            Console.WriteLine("Введите название гроба: ");
            name = CheckString();

            Console.WriteLine("Введите цену гроба: ");
            while (!decimal.TryParse(Console.ReadLine(), out price))
            {
                Console.WriteLine("Некорректный ввод. Повторите попытку: ");
            }
        }
        #endregion

        #region CeremonyMenu

        public void GetCeremonyMenu()
        {
            Console.Clear();

            int select;

            do
            {
                Console.WriteLine($"1. Вернуться");
                Console.WriteLine($"2. Увидеть все церемонии");
                if (CurUser.UserRole == Role.Admin)
                {
                    Console.WriteLine($"3. Добавить церемонию");
                    Console.WriteLine($"4. Удалить церемонию по Id");
                    Console.WriteLine($"5. Обновить церемонию");
                    Console.WriteLine($"6. Частичный поиск");
                }
                Console.WriteLine();
                select = CheckInput(5);

                switch (select)
                {
                    case 2:
                        foreach (var ceremony in requests.GetAllCeremonies().Result)
                        {
                            Console.WriteLine($"Номер церемонии(id): {ceremony.Id}, контакт: {ceremony.Contact}, название компании: {ceremony.NameOfCompany}, описание: {ceremony.Description}");
                        }
                        break;
                    case 3:
                        SetCeremonyParam(out string contact, out string nameOfCompany, out string description);
                        requests.CreateCeremony(contact, nameOfCompany, description).Wait();
                        break;
                    case 4:
                        int id = GetId();
                        requests.DeleteCeremonyById(id).Wait();
                        break;
                    case 5:
                        id = GetId();
                        SetCeremonyParam(out contact, out nameOfCompany, out description);
                        requests.UpdateCeremony(id, contact, nameOfCompany, description).Wait();
                        break;
                    case 6:
                        Console.WriteLine("Введите часть имени:");
                        string part = CheckString();
                        foreach (var ceremony in requests.PartialSearchCeremonyByName(part).Result)
                        {
                            Console.WriteLine($"Номер церемонии(id): {ceremony.Id}, контакт: {ceremony.Contact}, название компании: {ceremony.NameOfCompany}, описание: {ceremony.Description}");
                        }
                        break;
                }
                Console.WriteLine("==========================================================================\n");
            } while (select != 1);

            Console.Clear();
        }

        private void SetCeremonyParam(out string contact, out string nameOfCompany, out string description)
        {
            Console.WriteLine("Введите контакт для церемонии: ");
            contact = CheckString();

            Console.WriteLine("Введите название компании для церемонии: ");
            nameOfCompany = CheckString();

            Console.WriteLine("Введите описание церемонии: ");
            description = CheckString();
        }

        #endregion

        #region RitualUrnMenu

        public void GetRitualUrnMenu()
        {
            Console.Clear();

            int select;

            do
            {
                Console.WriteLine($"1. Вернуться");
                Console.WriteLine($"2. Увидеть все ритуальные урны");
                if (CurUser.UserRole == Role.Admin)
                {
                    Console.WriteLine($"3. Добавить ритуальную урну");
                    Console.WriteLine($"4. Удалить ритуальную урну по Id");
                    Console.WriteLine($"5. Обновить ритуальную урну");
                    Console.WriteLine($"6. Частичный поиск");
                }
                Console.WriteLine();
                select = CheckInput(6);

                switch (select)
                {
                    case 2:
                        foreach (var urn in requests.GetAllRitualUrns().Result)
                        {
                            Console.WriteLine($"Номер урны(id): {urn.Id}, название: {urn.Name}, цена: {urn.Price} руб.");
                        }
                        break;
                    case 3:
                        SetRitualUrnParam(out string name, out decimal price);
                        requests.CreateRitualUrn(name, price, "C:").Wait();
                        break;
                    case 4:
                        int id = GetId();
                        requests.DeleteRitualUrnById(id).Wait();
                        break;
                    case 5:
                        id = GetId();
                        SetRitualUrnParam(out name, out price);
                        requests.UpdateRitualUrn(id, name, price, "C:").Wait();
                        break;
                    case 6:
                        Console.WriteLine("Введите часть имени:");
                        string part = CheckString();
                        foreach (var urn in requests.PartialSearchRitualUrn(part).Result)
                        {
                            Console.WriteLine($"Номер урны(id): {urn.Id}, название: {urn.Name}, цена: {urn.Price} руб.");
                        }
                        break;
                }
                Console.WriteLine("==========================================================================\n");
            } while (select != 1);

            Console.Clear();
        }

        private void SetRitualUrnParam(out string name, out decimal price)
        {
            Console.WriteLine("Введите название ритуальной урны: ");
            name = CheckString();

            Console.WriteLine("Введите цену ритуальной урны: ");
            while (!decimal.TryParse(Console.ReadLine(), out price))
            {
                Console.WriteLine("Некорректный ввод. Повторите попытку: ");
            }
        }

        #endregion

        #region UserMenu
        public void GetUserChangeMenu()
        {
            int select;

            do
            {
                Console.Clear();
                Console.WriteLine("1. Вернуться");
                Console.WriteLine("2. Увидеть всех пользователей");
                Console.WriteLine("3. Добавить пользователя");
                Console.WriteLine("4. Удалить пользователя по Id");
                Console.WriteLine("5. Обновить пользователя");
                Console.WriteLine();
                select = CheckInput(5);

                switch (select)
                {
                    case 2:
                        ViewAllUsers();
                        break;
                    case 3:
                        CreateUser();
                        break;
                    case 4:
                        DeleteUser();
                        break;
                    case 5:
                        UpdateUser();
                        break;
                }

                Console.WriteLine("==========================================================================\n");
            } while (select != 1);

            Console.Clear();
        }

        private void ViewAllUsers()
        {
            var users = requests.GetAllUsers().Result;
            Console.WriteLine("===== Все пользователи =====");
            foreach (var user in users)
            {
                Console.WriteLine($"ID пользователя: {user.Id}, Имя: {user.Name}, Фамилия: {user.Surname}, Email: {user.MailAdress}, Роль: {user.UserRole}, Паспорт: {user.NumPassport}");
            }
        }

        private void CreateUser()
        {
            Console.Write("Введите имя пользователя: ");
            var userName = Console.ReadLine();

            Console.Write("Введите фамилию пользователя: ");
            var userSurname = Console.ReadLine();

            Console.Write("Введите Email: ");
            var emailAddress = Console.ReadLine();

            Console.Write("Введите код роли: ");
            if (int.TryParse(Console.ReadLine(), out int roleCode))
            {
                Console.Write("Введите номер паспорта: ");
                var numPassport = Console.ReadLine();

                requests.CreateUser(userName, userSurname, emailAddress, roleCode, numPassport).Wait();
            }
            else
            {
                Console.WriteLine("Неверный код роли. Пожалуйста, введите корректный номер.");
            }
        }

        private void UpdateUser()
        {
            int userId = GetId();
            Console.Write("Введите новое имя пользователя: ");
            var newUserName = Console.ReadLine();

            Console.Write("Введите новую фамилию пользователя: ");
            var newUserSurname = Console.ReadLine();

            Console.Write("Введите новый Email: ");
            var newEmailAddress = Console.ReadLine();

            Console.Write("Введите новый код роли: ");
            if (int.TryParse(Console.ReadLine(), out int newRoleCode))
            {
                requests.UpdateUser(userId, newUserName, newUserSurname, newEmailAddress, newRoleCode).Wait();
            }
            else
            {
                Console.WriteLine("Неверный новый код роли. Пожалуйста, введите корректный номер.");
            }
        }

        private void DeleteUser()
        {
            int userId = GetId();
            requests.DeleteUserById(userId).Wait();
        }
        #endregion

        #region CreateOrderMenu
        public void CreateOrdeMenu()
        {
            Console.WriteLine("Сначала зарегестрируем погибшего");
            Corpose? corpose;
            do
            {
                corpose = CorposeRegistration();
            } while (corpose is null);

            Console.WriteLine("Доступные урны");
            foreach (var urn in requests.GetAllRitualUrns().Result)
            {
                Console.WriteLine($"Номер урны(id): {urn.Id}, название: {urn.Name}, цена: {urn.Price} руб.");
            }
            Console.WriteLine("\n");
            int urnId = GetId();

            Console.WriteLine("Доступные залы с датами");
            var halls = requests.GetHallWithDates().Result;
            foreach (var hall in halls)
            {
                Console.WriteLine($"Номер зала(id): {hall.Id} , вместительность {hall.Capacity} человек, цена {hall.Price} руб.");
                if (hall.FreeDates.Count == 0)
                    Console.WriteLine("\tСвободных дат нет\n");
                else
                {
                    int i = 1;
                    foreach (var hallDate in hall.FreeDates)
                    {
                        Console.WriteLine($"\t{i++}. Свободная дата: {hallDate}");
                    }
                    Console.WriteLine();
                }
            }
            Console.WriteLine("\n");
            int hallId = GetId();
            Hall curHall = halls.First(h => h.Id == hallId);

            int ind;
            Console.WriteLine("Выберете номер даты зала");
            while (!int.TryParse(Console.ReadLine(), out ind) || ind < 0 || ind > curHall.FreeDates.Count )
            {
                Console.WriteLine("Некорректный ввод. ПОпытайтесь еще раз:");
            }
            DateTime date = curHall.FreeDates[ind].ToDateTime(new TimeOnly());

            Console.WriteLine("Доступные гробы");
            foreach (var coffin in requests.GetAllCoffins().Result)
            {
                Console.WriteLine($"Номер гроба(id): {coffin.Id}, название: {coffin.Name}, цена: {coffin.Price} руб.");
            }
            int coffinId = GetId();
            Console.WriteLine("\n");

            if(!requests.CreateOrder(hallId, date, corpose.Id, coffinId, CurUser.Id, urnId, (int)StateOrder.Decorated).Result)
            {
                requests.DeleteCorpseById(corpose.Id).Wait();
            }
        }

        private Corpose? CorposeRegistration()
        {
            Console.WriteLine("Введите имя погибшего");
            string name = CheckString();

            Console.WriteLine("Введите фамилию погибшего");
            string surname = CheckString();

            Console.WriteLine("Введите номер паспорта");
            string passport = CheckString();

            if(requests.CreateCorpse(name, surname, passport).Result)
            {
                return requests.GetCorposeByPassport(passport).Result;
            }
            else
            {
                Console.WriteLine("\nЗарегистрируйте заново");
                return null;
            }
        }

        #endregion

        #region Authorize
        public void Authorize()
        {
            bool tryThis = true;
            
            while(tryThis)
            {
                string name;
                string passport;

                Console.Clear();
                Console.WriteLine("Введите имя пользователя: ");
                name = CheckString();
                Console.WriteLine("Введите номер паспорта: ");
                passport = CheckString();

                if(requests.IsExist(name, passport).Result)
                {
                    CurUser = requests.GetUserByNameAndPassport(name, passport).Result;
                    Console.Clear();
                    return;
                }
                else
                {
                    Console.WriteLine("Такого пользователя нет. Хотите повторить попытку (y/n)?");
                    tryThis = Console.ReadLine() == "y";
                }
            }
        }

        public void Registration()
        {
            bool tryThis = true;

            while (tryThis)
            {
                Console.Clear();

                Console.WriteLine("Введите имя: ");
                string name = CheckString();

                Console.WriteLine("Введите фамилию: ");
                string surname = CheckString();

                Console.WriteLine("Введите почту: ");
                string email = CheckString();

                Console.WriteLine("Введите номер паспорта: ");
                string numPassport = CheckString();

                if (requests.CreateUser(name, surname, email, (int)Role.Customer, numPassport).Result)
                {
                    CurUser = requests.GetUserByNameAndPassport(name, numPassport).Result;
                    return;
                }
                else
                {
                    //Console.Clear();
                    Console.WriteLine("Были введены некорректные данные. Хотите повторить попытку (y/n)?");
                    tryThis = Console.ReadLine() == "y";
                }
            }
        }
        #endregion

        #region UserMenu

        public void GetUserMenu()
        {
            Console.Clear();
            Console.WriteLine($"Имя: {CurUser.Name}");
            Console.WriteLine($"Фамилия: {CurUser.Surname}");
            Console.WriteLine($"Почта: {CurUser.MailAdress}");
            Console.WriteLine($"Роль: {CurUser.UserRole.ToString()}");
            Console.WriteLine("========================================");

            int select;
            do
            {
                Console.WriteLine();
                Console.WriteLine("1.Назад");
                Console.WriteLine("2. Изменить пользователя");
                Console.WriteLine("3. Просмотреть заказы");
                Console.WriteLine("4. Просмотреть подробно заказ");
                Console.WriteLine("5. Оформить заказ");
                Console.WriteLine("6. Отменить заказ");
                Console.WriteLine("7. Добавить церемоний");
                Console.WriteLine();

                select = CheckInput(7);

                switch (select)
                {
                    case 2:
                        SetUserParametr(out string name, out string surname, out string mail);
                        requests.UpdateUser(CurUser.Id, name, surname, mail).Wait();
                        CurUser = requests.GetUserByNameAndPassport(name, CurUser.NumPassport).Result;
                        Console.Clear();
                        Console.WriteLine($"Имя: {CurUser.Name}");
                        Console.WriteLine($"Фамилия: {CurUser.Surname}");
                        Console.WriteLine($"Почта: {CurUser.MailAdress}");
                        Console.WriteLine($"Роль: {nameof(CurUser.UserRole)}");
                        break;
                    case 3:
                        foreach(Order order in requests.GetShortOrders().Result.Where(o => o.Customer.Id == CurUser.Id))
                        {
                            Console.WriteLine($"Заказ: {order.Id}, Дата: {order.DateOfActual:yyyy-MM-dd} Статус: {order.State.ToString()}" +
                                $"\nИмя мертвого: {order.CorposeId.Name}, Фамилия: {order.CorposeId.SurName}," +
                                $"\nНомер зала: {order.HallId?.Id}, Урна: {order.RitualUrnId?.Name}, Гроб: {order.CoffinId?.Name}");
                        }
                        break;
                    case 4:
                        int id = GetId();
                        Order fo= requests.GetFullOrderById(id).Result;
                        Console.WriteLine($"Клиент: {CurUser.Name} {CurUser.Surname} ");
                        Console.WriteLine($"Дата: {fo.DateOfActual:yyyy-MM-dd}; Статус: {fo.State.ToString()}");
                        Console.WriteLine($"Погибший: {fo.CorposeId.Name} {fo.CorposeId.SurName} ");
                        Console.WriteLine($"Зал: {fo.HallId?.Id} {fo.HallId?.Capacity} человек за {fo.HallId?.Price} руб. ");
                        Console.WriteLine($"Урна: {fo.RitualUrnId?.Name} за {fo.RitualUrnId?.Price} ");
                        Console.WriteLine($"Гроб: {fo.CoffinId?.Name} за {fo.CoffinId?.Price} ");
                        Console.WriteLine($"Церемонии: ");
                        foreach (Ceremony ceremony in fo.Ceremonies)
                        {
                            Console.WriteLine($"\t{ceremony.NameOfCompany}\n\t{ceremony.Contact}\n\t{ceremony.Description}\n");
                        }
                        break;
                    case 5:
                        CreateOrdeMenu();
                        break;
                    case 6:
                        id = GetId();
                        CancelOrder(id);
                        break;
                    case 7:
                        id = GetId();
                        Console.WriteLine("Доступные церемонии");
                        foreach (var ceremony in requests.GetAllCeremonies().Result)
                        {
                            Console.WriteLine($"Номер церемонии(id): {ceremony.Id}, контакт: {ceremony.Contact}, название компании: {ceremony.NameOfCompany}, описание: {ceremony.Description}");
                        }
                        Console.WriteLine("\n");
                        Console.WriteLine("Введите через пробел номера церемоний");
                        requests.AddCeremoniesToOrder(id, Console.ReadLine().Split().Select(s => int.Parse(s)).ToArray());
                        break;
                }
                Console.WriteLine("==========================================================================\n");

            } while (select != 1);
            
        }

        private void SetUserParametr(out string name, out string surname, out string mail)
        {
            Console.WriteLine("Введите новое имя: ");
            name = CheckString();

            Console.WriteLine("Введите новую фамилию: ");
            surname = CheckString();

            Console.WriteLine("Введите новую почту: ");
            mail = CheckString();
        }

        #endregion

        #region WorkMenu
        public void GetWorkMenu()
        {
            Console.Clear();

            int select;
            do
            {
                int max = 7;
                Console.WriteLine("1. Вернуться");
                Console.WriteLine("2. Просмотреть все заказы");

                if (CurUser.UserRole == Role.Employee || CurUser.UserRole == Role.Admin)
                {
                    Console.WriteLine("3. выполнить заказ");
                    max = 3;
                }

                if (CurUser.UserRole == Role.Admin)
                {
                    Console.WriteLine("4. Одобрить заказ");
                    max = 4;
                }

                Console.WriteLine();
                select = CheckInput(max);
                Console.WriteLine();

                int id = -1;

                switch (select)
                {
                    case 2:
                        foreach (Order order in requests.GetShortOrders().Result)
                        {
                            Console.WriteLine($"Заказ: {order.Id}, Дата: {order.DateOfActual:yyyy-MM-dd} Статус: {order.State.ToString()}" +
                                $"\nИмя мертвого: {order.CorposeId.Name}, Фамилия: {order.CorposeId.SurName}," +
                                $"\nНомер зала: {order.HallId?.Id}, Урна: {order.RitualUrnId?.Name}, Гроб: {order.CoffinId?.Name}");
                        }
                        break;
                    case 3:
                        id = GetId();
                        CompliteOrder(id);
                        break;
                    case 4:
                        id = GetId();
                        ApproveOrder(id);
                        break;
                }
                Console.WriteLine();
            } while (select != 1);
        }

        //выполнить заказ
        private void CompliteOrder(int id)
        {
            StateOrder state = requests.GetStateOrder(id).Result;

            if (state == StateOrder.Approved)
            {
                requests.ChangeState(id, StateOrder.Closed).Wait();
                Console.WriteLine("Статус изменен");
            }
            else
            {
                Console.WriteLine("Заказ нельзя выполнить");
            }
        }

        //отменить заказ
        private void CancelOrder(int id)
        {
            StateOrder state = requests.GetStateOrder(id).Result;

            if (state == StateOrder.Decorated || state == StateOrder.Approved)
            {
                requests.ChangeState(id, StateOrder.Cancelled).Wait();
                Console.WriteLine("Статус изменен");
            }
            else
            {
                Console.WriteLine("Заказ нельзя отменить");
            }
        }

        //одобрить заказ
        private void ApproveOrder(int id)
        {
            StateOrder state = requests.GetStateOrder(id).Result;

            if (state == StateOrder.Decorated)
            {
                requests.ChangeState(id, StateOrder.Approved).Wait();
                Console.WriteLine("Статус изменен");
            }
            else
            {
                Console.WriteLine("Заказ нельзя одобрить");
            }
        }

        #endregion

        private string CheckString()
        {
            string res = Console.ReadLine();
            while(string.IsNullOrEmpty(res))
            {
                Console.WriteLine("Некорректный ввод. Повторите попытку: ");
                res = Console.ReadLine();
            }

            return res;
        }

        private int CheckInput(int max)
        {
            int change;
            Console.WriteLine("Сделайте выбор: ");
            while (!int.TryParse(Console.ReadLine(), out change) || (change < 1 || change > max))
            {
                Console.WriteLine("Некорректный ввод. Сделайте выбор: ");
            }

            return change;
        }

        private int GetId()
        {
            int change;
            Console.WriteLine("Введите id объекта: ");
            while (!int.TryParse(Console.ReadLine(), out change))
            {
                Console.WriteLine("Некорректный ввод. Сделайте выбор: ");
            }

            return change;
        }
    }
}
