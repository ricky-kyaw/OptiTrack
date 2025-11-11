using OptiTrack.Business.Services;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace OptiTrack.App.UI.ctrls
{
    public partial class ClockIn : UserControl
    {
        private readonly AttendanceService _attendanceService;

        // 🔹 Dependency Property - MUST be Guid
        public static readonly DependencyProperty EmployeeIdProperty =
            DependencyProperty.Register(nameof(EmployeeId), typeof(Guid), typeof(ClockIn), new PropertyMetadata(Guid.Empty));

        public Guid EmployeeId
        {
            get => (Guid)GetValue(EmployeeIdProperty);
            set => SetValue(EmployeeIdProperty, value);
        }

        // 🔹 Event raised when clock in succeeds
        public event Action ClockInCompleted;

        public ClockIn()
        {
            InitializeComponent();
            _attendanceService = new AttendanceService();
        }

        private async void ClockInButton_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeeId == Guid.Empty)
            {
                MessageBox.Show("Employee ID is missing. Cannot clock in.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Guid safeEmployeeId = EmployeeId;

            try
            {
                await Task.Run(() => _attendanceService.ClockIn(safeEmployeeId));

                MessageBox.Show("✅ Successfully Clocked In!",
                    "Clock In",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show(
                    "⚠️ You have already clocked in.\nYou must clock out before clocking in again.",
                    "Already Clocked In",
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
