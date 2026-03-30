using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfAppWorkStations.Interfaces.ViewModels;

namespace WpfAppWorkStations.ViewModels.Pages
{
    public class MainWindowViewModel : BaseViewModel, IPageViewModel
    {
        public string Title { get => "Главное окно"; }
    }
}
