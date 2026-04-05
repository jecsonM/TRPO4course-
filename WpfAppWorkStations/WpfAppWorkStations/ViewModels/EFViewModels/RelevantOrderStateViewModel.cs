// OrderViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using WpfAppWorkStations.EntityFramework;
using WpfAppWorkStations.Interfaces.ViewModels;



// RelevantOrderStateViewModel.cs
namespace WpfAppWorkStations.ViewModels.EFViewModels
{
    public partial class RelevantOrderStateViewModel : BaseViewModel, IEFViewModel
    {
        [ObservableProperty]
        private bool isChanged;

        [ObservableProperty]
        private Relevantorderstate relevantOrderState;

        public RelevantOrderStateViewModel(Relevantorderstate relevantOrderState)
        {
            this.relevantOrderState = relevantOrderState;
            isChanged = false;
        }
    }
}