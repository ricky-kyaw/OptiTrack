using OptiTrack.App.UI.ctrls;
using OptiTrack.App.UI.general;
using OptiTrack.Business.Services;
using OptiTrack.Data.DBMLs;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace OptiTrack.App.UI.employees
{
    public partial class EmployeeDashboard : Window
    {
        private readonly AttendanceService _attendanceService;

        public Guid EmployeeId { get; private set; }
        public string EmployeeName { get; private set; }
        public string Department { get; private set; }
        public string UserRole { get; private set; }

        public static readonly DependencyProperty IsClockedInProperty =
            DependencyProperty.Register(nameof(IsClockedIn), typeof(bool), typeof(EmployeeDashboard), new PropertyMetadata(false));

        public bool IsClockedIn
        {
            get => (bool)GetValue(IsClockedInProperty);
            set => SetValue(IsClockedInProperty, value);
        }

        // ✅ Updated constructor to accept vw_AppUserWithRole
        public EmployeeDashboard(vw_AppUserWithRole appUser, string employeeFullName, string department, string role)
        {
            InitializeComponent();

            _attendanceService = new AttendanceService();

            // KEEP — needed for ClockIn / ClockOut logic
            EmployeeId = appUser.EmployeeID ?? Guid.Empty;

            // SHOW — displayed in dashboard
            EmployeeName = employeeFullName;
            Department = department;
            UserRole = role;

            DataContext = this;

            Loaded += EmployeeDashboard_Loaded;
        }


        private async void EmployeeDashboard_Loaded(object sender, RoutedEventArgs e)
        {
            await UpdateAttendanceStatus();
        }

        public async Task UpdateAttendanceStatus()
        {
            try
            {
                Guid employeeGuid = EmployeeId; // ✅ capture on UI thread

                bool isClockedIn = await Task.Run(() =>
                    _attendanceService.IsClockedIn(employeeGuid)); // ✅ safe

                Dispatcher.Invoke(() =>
                {
                    IsClockedIn = isClockedIn;
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while checking attendance status:\n{ex.Message}",
                                "System Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);

                IsClockedIn = false;
            }
        }


        private void ViewOvertimeButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Overtime view logic not yet implemented.",
                            "Info",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to log out?",
                                "Confirm Logout",
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                new LoginPage().Show();
                Close();
            }
        }
    }
}
