using CommunityToolkit.Mvvm.ComponentModel;
using Crematorium.Domain.Entities;
using Crematorium.UI.Converters.PropertyConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crematorium.UI.ViewModels
{
    public partial class OrderInformationVM : ObservableValidator
    {
        [ObservableProperty]
        private Order selectedOrder;
        public void InitializeOrder(Order order)
        {
            SelectedOrder = order;
            SelectedCustomer = order.Customer;
            SelectedCorpose = order.CorposeId;
            SelectedUrn = order.RitualUrnId;
            SelectedHall = order.HallId;
            DateOfStart = order.DateOfStart;
            SelectedState = order.State;
        }

        [ObservableProperty]
        private User selectedCustomer;

        [ObservableProperty]
        private Corpose selectedCorpose;

        [ObservableProperty]
        private RitualUrn selectedUrn;

        [ObservableProperty]
        private Date dateOfStart;

        [ObservableProperty]
        private Hall selectedHall;

        [ObservableProperty]
        private StateOrder selectedState;
    }
}
