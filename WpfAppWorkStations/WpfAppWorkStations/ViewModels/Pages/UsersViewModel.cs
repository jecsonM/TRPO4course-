using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Security.Claims;
using System.Windows;
using WpfAppWorkStations.EntityFramework;
using WpfAppWorkStations.Enums;
using WpfAppWorkStations.Interfaces.Services;
using WpfAppWorkStations.Interfaces.ViewModels;
using WpfAppWorkStations.ViewModels.EFViewModels;

namespace WpfAppWorkStations.ViewModels.Pages
{
    public partial class UsersViewModel : BaseViewModel, IPageViewModel
    {
        private IDBWorkStationsService _DBworkStationsService;
        private IAuthorizationService _authorizationService;
        private IPasswordService _passwordService;

        public UsersViewModel(
            IDBWorkStationsService workStationsService,
            IAuthorizationService authorizationService,
            IPasswordService passwordService)
        {
            _DBworkStationsService = workStationsService;
            _authorizationService = authorizationService;
            _passwordService = passwordService;
            AllowedRoles = new AppRoles[] { AppRoles.Директор, AppRoles.СистемныйАдминистратор };

            _staffViewModels = new ObservableCollection<StaffViewModel>();
            _rolesViewModels = new ObservableCollection<RoleViewModel>();

            if (_authorizationService != null)
            {
                _authorizationService.UserChanged += (cp) => FullRefresh();
            }

            FullRefresh();
        }

        private void FullRefresh()
        {
            RefreshUsers();
            RefreshRoles();
        }

        private ObservableCollection<StaffViewModel> _staffViewModels;
        public ObservableCollection<StaffViewModel> StaffViewModels => _staffViewModels;

        private ObservableCollection<RoleViewModel> _rolesViewModels;
        public ObservableCollection<RoleViewModel> RolesViewModels => _rolesViewModels;

        private StaffViewModel currentlySelectedStaff;
        public StaffViewModel CurrentlySelectedStaff
        {
            get => currentlySelectedStaff;
            set
            {
                currentlySelectedStaff = value;
                NewPassword = string.Empty;
                OnPropertyChanged(nameof(CurrentlySelectedStaff));
                OnPropertyChanged(nameof(CanChangePassword));
            }
        }

        [ObservableProperty]
        private string newPassword;

        [ObservableProperty]
        private string newUserLogin;

        [ObservableProperty]
        private string newUserPassword;

        [ObservableProperty]
        private RoleViewModel selectedNewUserRole;

        // Проверка, может ли текущий пользователь менять пароль выбранному
        public bool CanChangePassword
        {
            get
            {
                if (CurrentlySelectedStaff?.Staff == null) return false;

                var currentUserRole = _authorizationService?.CurrentUser?
                    .FindFirst(ClaimTypes.Role)?.Value;

                var selectedUserRole = CurrentlySelectedStaff.Staff.Role?.RoleName;

                // Если выбранный пользователь - Директор, то только Директор может менять ему пароль
                if (selectedUserRole == AppRoles.Директор.ToString())
                {
                    return currentUserRole == AppRoles.Директор.ToString();
                }

                return true;
            }
        }

        [RelayCommand]
        public void RefreshUsers()
        {
            _staffViewModels.Clear();
            List<Staff> staff = _DBworkStationsService.GetStaff();
            foreach (var staffMember in staff)
            {
                _staffViewModels.Add(new StaffViewModel(staffMember));
            }
        }

        [RelayCommand]
        public void RefreshRoles()
        {
            _rolesViewModels.Clear();
            List<Role> roles = _DBworkStationsService.GetRoles();
            foreach (var role in roles)
            {
                _rolesViewModels.Add(new RoleViewModel(role));
            }
        }

        [RelayCommand]
        private void SetNewPassword()
        {
            if (CurrentlySelectedStaff == null)
            {
                MessageBox.Show("Выберите пользователя", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(NewPassword))
            {
                MessageBox.Show("Введите новый пароль", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!CanChangePassword)
            {
                MessageBox.Show("Только директор может менять пароль директору", "Доступ запрещен",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Установить новый пароль для пользователя {CurrentlySelectedStaff.Staff.Login}?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var passwordHash = _passwordService.HashPassword(NewPassword);
                    _DBworkStationsService.UpdateStaffPassword(CurrentlySelectedStaff.Staff.StaffId, passwordHash);

                    // Обновляем пользователя в списке
                    CurrentlySelectedStaff.Staff.PasswordHash = passwordHash;

                    MessageBox.Show("Пароль успешно изменен", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    NewPassword = string.Empty;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при смене пароля: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        [RelayCommand]
        private void AddNewUser()
        {
            if (string.IsNullOrWhiteSpace(NewUserLogin))
            {
                MessageBox.Show("Введите логин", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(NewUserPassword))
            {
                MessageBox.Show("Введите пароль", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (SelectedNewUserRole == null)
            {
                MessageBox.Show("Выберите роль", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Проверяем, существует ли пользователь с таким логином
            var existingUser = _DBworkStationsService.GetStaffByLogin(NewUserLogin);
            if (existingUser != null)
            {
                MessageBox.Show("Пользователь с таким логином уже существует", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var passwordHash = _passwordService.HashPassword(NewUserPassword);

                var newStaff = new Staff
                {
                    Login = NewUserLogin,
                    PasswordHash = passwordHash,
                    RoleId = SelectedNewUserRole.Role.RoleId,
                    Role = SelectedNewUserRole.Role
                };

                _DBworkStationsService.AddStaff(newStaff);

                // Обновляем список пользователей
                RefreshUsers();

                // Сбрасываем поля
                NewUserLogin = string.Empty;
                NewUserPassword = string.Empty;
                SelectedNewUserRole = null;

                MessageBox.Show("Пользователь успешно создан", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании пользователя: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void DeleteUser()
        {
            if (CurrentlySelectedStaff == null)
            {
                MessageBox.Show("Выберите пользователя", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var currentUserLogin = _authorizationService?.CurrentUser?.Identity?.Name;
            if (CurrentlySelectedStaff.Staff.Login == currentUserLogin)
            {
                MessageBox.Show("Нельзя удалить самого себя", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var result = MessageBox.Show(
                $"Удалить пользователя {CurrentlySelectedStaff.Staff.Login}?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _DBworkStationsService.DeleteStaff(CurrentlySelectedStaff.Staff.StaffId);
                    RefreshUsers();
                    CurrentlySelectedStaff = null;

                    MessageBox.Show("Пользователь удален", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public string Title => "Пользователи";
        public AppRoles[] AllowedRoles { get; }
    }
}