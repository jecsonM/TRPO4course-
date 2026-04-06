using CommunityToolkit.Mvvm.ComponentModel;
using WpfAppWorkStations.EntityFramework;
using WpfAppWorkStations.Interfaces.ViewModels;

namespace WpfAppWorkStations.ViewModels.EFViewModels
{
    public partial class StaffViewModel : BaseViewModel, IEFViewModel
    {
        [ObservableProperty]
        private bool isChanged;

        [ObservableProperty]
        private Staff staff;

        public StaffViewModel(Staff staff)
        {
            this.staff = staff;
            isChanged = false;
        }

        public string RoleName => Staff.Role?.RoleName ?? "Не назначена";
        public string DisplayText => $"{Staff.Login} - {RoleName}";

        public override string ToString()
        {
            return DisplayText;
        }
    }
}

