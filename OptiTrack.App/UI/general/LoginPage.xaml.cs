using OptiTrack.App.UI.admin;
using OptiTrack.App.UI.employees;
using OptiTrack.Business.Services.Auth;
using OptiTrack.Data.DBConnector;
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

namespace OptiTrack.App.UI.general
{
    public partial class LoginPage : Window
    {
        private readonly LoginAuth _loginAuth;
        private readonly RoleAuth _roleAuth;

        private string selectedAccountType = "Employee"; // default

        public LoginPage()
        {
            InitializeComponent();
            _loginAuth = new LoginAuth();
            _roleAuth = new RoleAuth();
        }

        // Account type radio button handlers
        private void EmployeeRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            selectedAccountType = "Employee";
        }

        private void AdminRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            selectedAccountType = "Admin";
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text.Trim();
            string password = PasswordBox.Password;
            List<string> roles;

            // 1️⃣ Authenticate user
            var user = _loginAuth.Authenticate(email, password, out roles);

            if (user == null)
            {
                MessageBox.Show("Invalid email or password.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                PasswordBox.Clear();
                return;
            }

            // Optional: check if user has selected correct account type
            if (!roles.Contains(selectedAccountType))
            {
                MessageBox.Show($"Your account is not registered as {selectedAccountType}.", "Access Denied", MessageBoxButton.OK, MessageBoxImage.Warning);
                PasswordBox.Clear();
                return;
            }

            // 2️⃣ Open dashboard based on role
            if (_roleAuth.HasRole(user.AppUserID, "Admin"))
            {
               new adminDashBoard().ShowDialog();
            }
            else if (_roleAuth.HasRole(user.AppUserID, "Employee"))
            {
                new employeeDashboard().ShowDialog();
            }
            else
            {
                MessageBox.Show("No role assigned. Access denied.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            // Clear password after login attempt
            PasswordBox.Clear();
        }

        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            // TODO: implement password reset
            MessageBox.Show("Forgot password functionality is not implemented yet.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void EmailTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // Optional: you can validate email format here
        }
    }
}
