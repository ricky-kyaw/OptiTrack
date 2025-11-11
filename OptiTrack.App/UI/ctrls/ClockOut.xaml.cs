using OptiTrack.Business.Services;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace OptiTrack.App.UI.ctrls
{
    public partial class ClockOut : UserControl
    {
        private readonly AttendanceService _attendanceService;

        // 🔹 Dependency Property - MUST be Guid
        public static readonly DependencyProperty EmployeeIdProperty =
            DependencyProperty.Register(nameof(EmployeeId), typeof(Guid), typeof(ClockOut), new PropertyMetadata(Guid.Empty));

        public Guid EmployeeId
        {
            get => (Guid)GetValue(EmployeeIdProperty);
            set => SetValue(EmployeeIdProperty, value);
        }

        // 🔹 Event raised when clock-out succeeds
        public event Action ClockOutCompleted;

        public ClockOut()
        {
            InitializeComponent();
            _attendanceService = new AttendanceService();
        }

        private async void ClockOutButton_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeeId == Guid.Empty)
            {
                MessageBox.Show("Employee ID is missing. Cannot clock out.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Guid safeEmployeeId = EmployeeId;

            try
            {
                await Task.Run(() => _attendanceService.ClockOut(safeEmployeeId));

                MessageBox.Show("✅ Successfully Clocked Out!",
                    "Clock Out",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show(
                    "⚠️ You cannot clock out without clocking in first.",
                    "Clock Out Failed",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error:\n{ex.Message}",
                    "System Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}
