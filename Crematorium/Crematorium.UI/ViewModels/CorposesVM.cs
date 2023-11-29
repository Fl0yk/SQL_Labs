using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Crematorium.Application.Abstractions;
using Crematorium.Domain.Entities;
using Crematorium.UI.Fabrics;
using Crematorium.UI.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Crematorium.UI.ViewModels
{
    public partial class CorposesVM : ObservableValidator
    {
        private IHelpersService<Corpose> _corposeService;

        public ObservableCollection<Corpose> Corposes { get; set; }
        public CorposesVM(IHelpersService<Corpose> baseService)
        {
            _corposeService = baseService;
            Corposes = new ObservableCollection<Corpose>(_corposeService.GetAllAsync().Result);
        }

        [ObservableProperty]
        [MaxLength(20)]
        private string inputFindName;

        [RelayCommand]
        public void FindCorposes()
        {
            Corposes.Clear();
            if (string.IsNullOrEmpty(InputFindName) || string.IsNullOrWhiteSpace(InputFindName))
            {
                UpdateUrnsCollection();
                return;
            }

            foreach (Corpose corpose in _corposeService.FindByName(InputFindName).Result)
            {
                Corposes.Add(corpose);
            }
        }

        [RelayCommand]
        public void AddCorpose()
        {
            var userChange = (ChangeCorposePage)ServicesFabric.GetPage(typeof(ChangeCorposePage));
            userChange.InitializeCorpose(-1);
            userChange.OpBtnName.Text = "Registration";
            userChange.ShowDialog();
            UpdateUrnsCollection();
        }

        [ObservableProperty]
        private Corpose selectedCorpose;

        [RelayCommand]
        public void UpdateCorpose()
        {
            if (SelectedCorpose is null)
                return;

            var userChange = (ChangeCorposePage)ServicesFabric.GetPage(typeof(ChangeCorposePage));
            userChange.InitializeCorpose(SelectedCorpose.Id);
            userChange.OpBtnName.Text = "Update";
            userChange.ShowDialog();
            UpdateUrnsCollection();
        }

        [RelayCommand]
        public void DeleteCorpose()
        {
            if (SelectedCorpose is null)
                return;

            _corposeService.DeleteAsync(SelectedCorpose.Id);
            UpdateUrnsCollection();
        }

        private void UpdateUrnsCollection()
        {
            Corposes.Clear();
            foreach (Corpose corpose in _corposeService.GetAllAsync().Result)
            {
                Corposes.Add(corpose);
            }
        }
    }
}
