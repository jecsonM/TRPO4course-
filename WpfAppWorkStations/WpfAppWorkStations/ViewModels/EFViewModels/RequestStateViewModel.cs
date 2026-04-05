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
    public partial class RequestStateViewModel : BaseViewModel, IEFViewModel
    {
        [ObservableProperty]
        private bool isChanged;

        [ObservableProperty]
        private Requeststate requeststate;

        public RequestStateViewModel(Requeststate requeststate)
        {
            this.requeststate = requeststate;
        }

        public override string ToString()
        {
            return requeststate.RequestStateName;
        }
    }
}
