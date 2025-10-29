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
using System.Windows.Shapes;
using OptiTrack.Data.DBConnector;

namespace OptiTrack.App.UI.general
{
    public partial class LoginPage : Window
    {
        TableModelsConnector connector;
        ViewModelsConnector vConnector;

        public LoginPage()
        {
            InitializeComponent();
            connector = new TableModelsConnector();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            UsernameTextBox.Text = "";


        }

        private void EmployeeRadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void AdminRadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void UsernameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
