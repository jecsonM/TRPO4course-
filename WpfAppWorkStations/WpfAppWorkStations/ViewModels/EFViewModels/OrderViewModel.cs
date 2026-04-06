using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
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

        public ObservableCollection<MachineViewModel> Machines
        {
            get
            {
                return
                    new ObservableCollection
    <MachineViewModel>(
                    order.Machines.Select(m => new MachineViewModel(m))
                    .ToList()
                );
            }

        }

    }
}