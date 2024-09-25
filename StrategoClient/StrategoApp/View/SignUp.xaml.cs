using StrategoApp.Properties;
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

        private void MailTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (MailTextBox.Text == Properties.Resources.UserMail_Label)
            {
                MailTextBox.Text = string.Empty;
            }
        }

        private void MailTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(MailTextBox.Text)) 
            {
                MailTextBox.Text = Properties.Resources.UserMail_Label;
            }
        }

        private void UsernameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (UsernameTextBox.Text == Properties.Resources.UserName_Label)
            {
                UsernameTextBox.Text = string.Empty;
            }
        }

        private void UsernameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(UsernameTextBox.Text))
            {
                UsernameTextBox.Text = Properties.Resources.UserName_Label;
            }
        }

        private void PasswordTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            WatermarkTextBoxPassword.Visibility = Visibility.Collapsed;
        }

        private void PasswordTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(PasswordTextBox.Password))
            {
                WatermarkTextBoxPassword.Visibility = Visibility.Visible;
            }
        }

    }
}
