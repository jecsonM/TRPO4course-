using CommunityToolkit.Mvvm.ComponentModel;
using WpfAppWorkStations.EntityFramework;
using WpfAppWorkStations.Interfaces.ViewModels;

namespace WpfAppWorkStations.ViewModels.EFViewModels
{
    public partial class RoleViewModel : BaseViewModel, IEFViewModel
    {
        [ObservableProperty]
        private bool isChanged;

        [ObservableProperty]
        private Role role;

        public RoleViewModel(Role role)
        {
            this.role = role;
            isChanged = false;
        }

        public override string ToString()
        {
            return Role.RoleName;
        }
    }
}