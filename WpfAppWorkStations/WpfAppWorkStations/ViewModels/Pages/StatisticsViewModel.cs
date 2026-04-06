using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Windows;
using WpfAppWorkStations.Enums;
using WpfAppWorkStations.Interfaces.Services;
using WpfAppWorkStations.Interfaces.ViewModels;

namespace WpfAppWorkStations.ViewModels.Pages
{
    public partial class StatisticsViewModel : BaseViewModel, IPageViewModel
    {
        private IDBWorkStationsService _DBworkStationsService;

        public StatisticsViewModel(IDBWorkStationsService workStationsService)
        {
            _DBworkStationsService = workStationsService;
            AllowedRoles = new AppRoles[] { AppRoles.Бухгалтер };

            // Устанавливаем значения по умолчанию
            FromDate = DateTime.Now.AddMonths(-1);
            ToDate = DateTime.Now;
        }

        [ObservableProperty]
        private DateTime fromDate;

        [ObservableProperty]
        private DateTime toDate;

        [ObservableProperty]
        private decimal totalIncome;

        [ObservableProperty]
        private bool isCalculating;

        [RelayCommand]
        private async System.Threading.Tasks.Task CalculateIncome()
        {
            if (FromDate > ToDate)
            {
                MessageBox.Show("Дата 'от' не может быть позже даты 'до'",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            IsCalculating = true;

            try
            {
                DateTime FromDateUTC = FromDate.ToUniversalTime();
                DateTime ToDateUTC = ToDate.ToUniversalTime();
                // Запускаем расчет в отдельном потоке
                var result = await System.Threading.Tasks.Task.Run(() =>
                    _DBworkStationsService.GetOrderSummForPeriod(FromDateUTC, ToDateUTC));

                TotalIncome = result;

                MessageBox.Show($"Расчет завершен. Сумма за период: {TotalIncome:F2} ₽",
                    "Результат", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при расчете: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsCalculating = false;
            }
        }

        public string Title => "Статистика";
        public AppRoles[] AllowedRoles { get; }
    }
}