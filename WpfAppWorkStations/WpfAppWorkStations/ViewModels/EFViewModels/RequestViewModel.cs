using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfAppWorkStations.EntityFramework;
using WpfAppWorkStations.Interfaces.ViewModels;
using WpfAppWorkStations.ViewModels.EFViewModels;

namespace WpfAppWorkStations.ViewModels
{
    public partial class RequestViewModel : BaseViewModel, IEFViewModel
    {
        [ObservableProperty]
        private bool isChanged;

        [ObservableProperty]
        private Request request;

        public ObservableCollection<RelevantRequestStateViewModel> relevantRequestStates;

        public string MastersLoginDisplay
        {
            get
            {
                if (request.Master == null)
                    return "Мастер не закреплён";
                else
                    return $"Мастер: {request.Master.Login}";
            }
        }


        public void SetClient(Client client)
        {
            request.ClientId = client.ClientId;
            request.Client = client;
            IsChanged = true;
            OnPropertyChanged(nameof(Request));
        }

        public void SetMaster(Staff master)
        {
            request.Master = master;
            IsChanged = true;
            OnPropertyChanged(nameof(MastersLoginDisplay));
        }
        public RequestViewModel(Request request)
        {
            this.request = request;
            isChanged = false;
        }
    }
}
