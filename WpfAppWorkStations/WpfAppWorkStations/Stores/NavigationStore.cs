using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WpfAppWorkStations.Interfaces.ViewModels;

namespace WpfAppWorkStations.Stores
{
    public class NavigationStore
    {
        private IPageViewModel _currentViewModel;

        public NavigationStore(Dictionary<Type, IPageViewModel> allPageViewModels)
        {
            AllPageViewModels = allPageViewModels;
        }

        public IPageViewModel CurrentViewModel 
        { 
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                CurrentViewModelChanged?.Invoke();
            }
        }

        
        public Dictionary<Type, IPageViewModel> AllPageViewModels { get; private set; }
        public event Action CurrentViewModelChanged;

    }
    


}
