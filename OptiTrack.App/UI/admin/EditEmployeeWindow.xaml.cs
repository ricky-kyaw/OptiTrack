using OptiTrack.Data.DBMLs;
using OptiTrack.Data.DBConnector;
using System;
using System.Linq;
using System.Windows;

namespace OptiTrack.App.UI.admin
{
    public partial class EditEmployeeWindow : Window
    {
        private readonly Guid _appUserId;
        private Guid _employeeId;

        public EditEmployeeWindow(Guid appUserId)
        {
            InitializeComponent();
            _appUserId = appUserId;
            LoadData();
        }

        private void LoadData()
        {
            // 1. Get basic info from VIEW
            using (var ctx = new ViewModelsDataContext(OptiTrack.Data.Properties.Resources.connectionString))
            {
                var vm = ctx.vw_AppUserWithRoles.FirstOrDefault(u => u.AppUserID == _appUserId);
                if (vm == null)
                {
                    MessageBox.Show("Employee not found.");
                    this.Close();
                    return;
                }

                _employeeId = vm.EmployeeID ?? Guid.Empty;

                EmailBox.Text = vm.Email;
                FirstNameBox.Text = vm.FirstName;
                LastNameBox.Text = vm.LastName;
            }

            // 2. Load real employee table info
            using (var db = new TableModelsDataContext(OptiTrack.Data.Properties.Resources.connectionString))
            {
                var emp = db.Employees.FirstOrDefault(e => e.EmployeeID == _employeeId);
                if (emp == null)
                {
                    MessageBox.Show("Employee record missing.");
                    return;
                }

                DepartmentCombo.ItemsSource = db.Departments.ToList();
                DepartmentCombo.DisplayMemberPath = "DepartmentName";
                DepartmentCombo.SelectedValuePath = "DepartmentID";
                DepartmentCombo.SelectedValue = emp.DepartmentID;

                JobTitleCombo.ItemsSource = db.JobTitles.ToList();
                JobTitleCombo.DisplayMemberPath = "TitleName";
                JobTitleCombo.SelectedValuePath = "JobTitleID";
                JobTitleCombo.SelectedValue = emp.JobTitleID;

                PayTypeCombo.ItemsSource = db.PayTypes.ToList();
                PayTypeCombo.DisplayMemberPath = "TypeName";
                PayTypeCombo.SelectedValuePath = "PayTypeID";
                PayTypeCombo.SelectedValue = emp.PayTypeID;

                PayRateBox.Text = emp.PayRate.ToString();

                RoleCombo.ItemsSource = db.Roles.ToList();
                RoleCombo.DisplayMemberPath = "RoleName";
                RoleCombo.SelectedValuePath = "RoleName";

                var currentRoleId = db.AppUserRoles
                                      .First(r => r.AppUserID == _appUserId)
                                      .RoleID;

                RoleCombo.SelectedValue = db.Roles
                                           .First(r => r.RoleID == currentRoleId)
                                           .RoleName;
            }
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            using (var ctx = new TableModelsDataContext(OptiTrack.Data.Properties.Resources.connectionString))
            {
                var user = ctx.AppUsers.FirstOrDefault(a => a.AppUserID == _appUserId);
                var emp = ctx.Employees.FirstOrDefault(e => e.EmployeeID == _employeeId);

                if (user == null || emp == null)
                {
                    MessageBox.Show("Employee not found in database.");
                    return;
                }

                // Validate required dropdowns (avoids null Guid? errors)
                if (DepartmentCombo.SelectedValue == null ||
                    JobTitleCombo.SelectedValue == null ||
                    PayTypeCombo.SelectedValue == null)
                {
                    MessageBox.Show("Please select Department, Job Title, and Pay Type.");
                    return;
                }

                // Validate PayRate
                if (!decimal.TryParse(PayRateBox.Text, out decimal payRate))
                {
                    MessageBox.Show("Invalid pay rate.");
                    return;
                }

                // Update fields
                emp.FirstName = FirstNameBox.Text;
                emp.LastName = LastNameBox.Text;
                emp.DepartmentID = ((Guid?)DepartmentCombo.SelectedValue).Value;
                emp.JobTitleID = ((Guid?)JobTitleCombo.SelectedValue).Value;
                emp.PayTypeID = ((Guid?)PayTypeCombo.SelectedValue).Value;
                emp.PayRate = payRate;

                // Update role
                var newRoleName = RoleCombo.SelectedValue?.ToString();
                var role = ctx.Roles.FirstOrDefault(r => r.RoleName == newRoleName);

                if (role == null)
                {
                    MessageBox.Show("Selected role does not exist!");
                    return;
                }

                var userRole = ctx.AppUserRoles.First(r => r.AppUserID == _appUserId);
                userRole.RoleID = role.RoleID;

                ctx.SubmitChanges();
            }

            MessageBox.Show("Employee updated successfully!");
            this.Close();
        }

    }
}
