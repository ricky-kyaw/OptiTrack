using OptiTrack.App.UI.admin;
using OptiTrack.App.UI.employees;
using OptiTrack.Business.Services.Auth;
using OptiTrack.Data.DBMLs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace OptiTrack.App.UI.general
{
    public partial class LoginPage : Window
    {
        private readonly LoginAuth _loginAuth;
        private readonly RoleAuth _roleAuth;

        private string selectedAccountType = "Employee"; // Default role selection

        public LoginPage()
        {
            InitializeComponent();
            _loginAuth = new LoginAuth();
            _roleAuth = new RoleAuth();
        }

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

            // Authenticate user → returns vw_AppUserWithRole
            var user = _loginAuth.Authenticate(email, password, out roles);

            if (user == null)
            {
                MessageBox.Show("Invalid email or password.", "Login Failed",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                PasswordBox.Clear();
                return;
            }

            // Ensure user matches selected account type (Employee/Admin)
            if (!roles.Contains(selectedAccountType))
            {
                MessageBox.Show($"Your account is not registered as {selectedAccountType}.",
                                "Access Denied",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                PasswordBox.Clear();
                return;
            }

            // Route to dashboard based on role
            if (_roleAuth.HasRole(user.AppUserID, "Admin"))
            {
                var dashboard = new adminDashBoard();
                dashboard.Show();
                this.Close();
            }
            else if (_roleAuth.HasRole(user.AppUserID, "Employee"))
            {
                var dashboard = new EmployeeDashboard(
                    user,                                      // ✅ Pass vw_AppUserWithRole
                    $"{user.FirstName} {user.LastName}",       // ✅ Employee Full Name
                    user.DepartmentName,                      // ✅ Department from updated view
                    "Employee"                                 // ✅ Role
                );

                dashboard.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("No assigned system role. Access denied.",
                                "Login Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            PasswordBox.Clear();
        }

        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Forgot password feature is not implemented yet.",
                            "Info",
                            MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void EmailTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }
    }
}
