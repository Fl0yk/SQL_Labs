using CommunityToolkit.Mvvm.ComponentModel;
using Crematorium.Domain.Entities;
using Crematorium.UI.Fabrics;
using Crematorium.UI.Pages;
using Microsoft.Extensions.Hosting;

namespace Crematorium.UI.ViewModels
{
    public partial class MainWindowVM : ObservableValidator
    {
        public MainWindowVM(IHostEnvironment env)
        {
            string a = env.ContentRootPath;
            CurUser = ServicesFabric.CurrentUser;
        }

        [ObservableProperty]
        private User? curUser;

        [ObservableProperty]
        private string loginBtn = "Login";

        public void LoginUser()
        {
            if (ServicesFabric.CurrentUser is null)
            {
                var loginPage = (LoginPage)ServicesFabric.GetPage(typeof(LoginPage));
                loginPage.ShowDialog();
            }
            else
            {
                ServicesFabric.CurrentUser = null;
            }
            //var loginPage = (LoginPage)ServicesFabric.GetPage(typeof(LoginPage));
            //loginPage.ShowDialog();
            CurUser = ServicesFabric.CurrentUser;
            if (ServicesFabric.CurrentUser is not null)
            {
                LoginBtn = "Logout";
            }
            else
            {
                LoginBtn = "Login";
            }
        }
    }
}
