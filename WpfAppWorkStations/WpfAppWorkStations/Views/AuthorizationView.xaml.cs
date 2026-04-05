using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfAppWorkStations.EntityFramework;
using WpfAppWorkStations.Services;
using WpfAppWorkStations.ViewModels.Pages;

namespace WpfAppWorkStations.Views
{
    /// <summary>
    /// Interaction logic for AuthorizationView.xaml
    /// </summary>
    public partial class AuthorizationView : UserControl
    {
        public AuthorizationView()
        {
            InitializeComponent();
        }

        private async void PasswordBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ButtonAuthorize_Click(sender, null);
            }
        }

        private async void ButtonAuthorize_Click(object sender, RoutedEventArgs e)
        {
            


            AuthorizationViewModel authorizationViewModel = DataContext as AuthorizationViewModel;
            await authorizationViewModel.Authorize(LoginBox.Text, PasswordBox.Password);
        }
    }
}
