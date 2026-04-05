using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WpfAppWorkStations.Interfaces.Services;
using WpfAppWorkStations.Interfaces.ViewModels;
using WpfAppWorkStations.ViewModels.Pages;

namespace WpfAppWorkStations.Stores
{
    public class NavigationStore
    {
        private IPageViewModel _currentViewModel;
        private IAuthorizationService _authorizationService;

        public NavigationStore(IAuthorizationService authorizationService)
        {
            
            _authorizationService = authorizationService;
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



        public void NavigateTo(Type pageViewModelType)
        {
            if (AllPageViewModels.ContainsKey(pageViewModelType))
            {
                IPageViewModel pageViewModel = AllPageViewModels[pageViewModelType];
                if (_authorizationService.CanCurrentUserAcces(pageViewModel))
                    CurrentViewModel = pageViewModel;
            }
        }
        

        public void NavigateTo<TPageViewModel>() where TPageViewModel : IPageViewModel
            => NavigateTo(typeof(TPageViewModel));
        
        
        public Dictionary<Type, IPageViewModel> AllPageViewModels { get; set; }
        public IPageViewModel CurrentViewModel1 { get => _currentViewModel; set => _currentViewModel = value; }

        public event Action CurrentViewModelChanged;

    }
    


}
