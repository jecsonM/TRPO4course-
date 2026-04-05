using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfAppWorkStations.EntityFramework;
using WpfAppWorkStations.Interfaces.ViewModels;

namespace WpfAppWorkStations.ViewModels.EFViewModels
{
    public partial class ClientViewModel : BaseViewModel, IEFViewModel
    {
        [ObservableProperty]
        private bool isChanged;

        [ObservableProperty]
        private Client client;

        public ClientViewModel(Client client)
        {
            this.client = client;
        }

        public override string ToString()
        {
            return client?.CompanyName ?? "";
        }
    }
}
