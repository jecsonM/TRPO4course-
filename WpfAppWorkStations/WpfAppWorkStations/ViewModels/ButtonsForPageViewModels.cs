using System.Windows.Input;
using WpfAppWorkStations.Interfaces.ViewModels;
using WpfAppWorkStations.Stores;

namespace WpfAppWorkStations.ViewModels
{
    public class ButtonsForPageViewModels : BaseViewModel, ICommand
    {
        private NavigationStore _navigationStore;
        public IPageViewModel PageViewModel { get; }
        public ButtonsForPageViewModels(
            NavigationStore navigationStore,
            IPageViewModel pageViewModel) 
        {
            _navigationStore = navigationStore;
            PageViewModel = pageViewModel;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter)
        {
            _navigationStore.NavigateTo(PageViewModel.GetType());
        }
    }
}
