using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using WpfAppWorkStations.EntityFramework;
using WpfAppWorkStations.Interfaces.ViewModels;

namespace WpfAppWorkStations.ViewModels.EFViewModels
{
    public partial class ServiceViewModel : BaseViewModel, IEFViewModel
    {
        [ObservableProperty]
        private bool isChanged;

        [ObservableProperty]
        private Machineservice service;

        private ObservableCollection<RelevantCostViewModel> _relevantCosts;
        public ObservableCollection<RelevantCostViewModel> RelevantCosts
        {
            get => _relevantCosts;
            set
            {
                _relevantCosts = value;
                OnPropertyChanged(nameof(RelevantCosts));
                OnPropertyChanged(nameof(CurrentPrice));
            }
        }

        public ServiceViewModel(Machineservice service)
        {
            this.service = service;
            isChanged = false;
            _relevantCosts = new ObservableCollection<RelevantCostViewModel>();
        }

        public decimal CurrentPrice
        {
            get
            {
                var latestCost = RelevantCosts?
                    .OrderByDescending(rc => rc.RelevantCost.SetDate)
                    .FirstOrDefault();
                return latestCost?.RelevantCost.RelevantCost1 ?? 0;
            }
        }

        public string MachineServiceName
        {
            get => service.MachineServiceName;
            set
            {
                service.MachineServiceName = value;
                IsChanged = true;
                OnPropertyChanged(nameof(Service));
                OnPropertyChanged(nameof(MachineServiceName));
            }
        }

        public override string ToString()
        {
            return $"{service.MachineServiceName} - {CurrentPrice:F2} ₽";
        }

        public void LoadRelevantCosts(List<Relevantcost> costs)
        {
            RelevantCosts.Clear();
            foreach (var cost in costs.OrderByDescending(c => c.SetDate))
            {
                RelevantCosts.Add(new RelevantCostViewModel(cost));
            }
            OnPropertyChanged(nameof(CurrentPrice));
        }
    }
}