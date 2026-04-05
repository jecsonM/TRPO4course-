using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfAppWorkStations.Enums;
using WpfAppWorkStations.Interfaces.ViewModels;
using WpfAppWorkStations.Stores;

namespace WpfAppWorkStations.ViewModels.Pages
{
    public partial class MainWindowViewModel : BaseViewModel
    {
        public IPageViewModel CurrentViewModel => _navigationStore.CurrentViewModel;
        private NavigationStore _navigationStore;

        public MainWindowViewModel(NavigationStore navigationStore)
        {
            _navigationStore = navigationStore;
            _navigationStore.CurrentViewModelChanged += () => OnPropertyChanged(nameof(CurrentViewModel));
        }

        
        

    }
}
