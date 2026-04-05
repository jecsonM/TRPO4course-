using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfAppWorkStations.Enums;
using WpfAppWorkStations.Interfaces.Services;
using WpfAppWorkStations.Interfaces.ViewModels;
using WpfAppWorkStations.Stores;

namespace WpfAppWorkStations.ViewModels.Pages
{
    public partial class ActivitySelectionViewModel : IPageViewModel
    {
        private IAuthorizationService _authorizationService;
        private NavigationStore _navigationStore;

        public ActivitySelectionViewModel(IAuthorizationService authorizationService, NavigationStore navigationStore)
        {
            _authorizationService = authorizationService;
            _navigationStore = navigationStore;

            _buttonsForPageViews = new ObservableCollection<ButtonsForPageViewModels>();

            authorizationService.UserChanged += (cp) => RefreshButtonsList();

            AllowedRoles = new AppRoles[] { AppRoles.Authorized };
        }
        private ObservableCollection<ButtonsForPageViewModels> _buttonsForPageViews;
        public ObservableCollection<ButtonsForPageViewModels> 
            ButtonsForPageViews { get => _buttonsForPageViews; }

        public AppRoles[] AllowedRoles { get; }

        public string Title => "Главное меню";

        public void RefreshButtonsList()
        {
            _buttonsForPageViews = new ObservableCollection<ButtonsForPageViewModels>();
            if (_navigationStore
                .AllPageViewModels != null)
            {

                foreach (var page in
                    _navigationStore
                    .AllPageViewModels
                    .Values
                    .Where(p => _authorizationService.CanCurrentUserAcces(p))
                    .Where(p => p.GetType() != typeof(ActivitySelectionViewModel)))
                    
                {
                    _buttonsForPageViews
                        .Add(new ButtonsForPageViewModels(_navigationStore, page));
                }
            }
        }

    }
}
