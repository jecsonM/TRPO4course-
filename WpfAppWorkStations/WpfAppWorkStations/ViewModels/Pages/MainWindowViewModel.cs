using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfAppWorkStations.Enums;
using WpfAppWorkStations.Interfaces.Services;
using WpfAppWorkStations.Interfaces.ViewModels;
using WpfAppWorkStations.Stores;

namespace WpfAppWorkStations.ViewModels.Pages
{
    public partial class MainWindowViewModel : BaseViewModel
    {
        public IPageViewModel CurrentViewModel => _navigationStore.CurrentViewModel;
        private NavigationStore _navigationStore;
        private IAuthorizationService _authorizationService;

        public MainWindowViewModel(NavigationStore navigationStore, IAuthorizationService authorizationService)
        {
            _navigationStore = navigationStore;
            _navigationStore.CurrentViewModelChanged += () => OnPropertyChanged(nameof(CurrentViewModel));

            MainMenu =
                new ButtonsForPageViewModels(
                    navigationStore,
                    navigationStore.AllPageViewModels[typeof(ActivitySelectionViewModel)]
                );
            navPanelVisibility = Visibility.Hidden;
            _authorizationService = authorizationService;

            _authorizationService.UserChanged += (cp) => ChangeNavPanelVisibility();
            _authorizationService.UserChanged += (cp) => OnPropertyChanged(nameof(CurrentUserDisplay));
        }

        public ButtonsForPageViewModels MainMenu { get; }

        [ObservableProperty]
        private Visibility navPanelVisibility;

        public string CurrentUserDisplay => $"{_authorizationService.CurrentUser?.Identity.Name} {_authorizationService.CurrentUser?.FindFirst(ClaimTypes.Role)?.Value}";
        public void ChangeNavPanelVisibility()
        {
            if(_authorizationService.CurrentUser?.Identity.IsAuthenticated == true)
                NavPanelVisibility = Visibility.Visible;
            else
                NavPanelVisibility = Visibility.Hidden;
        }

    }
}
