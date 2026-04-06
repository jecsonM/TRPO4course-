using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Security.Claims;
using System.Windows;
using WpfAppWorkStations.EntityFramework;
using WpfAppWorkStations.Enums;
using WpfAppWorkStations.Interfaces.Services;
using WpfAppWorkStations.Interfaces.ViewModels;
using WpfAppWorkStations.ViewModels;
using WpfAppWorkStations.ViewModels.EFViewModels;

namespace WpfAppWorkStations.ViewModels.Pages
{
    public partial class ServicesViewModel : BaseViewModel, IPageViewModel
    {
        private IDBWorkStationsService _DBworkStationsService;
        private IAuthorizationService _authorizationService;

        public ServicesViewModel(IDBWorkStationsService workStationsService, IAuthorizationService authorizationService = null)
        {
            _DBworkStationsService = workStationsService;
            AllowedRoles = new AppRoles[] { AppRoles.Мастер, AppRoles.Директор };
            _authorizationService = authorizationService;

            _servicesViewModels = new ObservableCollection<ServiceViewModel>();

            if (_authorizationService != null)
            {
                _authorizationService.UserChanged += (cp) => FullRefresh();
            }

            FullRefresh();
        }

        private void FullRefresh()
        {
            RefreshServices();
        }

        private ObservableCollection<ServiceViewModel> _servicesViewModels;
        public ObservableCollection<ServiceViewModel> ServicesViewModels => _servicesViewModels;

        private ServiceViewModel currentlySelectedService;
        public ServiceViewModel CurrentlySelectedService
        {
            get => currentlySelectedService;
            set
            {
                currentlySelectedService = value;
                OnPropertyChanged(nameof(CurrentlySelectedService));
            }
        }

        [ObservableProperty]
        private string newServicePrice;

        [RelayCommand]
        public void RefreshServices()
        {
            _servicesViewModels.Clear();
            List<Machineservice> services = _DBworkStationsService.GetServices();

            foreach (var service in services)
            {
                var serviceVM = new ServiceViewModel(service);
                // Загружаем историю цен для каждой услуги
                var costs = _DBworkStationsService.GetRelevantCostsByService(service.ServiceId);
                serviceVM.LoadRelevantCosts(costs);
                _servicesViewModels.Add(serviceVM);
            }
        }

        [RelayCommand]
        private void SaveChanges()
        {
            if (CurrentlySelectedService != null && CurrentlySelectedService.IsChanged)
            {
                _DBworkStationsService.AddOrEditService(CurrentlySelectedService.Service);
                CurrentlySelectedService.IsChanged = false;
                RefreshServices();
            }
        }

        [RelayCommand]
        private void AddNewService()
        {
            Machineservice service = new Machineservice()
            {
                MachineServiceName = "Новая услуга"
            };
            ServiceViewModel serviceViewModel = new ServiceViewModel(service);
            ServicesViewModels.Add(serviceViewModel);
            CurrentlySelectedService = serviceViewModel;
        }

        [RelayCommand]
        private void AddNewPrice()
        {
            if (CurrentlySelectedService == null)
                return;

            if (!decimal.TryParse(NewServicePrice, out decimal price))
            {
                MessageBox.Show("Введите корректную цену (например: 1500.50)", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var currentUser = _authorizationService?.CurrentUser;
            if (currentUser?.Identity == null)
            {
                MessageBox.Show("Пользователь не авторизован", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var staff = _DBworkStationsService.GetStaffByLogin(currentUser.Identity.Name);
            if (staff == null)
            {
                MessageBox.Show("Не удалось определить текущего пользователя", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Relevantcost newCost = new Relevantcost()
            {
                ServiceId = CurrentlySelectedService.Service.ServiceId,
                CreatorsId = staff.StaffId,
                RelevantCost1 = price,
                SetDate = DateTime.UtcNow,
                Creators = staff
            };

            _DBworkStationsService.AddRelevantCost(newCost);

            // Обновляем историю цен в ViewModel
            var updatedCosts = _DBworkStationsService.GetRelevantCostsByService(CurrentlySelectedService.Service.ServiceId);
            CurrentlySelectedService.LoadRelevantCosts(updatedCosts);
            CurrentlySelectedService.IsChanged = true;

            NewServicePrice = string.Empty;

            MessageBox.Show("Цена успешно добавлена", "Успех",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        [RelayCommand]
        private void DeleteService()
        {
            if (CurrentlySelectedService == null)
                return;

            var result = MessageBox.Show(
                $"Вы уверены, что хотите удалить услугу:\n\n" +
                $"Название: {CurrentlySelectedService.Service.MachineServiceName}",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _DBworkStationsService.DeleteService(CurrentlySelectedService.Service.ServiceId);
                    ServicesViewModels.Remove(CurrentlySelectedService);
                    CurrentlySelectedService = null;

                    MessageBox.Show("Услуга успешно удалена", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        [RelayCommand]
        private void CancelChanges()
        {
            if (CurrentlySelectedService != null && CurrentlySelectedService.IsChanged)
            {
                RefreshServices();
            }
        }

        public string Title => "Услуги";
        public AppRoles[] AllowedRoles { get; }
    }
}  