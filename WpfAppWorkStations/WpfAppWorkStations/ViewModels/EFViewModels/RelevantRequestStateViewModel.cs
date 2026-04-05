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
    public partial class RelevantRequestStateViewModel : BaseViewModel, IEFViewModel
    {
        [ObservableProperty]
        private bool isChanged;

        [ObservableProperty]
        private Relevantrequeststate relevantRequestState;

        public RelevantRequestStateViewModel(Relevantrequeststate relevantRequestState)
        {
            this.relevantRequestState = relevantRequestState;
        }
    }
}
