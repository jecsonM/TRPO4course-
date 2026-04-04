using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfAppWorkStations.Enums;

namespace WpfAppWorkStations.ViewModels
{
    public class ButtonWithAccessViewModel : BaseViewModel
    {
        public ButtonWithAccessViewModel(ICommand command, AppRoles[] allowedRoles)
        {
            Command = command;
            AllowedRoles = allowedRoles;
        }

        public ICommand Command { get; }
        public AppRoles[] AllowedRoles { get; }
    }
}
