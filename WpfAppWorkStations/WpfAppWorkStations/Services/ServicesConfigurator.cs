using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
                .AddSingleton<MainWindowViewModel>();

            //ViewModels


            //Views
            services
                .AddSingleton<MainWindow>();


            return services.BuildServiceProvider();
        }

        public static ServiceCollection ConfigureAllPagesForNavigationStore(
            this ServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
            serviceCollection
                .AddSingleton<NavigationStore>(
                sp =>
                new NavigationStore(
                    Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .Where(t => t.IsInterface && typeof(IPageViewModel).IsAssignableFrom(t) && t != typeof(IPageViewModel))
                    .Select(
                        t => new KeyValuePair<Type, IPageViewModel>(t, serviceProvider.GetRequiredService(t) as IPageViewModel)
                    )
                    .ToDictionary()
                )
                )
                ;
            return serviceCollection;
        }
    }


}
