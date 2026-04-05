// OrderViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using WpfAppWorkStations.EntityFramework;
using WpfAppWorkStations.Interfaces.ViewModels;

// OrderStateViewModel.cs
namespace WpfAppWorkStations.ViewModels.EFViewModels
{
    public partial class OrderStateViewModel : BaseViewModel, IEFViewModel
    {
        [ObservableProperty]
        private bool isChanged;

        [ObservableProperty]
        private Orderstate orderstate;

        public OrderStateViewModel(Orderstate orderstate)
        {
            this.orderstate = orderstate;
            isChanged = false;
        }

        public override string ToString()
        {
            return orderstate?.OrderStateName ?? "";
        }
    }
}
