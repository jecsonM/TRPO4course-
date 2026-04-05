// OrderViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using WpfAppWorkStations.EntityFramework;
using WpfAppWorkStations.Interfaces.ViewModels;

namespace WpfAppWorkStations.ViewModels.EFViewModels
{
    public partial class OrderViewModel : BaseViewModel, IEFViewModel
    {
        [ObservableProperty]
        private bool isChanged;

        [ObservableProperty]
        private Order order;

        public OrderViewModel(Order order)
        {
            this.order = order;
            isChanged = false;
        }
    }
}


