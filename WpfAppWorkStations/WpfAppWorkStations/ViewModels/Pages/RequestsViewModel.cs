using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Security.Claims;
using WpfAppWorkStations.EntityFramework;
using WpfAppWorkStations.Enums;
using WpfAppWorkStations.Interfaces.Services;
using WpfAppWorkStations.Interfaces.ViewModels;
using WpfAppWorkStations.ViewModels;
using WpfAppWorkStations.ViewModels.EFViewModels;

namespace WpfAppWorkStations.ViewModels.Pages
{
    public partial class RequestsViewModel : BaseViewModel, IPageViewModel
    {
        private IDBWorkStationsService _DBworkStationsService;
        private IAuthorizationService _authorizationService;

        public RequestsViewModel(IDBWorkStationsService workStationsService, IAuthorizationService authorizationService = null)
        {
            _DBworkStationsService = workStationsService;
            AllowedRoles = new AppRoles[] { AppRoles.Мастер };
            _authorizationService = authorizationService;


            _requestsViewModels = new ObservableCollection<RequestViewModel>();
            _clientViewModels = new ObservableCollection<ClientViewModel>();
            _relevantRequestStates = new ObservableCollection<RelevantRequestStateViewModel>();

            if (_authorizationService != null)
            {
                _authorizationService.UserChanged += (cp) => FullRefresh();
            }


            FullRefresh();
        }

        private Staff currentCurrentStaffAccount;

        private void FullRefresh()
        {
            RefreshRequests();
            RefreshCurrentStaffAccount();
            RefreshClients();
        }

        private void RefreshCurrentStaffAccount()
        {
            if (_authorizationService.CurrentUser?.Identity != null)
            {
               currentCurrentStaffAccount =
                _DBworkStationsService.GetStaffByLogin(
                    _authorizationService.CurrentUser.Identity.Name
                );
            }
        }

        private ObservableCollection<RequestViewModel> _requestsViewModels;
        public ObservableCollection<RequestViewModel> RequestsViewModels => _requestsViewModels;


        private ObservableCollection<ClientViewModel> _clientViewModels;
        public ObservableCollection<ClientViewModel> ClientViewModels => _clientViewModels;

        private ClientViewModel _currentlySelectedClient;
        public ClientViewModel CurrentlySelectedClient
        {
            get => _currentlySelectedClient;
            set
            {
                if (currentlySelectedRequest != null &&
                    value.Client.ClientId != currentlySelectedRequest.Request.ClientId)
                    currentlySelectedRequest.SetClient(value.Client);
            }
        }


        private ObservableCollection<RelevantRequestStateViewModel> _relevantRequestStates;
        public ObservableCollection<RelevantRequestStateViewModel> RelevantRequestStates
            => _relevantRequestStates;


        

        private RequestViewModel currentlySelectedRequest;
        public RequestViewModel CurrentlySelectedRequest
        {
            get => currentlySelectedRequest;
            set
            {
                currentlySelectedRequest = value;
                if (value != null)
                {
                    _currentlySelectedClient =
                        ClientViewModels
                        .FirstOrDefault(cvm => cvm.Client.ClientId == value.Request.ClientId);
                    List<Relevantrequeststate> relevantrequeststates = 
                        _DBworkStationsService
                        .GetRelevantrequests(currentlySelectedRequest.Request);
                    RelevantRequestStates.Clear();
                    foreach (Relevantrequeststate rrs in relevantrequeststates)
                    {
                        RelevantRequestStates
                            .Add(new RelevantRequestStateViewModel(rrs));
                    }
                }


                OnPropertyChanged(nameof(CurrentlySelectedRequest));
                OnPropertyChanged(nameof(CurrentlySelectedClient));
            }
        }

        [RelayCommand]
        public void RefreshClients()
        {
            ClientViewModels.Clear();

            List<Client> clients = _DBworkStationsService.GetClients();

            foreach (Client client in clients)
            {
                ClientViewModels.Add(new ClientViewModel(client));
            }

        }


        [RelayCommand]
        public void RefreshRequests()
        {

            _requestsViewModels.Clear();

            List<Request> requests = new List<Request>();

            if (_authorizationService != null &&
                _authorizationService.CurrentUser.HasClaim(ClaimTypes.Role, AppRoles.Директор.ToString()))
            {
                requests = _DBworkStationsService.GetRequests();
            }
            else if (_authorizationService != null)
            {
                int masterId;
                if (int.TryParse(_authorizationService.CurrentUser.Identity.Name, out masterId))
                {
                    requests = _DBworkStationsService.GetRequests(
                        masterId: masterId
                    );
                }
            }

            foreach (var request in requests)
            {
                _requestsViewModels.Add(new RequestViewModel(request));
            }
        }

        
        [RelayCommand]
        private void SaveChanges()
        {
            if (CurrentlySelectedRequest != null && CurrentlySelectedRequest.IsChanged)
            {
                _DBworkStationsService.AddOrEditRequest(CurrentlySelectedRequest.Request);
                CurrentlySelectedRequest.IsChanged = false;
            }
        }
        [RelayCommand]
        private void AddNewRequest()
        { 
            Request request = new Request() 
            { 
                CreationDate = DateTime.UtcNow, 
                ClientId = CurrentlySelectedClient?.Client.ClientId ?? 0 
            };
            RequestViewModel requestsViewModel = new RequestViewModel(request);
            RequestsViewModels.Add(requestsViewModel);
            CurrentlySelectedRequest = requestsViewModel;
        }


        [RelayCommand]
        private void AddMaster()
        {
            if (CurrentlySelectedRequest != null)
            {
                CurrentlySelectedRequest.SetMaster(currentCurrentStaffAccount);
            }
        }

        [RelayCommand]
        private void CancelChanges()
        {
            if (CurrentlySelectedRequest != null && CurrentlySelectedRequest.IsChanged)
            {
                
                RefreshRequests();
            }
        }

        public string Title => "Заявки";
        public AppRoles[] AllowedRoles { get; }
    }
}