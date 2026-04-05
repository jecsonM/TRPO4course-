using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfAppWorkStations.Interfaces.ViewModels;

namespace WpfAppWorkStations.ViewModels.EFViewModels
{
    public partial class RequestStateViewModel : BaseViewModel, IEFViewModel
    {
        [ObservableProperty]
        private bool isChanged;


    }
}
