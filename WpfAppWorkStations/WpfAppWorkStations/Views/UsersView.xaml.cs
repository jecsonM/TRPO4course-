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

namespace WpfAppWorkStations.Views
{
    /// <summary>
    /// Interaction logic for UsersView.xaml
    /// </summary>
    public partial class UsersView : UserControl
    {
        public UsersView()
        {
            InitializeComponent();
        }

        private void NewPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is ViewModels.Pages.UsersViewModel viewModel)
            {
                var passwordBox = sender as PasswordBox;
                viewModel.NewPassword = passwordBox?.Password ?? "";
            }
        }

        private void NewUserPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is ViewModels.Pages.UsersViewModel viewModel)
            {
                var passwordBox = sender as PasswordBox;
                viewModel.NewUserPassword = passwordBox?.Password ?? "";
            }
        }
    }
}
