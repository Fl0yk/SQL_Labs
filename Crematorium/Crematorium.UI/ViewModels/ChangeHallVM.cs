﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Crematorium.Application.Abstractions;
using Crematorium.Domain.Entities;
using Crematorium.UI.Fabrics;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Crematorium.UI.ViewModels
{
    public partial class ChangeHallVM : ObservableValidator
    {
        private IHelpersService<Hall> _hallService;
        private bool _isNewHall;

        public ChangeHallVM(IHelpersService<Hall> hallService)
        {
            _hallService = hallService;
            FreeDates = new ObservableCollection<Date>();
        }

        [ObservableProperty]
        private Hall selectedHall;

        public void SetHall(int id)
        {
            SelectedHall = _hallService.GetByIdAsync(id).Result;

            if(SelectedHall is null)
            {
                SelectedHall = new();
                _isNewHall = true;
            }
            else
            {
                _isNewHall = false;
            }
            Name = SelectedHall.Name;
            Capacity = SelectedHall.Capacity;
            Price = SelectedHall.Price;
            NewDate = DateTime.Now.Date.ToString("dd.MM.yyyy");
            FreeDates.Clear();
            if(SelectedHall.FreeDates is not null)
            {
                foreach(var date in SelectedHall.FreeDates)
                {
                    FreeDates.Add(date);
                }
            }
        }

        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private int capacity;

        [ObservableProperty]
        private decimal price;

        [ObservableProperty]
        private string newDate;

        public ObservableCollection<Date> FreeDates { get; set; }

        [RelayCommand]
        public void AddNewDate()
        {
            if (NewDate is null)
                return;
            
            //if (Regex.IsMatch(NewDate, @"(\d\d|\d)\.(\d\d|\d)\.\d\d\d\d"))
            if(DateTime.TryParse(NewDate, out DateTime dateOnly) && dateOnly >= DateTime.Now.Date)
            {
                Date d = new Date() { Data = dateOnly.ToString("dd.MM.yyyy") };
                FreeDates.Add(d);
                //SelectedHall.FreeDates.Add(d);
            }
            else
            {
                var er = ServicesFabric.GetErrorPage("Некорректная дата");
                er.ShowDialog();
                NewDate = DateTime.Now.Date.ToString("dd.MM.yyyy");
            }
        }

        [RelayCommand]
        public async void AddHall()
        {
            if (SelectedHall is null)
                throw new ArgumentNullException("Hall not initialized");

            if (Name == string.Empty || Name is null ||
                Price == 0  ||
                Capacity == 0)
            {
                var er = ServicesFabric.GetErrorPage("Что-то не заполнили");
                er.ShowDialog();
            }

            SelectedHall.Name = this.Name;
            SelectedHall.Price = this.Price;
            SelectedHall.Capacity = this.Capacity;
            SelectedHall.FreeDates = this.FreeDates.ToList();

            if (_isNewHall)
            {
                //User.Id = 0;
                await _hallService.AddAsync(SelectedHall);
            }
            else
            {
                await _hallService.UpdateAsync(SelectedHall);
            }
        }
    }
}
