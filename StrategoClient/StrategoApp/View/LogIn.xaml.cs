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
    /// Lógica de interacción para LogIn.xaml
    /// </summary>
    public partial class LogIn : UserControl
    {
        public LogIn()
        {
            InitializeComponent();
            this.Cursor = new Cursor(Application.GetResourceStream(new Uri("pack://application:,,,/StrategoApp;component/Assets/Cursors/normal_cursor.cur")).Stream);
        }
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LogInViewModel viewModel)
            {
                if (!viewModel.IsPasswordVisible)
                {
                    viewModel.Password = ((PasswordBox)sender).Password;
                }
            }
        }

        private void SyncPassword(object sender, RoutedEventArgs e)
        {
            if (DataContext is LogInViewModel viewModel)
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
