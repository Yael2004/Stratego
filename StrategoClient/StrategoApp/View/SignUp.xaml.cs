using StrategoApp.Properties;
using StrategoApp.ViewModel;
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

namespace StrategoApp.View
{
    /// <summary>
    /// Lógica de interacción para SignUp.xaml
    /// </summary>
    public partial class SignUp : UserControl
    {
        public SignUp()
        {
            InitializeComponent();
        }

        private void PasswordTextBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is SignUpViewModel viewModel)
            {
                viewModel.Password = ((PasswordBox)sender).Password;
            }
        }

        private void ShowPassword(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is SignUpViewModel viewModel)
            {
                viewModel.IsPasswordVisible = true;
            }
        }

        private void HidePassword(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is SignUpViewModel viewModel)
            {
                viewModel.IsPasswordVisible = false;
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is SignUpViewModel viewModel)
            {
                viewModel.Password = ((PasswordBox)sender).Password;
            }
        }

        private void SyncPassword(object sender, RoutedEventArgs e)
        {
            if (DataContext is SignUpViewModel viewModel)
            {
                if (viewModel.IsPasswordVisible)
                {
                    PasswordTextBox.Text = PasswordBox.Password;
                }
                else
                {
                    PasswordBox.Password = PasswordTextBox.Text;
                }
            }
        }
    }
}
