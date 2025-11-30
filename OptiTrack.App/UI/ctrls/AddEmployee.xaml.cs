using OptiTrack.Business.Services;
using OptiTrack.Data.DBConnector;
using OptiTrack.Data.DBMLs;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using OptiTrack.Data;

namespace OptiTrack.App.UI.ctrls
{
    public partial class AddEmployee : UserControl
    {
        private readonly TableModelsConnector _connector = new TableModelsConnector();
        private readonly EmployeeManagementService _employeeService = new EmployeeManagementService();

        public AddEmployee()
        {
            InitializeComponent();
            LoadDropdowns();
        }

        private void LoadDropdowns()
        {
            using (var db = new TableModelsDataContext(Config.ConnectionString))
            {
                // DEPARTMENTS
                DepartmentCombo.ItemsSource = db.Departments
                    .Select(d => new { d.DepartmentID, d.DepartmentName })
                    .ToList();
                DepartmentCombo.DisplayMemberPath = "DepartmentName";
                DepartmentCombo.SelectedValuePath = "DepartmentID";

                // JOB TITLES
                JobTitleCombo.ItemsSource = db.JobTitles
                    .Select(j => new { j.JobTitleID, j.TitleName })
                    .ToList();
                JobTitleCombo.DisplayMemberPath = "TitleName";
                JobTitleCombo.SelectedValuePath = "JobTitleID";

                // PAY TYPES
                PayTypeCombo.ItemsSource = db.PayTypes
                    .Select(p => new { p.PayTypeID, p.TypeName })
                    .ToList();
                PayTypeCombo.DisplayMemberPath = "TypeName";
                PayTypeCombo.SelectedValuePath = "PayTypeID";

                // ROLES
                RoleCombo.ItemsSource = db.Roles
                    .Select(r => new { r.RoleID, r.RoleName })
                    .ToList();
                RoleCombo.DisplayMemberPath = "RoleName";
                RoleCombo.SelectedValuePath = "RoleName";
            }
        }



        private void AddEmployeeBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Basic Validation
                if (string.IsNullOrWhiteSpace(FirstNameBox.Text) ||
                    string.IsNullOrWhiteSpace(LastNameBox.Text) ||
                    string.IsNullOrWhiteSpace(EmailBox.Text) ||
                    string.IsNullOrWhiteSpace(PayRateBox.Text) ||
                    PasswordBox.Password.Length == 0 ||
                    DepartmentCombo.SelectedValue == null ||
                    JobTitleCombo.SelectedValue == null ||
                    PayTypeCombo.SelectedValue == null ||
                    RoleCombo.SelectedValue == null)
                {
                    MessageBox.Show("Please fill in all required fields.", "Missing Info", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Convert PayRate safely
                if (!decimal.TryParse(PayRateBox.Text, out decimal payRate))
                {
                    MessageBox.Show("Invalid Pay Rate. Must be a number.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Create employee
                var employee = _employeeService.CreateNewEmployee(
                    FirstNameBox.Text,
                    LastNameBox.Text,
                    EmailBox.Text,
                    (Guid)JobTitleCombo.SelectedValue,
                    (Guid)DepartmentCombo.SelectedValue,
                    (Guid)PayTypeCombo.SelectedValue,
                    payRate,
                    RoleCombo.SelectedValue.ToString(),
                    PasswordBox.Password
                );

                MessageBox.Show("Employee created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                // Clear fields
                FirstNameBox.Text = "";
                LastNameBox.Text = "";
                EmailBox.Text = "";
                PayRateBox.Text = "";
                PasswordBox.Password = "";
                DepartmentCombo.SelectedIndex = -1;
                JobTitleCombo.SelectedIndex = -1;
                PayTypeCombo.SelectedIndex = -1;
                RoleCombo.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                string details = ex.InnerException?.Message ?? ex.Message;

                MessageBox.Show(
                    $"Error creating employee:\n{details}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }

        }
    }
}
