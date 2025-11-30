using OptiTrack.Data;
using OptiTrack.Data.DBMLs;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace OptiTrack.App.UI.admin
{
    public partial class ManageEmployees : Window
    {
        private List<vw_AppUserWithRole> _allEmployees;

        public ManageEmployees()
        {
            InitializeComponent();
            LoadEmployees();
        }

        private void LoadEmployees()
        {
            using (var ctx = new ViewModelsDataContext(Config.ConnectionString))
            {
                _allEmployees = ctx.vw_AppUserWithRoles.ToList();
                EmployeeGrid.ItemsSource = _allEmployees;
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_allEmployees == null)
                return;

            string query = SearchBox.Text.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(query))
            {
                EmployeeGrid.ItemsSource = _allEmployees;
                return;
            }

            var filtered = _allEmployees
                .Where(emp =>
                    (!string.IsNullOrEmpty(emp.FirstName) && emp.FirstName.ToLower().Contains(query)) ||
                    (!string.IsNullOrEmpty(emp.LastName) && emp.LastName.ToLower().Contains(query)) ||
                    (!string.IsNullOrEmpty(emp.Email) && emp.Email.ToLower().Contains(query)) ||
                    (!string.IsNullOrEmpty(emp.DepartmentName) && emp.DepartmentName.ToLower().Contains(query)) ||
                    (!string.IsNullOrEmpty(emp.RoleName) && emp.RoleName.ToLower().Contains(query))
                )
                .ToList();

            EmployeeGrid.ItemsSource = filtered;
        }
        private void EditEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is vw_AppUserWithRole emp)
            {
                var editWindow = new EditEmployeeWindow(emp.AppUserID);
                editWindow.ShowDialog();

                // Refresh list after editing
                LoadEmployees();
            }
        }
    }
}
