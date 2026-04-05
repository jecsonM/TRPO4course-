using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WpfAppWorkStations.Interfaces.Services;
using WpfAppWorkStations.Interfaces.ViewModels;
using WpfAppWorkStations.Stores;
using WpfAppWorkStations.ViewModels.Pages;

namespace WpfAppWorkStations.Services
{
    public static class ServicesConfigurator
    {
        public static ServiceProvider ConfigureServicesForWholeProject(this ServiceCollection services)
        {
            //Pages ViewModels
            services
                .AddSingleton<MainWindowViewModel>()
                .AddSingleton<AuthorizationViewModel>()
                .AddSingleton<ActivitySelectionViewModel>();

            //Services
            services
                .AddSingleton<IDBWorkStationsService, DBWorkStationsService>()
                .AddSingleton<IPasswordService, PasswordService>()
                .AddSingleton<IAuthorizationService, AuthorizationService>();

            //Stores
            services
                .AddSingleton<NavigationStore>();

            //Views
            services
                .AddSingleton<MainWindow>();

            ServiceProvider serviceProvider = services.BuildServiceProvider();

            serviceProvider
                .ConfigureAllPagesForNavigationStore();

            return serviceProvider;
        }

        public static void ConfigureAllPagesForNavigationStore(this ServiceProvider serviceProvider)
        {
            NavigationStore navigationStore = serviceProvider.GetRequiredService<NavigationStore>();
            navigationStore.AllPageViewModels = 
                Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .Where(t => typeof(IPageViewModel).IsAssignableFrom(t) && t != typeof(IPageViewModel))
                    .Select(
                        t => new KeyValuePair<Type, IPageViewModel>(t, serviceProvider.GetRequiredService(t) as IPageViewModel)
                    )
                    .ToDictionary();
            navigationStore.NavigateTo<AuthorizationViewModel>();
        }
    }


}
