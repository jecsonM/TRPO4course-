using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using WpfAppWorkStations.EntityFramework;
using WpfAppWorkStations.Enums;
using WpfAppWorkStations.Interfaces.Services;
using WpfAppWorkStations.Interfaces.ViewModels;
using WpfAppWorkStations.Services;
using WpfAppWorkStations.Stores;

namespace WpfAppWorkStations.ViewModels.Pages
{
    public partial class AuthorizationViewModel : IPageViewModel
    {
        private IAuthorizationService _authorizationService;
        private IDBWorkStationsService _dBWorkStationsService;
        private NavigationStore _navigationStore;

        public AppRoles[] AllowedRoles { get; }

        public AuthorizationViewModel(
            IAuthorizationService authorizationService, 
            IDBWorkStationsService dBWorkStationsService,
            NavigationStore navigationStore)
        {
            AllowedRoles = new AppRoles[] { AppRoles.None };

            _authorizationService = authorizationService;
            _dBWorkStationsService = dBWorkStationsService;
            _navigationStore = navigationStore;
        }

        public string Title => "Авторизация";


        public async Task<bool> Authorize(string login, string password)
        {

            //byte[] hashPass = PasswordService.HashPassword(password);
            //Staff newDIr = new Staff() { Login = login, PasswordHash = hashPass, RoleId = 27 };
            //_dBWorkStationsService.AddNewUser(newDIr);

            bool isSucces = await _authorizationService.LoginAsync(login, password);

            if (isSucces)
            {
                _navigationStore.NavigateTo<ActivitySelectionViewModel>();
            }
            return isSucces;
        }
    }
}
