using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using WpfAppWorkStations.Interfaces.ViewModels;

namespace WpfAppWorkStations.Interfaces.Services
{
    public interface IAuthorizationService
    {
        ClaimsPrincipal CurrentUser { get; }
        bool IsAuthenticated { get; }
        event Action<ClaimsPrincipal> UserChanged;

        Task<bool> LoginAsync(string login, string password);
        void Logout();

        
        public bool CanCurrentUserAcces<TPageViewModel>(TPageViewModel pageViewModel) where TPageViewModel : IPageViewModel;
        
        

    }
}
