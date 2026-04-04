using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfAppWorkStations.EntityFramework;

namespace WpfAppWorkStations.ViewModels
{
    public class AppUserViewModel
    {
        public readonly Staff User;
        public readonly Role Role;

        public AppUserViewModel(Staff user, Role role)
        {
            User = user;
            Role = role;
        }
    }
}
