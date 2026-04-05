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
    public partial class MachinesViewModel : BaseViewModel, IPageViewModel
    {
        private IDBWorkStationsService _DBworkStationsService;
        private IAuthorizationService _authorizationService;

        public MachinesViewModel(IDBWorkStationsService workStationsService, IAuthorizationService authorizationService = null)
        {
            _DBworkStationsService = workStationsService;
            AllowedRoles = new AppRoles[] { AppRoles.Мастер, AppRoles.Директор };
            _authorizationService = authorizationService;

            _machinesViewModels = new ObservableCollection<MachineViewModel>();
            _clientViewModels = new ObservableCollection<ClientViewModel>();

            if (_authorizationService != null)
            {
                _authorizationService.UserChanged += (cp) => FullRefresh();
            }

            FullRefresh();
        }

        private void FullRefresh()
        {
            RefreshMachines();
            RefreshClients();
        }

        private ObservableCollection<MachineViewModel> _machinesViewModels;
        public ObservableCollection<MachineViewModel> MachinesViewModels => _machinesViewModels;

        private ObservableCollection<ClientViewModel> _clientViewModels;
        public ObservableCollection<ClientViewModel> ClientViewModels => _clientViewModels;

        private ClientViewModel _currentlySelectedClient;
        public ClientViewModel CurrentlySelectedClient
        {
            get => _currentlySelectedClient;
            set
            {
                if (currentlySelectedMachine != null &&
                    value?.Client?.ClientId != currentlySelectedMachine.Machine.ClientId)
                {
                    currentlySelectedMachine.SetClient(value?.Client);
                }
                _currentlySelectedClient = value;
                OnPropertyChanged(nameof(CurrentlySelectedClient));
            }
        }

        private MachineViewModel currentlySelectedMachine;
        public MachineViewModel CurrentlySelectedMachine
        {
            get => currentlySelectedMachine;
            set
            {
                currentlySelectedMachine = value;
                if (value != null)
                {
                    _currentlySelectedClient = ClientViewModels
                        .FirstOrDefault(cvm => cvm.Client.ClientId == value.Machine.ClientId);
                }
                OnPropertyChanged(nameof(CurrentlySelectedMachine));
                OnPropertyChanged(nameof(CurrentlySelectedClient));
            }
        }


        [RelayCommand]
        private async Task DeleteMachine()
        {
            if (CurrentlySelectedMachine == null)
                return;

            var result = MessageBox.Show(
                $"Вы уверены, что хотите удалить оборудование:\n" +
                $"Модель: {CurrentlySelectedMachine.Machine.Model}\n" +
                $"Серийный номер: {CurrentlySelectedMachine.Machine.SerialNumber}",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _DBworkStationsService.DeleteMachine(CurrentlySelectedMachine.Machine.MachineId);

                    MachinesViewModels.Remove(CurrentlySelectedMachine);

                    CurrentlySelectedMachine = null;

                    MessageBox.Show("Оборудование успешно удалено", "Успех",
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
        public void RefreshMachines()
        {
            _machinesViewModels.Clear();
            List<Machine> machines = _DBworkStationsService.GetMachines();
            foreach (var machine in machines)
            {
                _machinesViewModels.Add(new MachineViewModel(machine));
            }
        }

        [RelayCommand]
        private void SaveChanges()
        {
            if (CurrentlySelectedMachine != null && CurrentlySelectedMachine.IsChanged)
            {
                _DBworkStationsService.AddOrEditMachine(CurrentlySelectedMachine.Machine);
                CurrentlySelectedMachine.IsChanged = false;
            }
        }

        [RelayCommand]
        private void AddNewMachine()
        {
            Machine machine = new Machine()
            {
                ClientId = CurrentlySelectedClient?.Client?.ClientId ?? 0
            };
            MachineViewModel machineViewModel = new MachineViewModel(machine);
            MachinesViewModels.Add(machineViewModel);
            CurrentlySelectedMachine = machineViewModel;
        }

        [RelayCommand]
        private void CancelChanges()
        {
            if (CurrentlySelectedMachine != null && CurrentlySelectedMachine.IsChanged)
            {
                RefreshMachines();
            }
        }

        public string Title => "Оборудование";
        public AppRoles[] AllowedRoles { get; }
    }
}