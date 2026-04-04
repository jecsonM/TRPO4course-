using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using WpfAppWorkStations.EntityFramework;
using WpfAppWorkStations.Enums;
using WpfAppWorkStations.Interfaces.Services;
using WpfAppWorkStations.Interfaces.ViewModels;
using WpfAppWorkStations.Stores;
using static WpfAppWorkStations.Services.AuthorizationService;

namespace WpfAppWorkStations.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        
        private ClaimsPrincipal _currentUser;
        private IPasswordService _passwordService;
        private NavigationStore _navigationStore;
        private IDBWorkStationsService _dbWorkStationsService;

        public AuthorizationService(
            IPasswordService passwordService, 
            NavigationStore navigationStore,
            IDBWorkStationsService dbWorkStationsService)
        {
            _passwordService = passwordService;
            _navigationStore = navigationStore;
            _dbWorkStationsService = dbWorkStationsService;
            _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
        }

        public ClaimsPrincipal CurrentUser => _currentUser;
        public bool IsAuthenticated => _currentUser?.Identity?.IsAuthenticated ?? false;

        public event Action<ClaimsPrincipal> UserChanged;

        
        public bool CanCurrentUserAcces<TPageViewModel>(TPageViewModel pageViewModel) where TPageViewModel : IPageViewModel
        {
            throw new NotImplementedException();
        }

        public async Task<bool> LoginAsync(string login, string password)
        {
            
            Staff user = _dbWorkStationsService.GetStaffByLogin(login);
            if (user == null || !_passwordService.ValidatePassword(password, user.PasswordHash ?? new byte[4]))
                return false;



            Role role = _dbWorkStationsService.GetRoleByStaffID(user.StaffId);
            if (role == null)
                role = new Role() { RoleName = "None"};
            AppRoles appRole = AppRoles.None;
            {
                object appRoleObj;
                if (Enum.TryParse(typeof(AppRoles), role.RoleName, out appRoleObj))
                    appRole = (AppRoles)appRoleObj;
            }


            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.StaffId.ToString()),
                new Claim(ClaimTypes.Name, user.Login),
                new Claim(ClaimTypes.Role, role.RoleName),
                new Claim("AppRoleId", ((int)appRole).ToString()),
                new Claim(ClaimTypes.AuthenticationMethod, "Password")
            };



            

            var identity = new ClaimsIdentity(claims, "ApplicationCookie");

            
            var principal = new ClaimsPrincipal(identity);

            
            _currentUser = principal;
            UserChanged?.Invoke(_currentUser);

            return true;
        }

        public void Logout()
        {
            _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
            UserChanged?.Invoke(_currentUser);
        }

        
        
    }
}
