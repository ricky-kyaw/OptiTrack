using OptiTrack.Data;
using OptiTrack.Data.DBMLs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace OptiTrack.App.UI.admin
{
    public partial class ViewAllLogs : Window
    {
        private List<AttendanceLogView> _allLogs;

        public ViewAllLogs()
        {
            InitializeComponent();
            LoadLogs();
        }

        private void LoadLogs()
        {
            using (var ctx = new TableModelsDataContext(Config.ConnectionString))
            {
                // Join Attendance with Employees to show names/emails
                var raw = (from att in ctx.Attendances
                           join emp in ctx.Employees on att.EmployeeID equals emp.EmployeeID
                           orderby att.ClockInTime descending
                           select new { att, emp })
                          .ToList();

                _allLogs = raw.Select(x => new AttendanceLogView
                {
                    EmployeeName = (x.emp.FirstName ?? "") + " " + (x.emp.LastName ?? ""),
                    Email = x.emp.Email,
                    ClockInTime = x.att.ClockInTime,
                    ClockOutTime = x.att.ClockOutTime,
                    HoursWorked = x.att.ClockOutTime.HasValue
                        ? (x.att.ClockOutTime.Value - x.att.ClockInTime).TotalHours
                        : 0
                }).ToList();

                LogsGrid.ItemsSource = _allLogs;
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_allLogs == null)
                return;

            string query = SearchBox.Text.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(query))
            {
                LogsGrid.ItemsSource = _allLogs;
                return;
            }

            var filtered = _allLogs
                .Where(l =>
                    (!string.IsNullOrEmpty(l.EmployeeName) && l.EmployeeName.ToLower().Contains(query)) ||
                    (!string.IsNullOrEmpty(l.Email) && l.Email.ToLower().Contains(query)))
                .ToList();

            LogsGrid.ItemsSource = filtered;
        }
    }

    // Simple view model for grid binding
    public class AttendanceLogView
    {
        public string EmployeeName { get; set; }
        public string Email { get; set; }
        public DateTime ClockInTime { get; set; }
        public DateTime? ClockOutTime { get; set; }
        public double HoursWorked { get; set; }
    }
}
