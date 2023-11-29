using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Crematorium.Application.Abstractions;
using Crematorium.Application.Services;
using Crematorium.Domain.Abstractions;
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
    public partial class RitualUrnsVM : ObservableValidator
    {
        private IHelpersService<RitualUrn> _urnService;

        public ObservableCollection<RitualUrn> RitualUrns { get; set; }
        public RitualUrnsVM(IHelpersService<RitualUrn> baseService)
        {
            _urnService = baseService;
            RitualUrns = new ObservableCollection<RitualUrn>(_urnService.GetAllAsync().Result);
        }

        [ObservableProperty]
        [MaxLength(20)]
        private string inputFindName;

        [RelayCommand]
        public void FindUrns()
        {
            RitualUrns.Clear();
            if (string.IsNullOrEmpty(InputFindName) || string.IsNullOrWhiteSpace(InputFindName))
            {
                UpdateUrnsCollection();
                return;
            }

            foreach (RitualUrn urn in _urnService.FindByName(InputFindName).Result)
            {
                RitualUrns.Add(urn);
            }
        }

        [RelayCommand]
        public void AddUrn()
        {
            var userChange = (ChangeUrnPage)ServicesFabric.GetPage(typeof(ChangeUrnPage));
            userChange.InitializeUrn(-1);
            userChange.OpBtnName.Text = "Registration";
            userChange.ShowDialog();
            UpdateUrnsCollection();
        }

        [ObservableProperty]
        private RitualUrn selectedUrn;

        [RelayCommand]
        public void UpdateUrn()
        {
            if (SelectedUrn is null)
                return;

            var userChange = (ChangeUrnPage)ServicesFabric.GetPage(typeof(ChangeUrnPage));
            userChange.InitializeUrn(SelectedUrn.Id);
            userChange.OpBtnName.Text = "Update";
            userChange.ShowDialog();
            UpdateUrnsCollection();
        }

        [RelayCommand]
        public void DeleteUrn()
        {
            if (SelectedUrn is null)
                return;

            _urnService.DeleteAsync(SelectedUrn.Id);
            UpdateUrnsCollection();
        }

        private void UpdateUrnsCollection()
        {
            RitualUrns.Clear();
            foreach (RitualUrn user in _urnService.GetAllAsync().Result)
            {
                RitualUrns.Add(user);
            }
        }
    }
}
