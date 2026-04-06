using CommunityToolkit.Mvvm.ComponentModel;
using WpfAppWorkStations.EntityFramework;
using WpfAppWorkStations.Interfaces.ViewModels;

namespace WpfAppWorkStations.ViewModels.EFViewModels
{
    public partial class RelevantCostViewModel : BaseViewModel, IEFViewModel
    {
        [ObservableProperty]
        private bool isChanged;

        [ObservableProperty]
        private Relevantcost relevantCost;

        public RelevantCostViewModel(Relevantcost relevantCost)
        {
            this.relevantCost = relevantCost;
            isChanged = false;
        }

        public string FormattedPrice => $"{RelevantCost.RelevantCost1:F2} ₽";
        public string FormattedDate => RelevantCost.SetDate.ToString("dd.MM.yyyy HH:mm");
        public string CreatorName => RelevantCost.Creators?.Login ?? "Неизвестен";
    }

}