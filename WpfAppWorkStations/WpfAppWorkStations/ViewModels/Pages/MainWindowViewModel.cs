using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfAppWorkStations.Interfaces.ViewModels;

namespace WpfAppWorkStations.ViewModels.Pages
{
    public partial class MainWindowViewModel : BaseViewModel, IPageViewModel
    {
        [ObservableProperty]
        private IPageViewModel currentViewModel;

        public MainWindowViewModel()
        {
            this.currentViewModel = new AuthorizationViewModel();
        }

        public string Title { get => "Главное окно"; }
    }
}
