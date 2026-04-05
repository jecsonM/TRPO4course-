using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfAppWorkStations.Enums;
using WpfAppWorkStations.Interfaces.Services;
using WpfAppWorkStations.Interfaces.ViewModels;

namespace WpfAppWorkStations.ViewModels.EFViewModels
{
    public class RequestsViewModel : IPageViewModel
    {
        private IDBWorkStationsService _workStationsService;

        public RequestsViewModel(IDBWorkStationsService workStationsService)
        {
            _workStationsService = workStationsService;
        }

        private ObservableCollection<RequestsViewModel> _requestsViewModels;
        public ObservableCollection<RequestsViewModel> RequestsViewModels => _requestsViewModels;

        public void RefreshRequests()
        {

        }

        public AppRoles[] AllowedRoles => throw new NotImplementedException();

        public string Title => "Заявки";

    }
}
