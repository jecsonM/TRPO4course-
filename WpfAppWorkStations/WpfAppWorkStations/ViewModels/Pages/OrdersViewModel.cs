using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.ObjectModel;
using System.Security.Claims;
using WpfAppWorkStations.EntityFramework;
using WpfAppWorkStations.Enums;
using WpfAppWorkStations.Interfaces.Services;
using WpfAppWorkStations.Interfaces.ViewModels;
using WpfAppWorkStations.Stores;
using WpfAppWorkStations.ViewModels;
using WpfAppWorkStations.ViewModels.EFViewModels;
using WpfAppWorkStations.Views;

namespace WpfAppWorkStations.ViewModels.Pages
{
    public partial class OrdersViewModel : BaseViewModel, IPageViewModel
    {
        private IDBWorkStationsService _DBworkStationsService;
        private IAuthorizationService _authorizationService;
        private NavigationStore _navigationStore;

        public OrdersViewModel(IDBWorkStationsService workStationsService, IAuthorizationService authorizationService, NavigationStore navigationStore)
        {
            _DBworkStationsService = workStationsService;
            AllowedRoles = new AppRoles[] { AppRoles.Мастер, AppRoles.Бухгалтер };
            _authorizationService = authorizationService;
            _navigationStore = navigationStore;

            _ordersViewModels = new ObservableCollection<OrderViewModel>();
            _requestsViewModels = new ObservableCollection<RequestViewModel>();
            _relevantOrderStates = new ObservableCollection<RelevantOrderStateViewModel>();
            _orderStates = new ObservableCollection<OrderStateViewModel>();


            List<Orderstate> orderStates = _DBworkStationsService.GetOrderstates();
            if (
                !_authorizationService.CurrentUser?.IsInRole(AppRoles.Бухгалтер.ToString()) ?? true
                &&
                (!_authorizationService.CurrentUser?.IsInRole(AppRoles.Директор.ToString()) ?? true))
            {
                orderStates.Remove(orderStates.FirstOrDefault(os => os.OrderStateName == "Одобрен"));
            }

            foreach (var orderState in orderStates)
            {
                OrderStates.Add(new OrderStateViewModel(orderState));
            }

            RefreshRequests();

            if (_authorizationService != null)
            {
                _authorizationService.UserChanged += (cp) => FullRefresh();
            }

            FullRefresh();
        }

        [RelayCommand]
        private void AddMachine()
        {
            if (CurrentlySelectedOrder != null && SelectedMachineToAdd != null)
            {
                // Добавляем оборудование в заказ
                CurrentlySelectedOrder.Order.Machines.Add(SelectedMachineToAdd.Machine);
                CurrentlySelectedOrder.IsChanged = true;

                // Обновляем список доступного оборудования
                LoadAvailableMachines();

                // Сбрасываем выбранное оборудование
                SelectedMachineToAdd = null;

                OnPropertyChanged(nameof(CurrentlySelectedOrder.Order.Machines));
            }
        }

        [RelayCommand]
        private void RemoveMachine(Machine machineToRemove)
        {
            if (CurrentlySelectedOrder != null && machineToRemove != null)
            {
                // Удаляем оборудование из заказа
                CurrentlySelectedOrder.Order.Machines.Remove(machineToRemove);
                CurrentlySelectedOrder.IsChanged = true;

                // Обновляем список доступного оборудования
                LoadAvailableMachines();

                OnPropertyChanged(nameof(CurrentlySelectedOrder.Order.Machines));
            }
        }

        [RelayCommand]
        private void FullRefresh()
        {
            RefreshOrders();
            RefreshRequests();
        }

        private ObservableCollection<OrderViewModel> _ordersViewModels;
        public ObservableCollection<OrderViewModel> OrdersViewModels => _ordersViewModels;

        private ObservableCollection<RequestViewModel> _requestsViewModels;
        public ObservableCollection<RequestViewModel> RequestsViewModels => _requestsViewModels;

        private ObservableCollection<RelevantOrderStateViewModel> _relevantOrderStates;
        public ObservableCollection<RelevantOrderStateViewModel> RelevantOrderStates
            => _relevantOrderStates;

        private ObservableCollection<OrderStateViewModel> _orderStates;
        public ObservableCollection<OrderStateViewModel> OrderStates => _orderStates;

        [ObservableProperty]
        private OrderStateViewModel currentlySelectedOrderState;

        private RequestViewModel _currentlySelectedRequestForOrder;
        public RequestViewModel CurrentlySelectedRequestForOrder
        {
            get => _currentlySelectedRequestForOrder;
            set
            {
                _currentlySelectedRequestForOrder = value;
                if (currentlySelectedOrder != null && value != null)
                {
                    currentlySelectedOrder.Order.RequestId = value.Request.RequestId;
                    currentlySelectedOrder.Order.Request = value.Request;
                    currentlySelectedOrder.IsChanged = true;
                    LoadAvailableMachines();
                }
                OnPropertyChanged(nameof(CurrentlySelectedRequestForOrder));
            }
        }

        private OrderViewModel currentlySelectedOrder;
        public OrderViewModel CurrentlySelectedOrder
        {
            get => currentlySelectedOrder;
            set
            {
                currentlySelectedOrder = value;
                if (value != null)
                {
                    // Находим связанную заявку
                    _currentlySelectedRequestForOrder = RequestsViewModels
                        .FirstOrDefault(rvm => rvm.Request.RequestId == value.Order.RequestId);

                    // Загружаем состояния заказа
                    List<Relevantorderstate> relevantOrderStates =
                        _DBworkStationsService.GetRelevantorderstates(currentlySelectedOrder.Order);
                    RelevantOrderStates.Clear();
                    foreach (Relevantorderstate ros in relevantOrderStates)
                    {
                        RelevantOrderStates.Add(new RelevantOrderStateViewModel(ros));
                    }

                    // Загружаем доступное оборудование для этой заявки
                    LoadAvailableMachines();
                }

                OnPropertyChanged(nameof(CurrentlySelectedOrder));
                OnPropertyChanged(nameof(CurrentlySelectedRequestForOrder));
            }
        }

        private void LoadAvailableMachines()
        {
            _availableMachinesForOrder = new ObservableCollection<MachineViewModel>();

            if (currentlySelectedOrder?.Order?.Request?.ClientId > 0)
            {

                var machines = _DBworkStationsService.GetMachinesByClient(
                    currentlySelectedOrder.Order.Request.ClientId);

                var existingMachineIds = currentlySelectedOrder.Order.Machines
                    .Select(m => m.MachineId)
                    .ToHashSet();

                var availableMachines = machines.Where(m => !existingMachineIds.Contains(m.MachineId));

                foreach (var machine in availableMachines)
                {
                    _availableMachinesForOrder.Add(new MachineViewModel(machine));
                }
            }

            OnPropertyChanged(nameof(AvailableMachinesForOrder));
        }

        [RelayCommand]
        private void AddOrderState()
        {
            if (CurrentlySelectedOrder != null && CurrentlySelectedOrderState != null)
            {
                Relevantorderstate relevantOrderState = new Relevantorderstate()
                {
                    OrderId = CurrentlySelectedOrder.Order.OrderId,
                    OrderStateId = CurrentlySelectedOrderState.Orderstate.OrderStateId,
                    SetDate = DateTime.UtcNow,
                    OrderState = CurrentlySelectedOrderState.Orderstate
                };
                _DBworkStationsService.AddRelevantOrderState(relevantOrderState);
                RelevantOrderStates.Add(new RelevantOrderStateViewModel(relevantOrderState));
                CurrentlySelectedOrder.IsChanged = true;
            }
        }

        [RelayCommand]
        public void RefreshRequests()
        {
            RequestsViewModels.Clear();

            List<Request> requests = new List<Request>();

            if (_authorizationService != null &&
                _authorizationService.CurrentUser.HasClaim(ClaimTypes.Role, AppRoles.Директор.ToString()))
            {
                requests = _DBworkStationsService.GetRequests();
            }
            else if (_authorizationService.CurrentUser != null)
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
                RequestsViewModels.Add(new RequestViewModel(request));
            }
        }

        [RelayCommand]
        public void RefreshOrders()
        {
            _ordersViewModels.Clear();

            List<Order> orders = _DBworkStationsService.GetOrders();

            foreach (var order in orders)
            {
                _ordersViewModels.Add(new OrderViewModel(order));
            }
        }

        [RelayCommand]
        private void SaveChanges()
        {
            if (CurrentlySelectedOrder != null && CurrentlySelectedOrder.IsChanged)
            {
                _DBworkStationsService.AddOrEditOrder(CurrentlySelectedOrder.Order);
                CurrentlySelectedOrder.IsChanged = false;
            }
        }

        private ObservableCollection<MachineViewModel> _availableMachinesForOrder;
        public ObservableCollection<MachineViewModel> AvailableMachinesForOrder
            => _availableMachinesForOrder;

        private MachineViewModel _selectedMachineToAdd;
        public MachineViewModel SelectedMachineToAdd
        {
            get => _selectedMachineToAdd;
            set
            {
                _selectedMachineToAdd = value;
                OnPropertyChanged(nameof(SelectedMachineToAdd));
            }
        }

        [RelayCommand]
        private void AddNewOrder()
        {
            Order order = new Order()
            {
                CreationDate = DateTime.UtcNow,
                RequestId = CurrentlySelectedRequestForOrder?.Request.RequestId ?? 0
            };
            OrderViewModel orderViewModel = new OrderViewModel(order);
            OrdersViewModels.Add(orderViewModel);
            CurrentlySelectedOrder = orderViewModel;
        }

        [RelayCommand]
        private void CancelChanges()
        {
            if (CurrentlySelectedOrder != null && CurrentlySelectedOrder.IsChanged)
            {
                RefreshOrders();
            }
        }

        public string Title => "Заказы";
        public AppRoles[] AllowedRoles { get; }
    }
}