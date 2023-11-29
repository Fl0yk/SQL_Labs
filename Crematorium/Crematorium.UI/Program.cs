using Crematorium.Application.Abstractions;
using Crematorium.Application.Services;
using Crematorium.Domain.Abstractions;
using Crematorium.Domain.Entities;
using Crematorium.Persistense.Data;
using Crematorium.Persistense.Repository;
using Crematorium.Persistense.Repository.Postgre;
using Crematorium.UI.Fabrics;
using Crematorium.UI.Pages;
using Crematorium.UI.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Reflection;

namespace Crematorium.UI
{
    public class Program
    {
        [STAThread]
        public static void Main()
        {
            string settingsStream = "Crematorium.UI.appsettings.json";

            var a = Assembly.GetExecutingAssembly();
            using var stream = a.GetManifestResourceStream(settingsStream);
            var configuration = new ConfigurationBuilder().AddJsonStream(stream).Build();

            // создаем хост приложения
            var host = Host.CreateDefaultBuilder()
                // внедряем сервисы
                .ConfigureServices(services => SetupServices(services, configuration))
                .Build();
            
            // получаем сервис - объект класса App
            var app = host.Services.GetService<App>();

            //Настраиваем фабрику сервисов
            ServicesFabric.Services = host.Services;
            ServicesFabric.CurrentUser = null;
            
            // запускаем приложения
            app?.Run();
        }

        private static void SetupServices(IServiceCollection services, IConfiguration config)
        {
            var connStr = config.GetConnectionString("SqliteConnection");

            //Data base
            services.AddDbContext<CrematoriumDbContext>(options =>
            {
                options.UseSqlite(connStr);
            });

            services.AddSingleton(config);

            //Repositories
            services.AddScoped<CeremonyRepository>();
            services.AddScoped<CorposeRepository>();
            services.AddScoped<HallRepository>();
            services.AddScoped<OrdersRepository>();
            services.AddScoped<RitualUrnRepository>();
            services.AddScoped<UsersRepository>();
            services.AddScoped<CoffinRepository>();

            //Services
            //services.AddSingleton<IUnitOfWork, FakeUnitOfWork>();
            services.AddSingleton<IUnitOfWork, EfUnitOfWork>();
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IHelpersService<RitualUrn>, RitualUrnService>();
            services.AddSingleton<IHelpersService<Corpose>,  CorposeService>();
            services.AddSingleton<IHelpersService<Hall>, HallService>();
            services.AddSingleton<IOrderService, OrderService>();

            //Pages
            services.AddTransient<App>();
            services.AddTransient<MainWindow>();
            services.AddTransient<HomePage>();
            services.AddTransient<UsersPage>();
            services.AddTransient<RitualUrnServicePage>();
            services.AddTransient<CorposesServicePage>();
            services.AddTransient<HallServicePage>();
            services.AddTransient<AllOrdersPage>();
            services.AddTransient<UserOrdersPage>();
            services.AddTransient<UserAccountPage>();

            //Help pages
            services.AddSingleton<LoginPage>();
            services.AddSingleton<ChangeUserPage>();
            services.AddSingleton<ChangeUrnPage>();
            services.AddSingleton<ChangeCorposePage>();
            services.AddSingleton<ChangeHallPage>();
            services.AddSingleton<OrderInformationPage>();
            services.AddSingleton<ErrorPage>();

            //ViewModels
            services.AddTransient<MainWindowVM>();
            services.AddTransient<LoginVM>();
            services.AddTransient<HomeVM>();
            services.AddTransient<UsersVM>();
            services.AddTransient<UserChangeVM>();
            services.AddTransient<RitualUrnsVM>();
            services.AddTransient<ChangeUrnVM>();
            services.AddTransient<CorposesVM>();
            services.AddTransient<ChangeCorposeVM>();
            services.AddTransient<HallServiceVM>();
            services.AddTransient<ChangeHallVM>();
            services.AddTransient<AllOrdersVM>();
            services.AddTransient<UserOrdersVM>();
            services.AddTransient<OrderInformationVM>();
            services.AddTransient<UserAccountVM>();
        }
    }
}
