using CommunityToolkit.Mvvm.ComponentModel;
using WpfAppWorkStations.EntityFramework;
using WpfAppWorkStations.Interfaces.ViewModels;

namespace WpfAppWorkStations.ViewModels.EFViewModels
{
    public partial class MachineViewModel : BaseViewModel, IEFViewModel
    {
        [ObservableProperty]
        private bool isChanged;

        [ObservableProperty]
        private Machine machine;

        public MachineViewModel(Machine machine)
        {
            this.machine = machine;
            isChanged = false;
        }

        public void SetClient(Client client)
        {
            if (client != null)
            {
                Machine.ClientId = client.ClientId;
                Machine.Client = client;
            }
            else
            {
                Machine.ClientId = 0;
                Machine.Client = null;
            }
            IsChanged = true;
            OnPropertyChanged(nameof(Machine));
        }

        public override string ToString()
        {
            return $"{Machine.Model} - {Machine.SerialNumber}";
        }
    }
}