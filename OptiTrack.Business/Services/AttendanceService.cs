using OptiTrack.Data.DBConnector;
using OptiTrack.Data.DBMLs;
using System;

namespace OptiTrack.Business.Services
{
    /// <summary>
    /// Handles business logic for employee attendance (Clock In/Out).
    /// </summary>
    public class AttendanceService
    {
        private readonly TableModelsConnector _connector;

        public AttendanceService()
        {
            _connector = new TableModelsConnector();
        }

        /// <summary>
        /// Records a new Clock In entry for an employee.
        /// </summary>
        /// <param name="employeeId">Employee GUID.</param>
        /// <returns>The newly created Attendance record.</returns>
        public Attendance ClockIn(Guid employeeId)
        {
            if (employeeId == Guid.Empty)
                throw new ArgumentException("Employee ID cannot be empty.", nameof(employeeId));

            // Ensure no open shift exists
            var openShift = _connector.GetOpenAttendanceRecord(employeeId);
            if (openShift != null)
                throw new InvalidOperationException("Cannot clock in: there is already an active shift for this employee.");

            var now = DateTime.UtcNow;

            var newRecord = new Attendance
            {
                AttendanceID = Guid.NewGuid(),
                EmployeeID = employeeId,
                ClockInTime = now,
                ClockOutTime = null
            };

            _connector.AddAttendanceRecord(newRecord);
            return newRecord;
        }

        /// <summary>
        /// Records the Clock Out time for the employee's open shift.
        /// </summary>
        /// <param name="employeeId">Employee GUID.</param>
        /// <returns>The updated Attendance record.</returns>
        public Attendance ClockOut(Guid employeeId)
        {
            if (employeeId == Guid.Empty)
                throw new ArgumentException("Employee ID cannot be empty.", nameof(employeeId));

            var openShift = _connector.GetOpenAttendanceRecord(employeeId);
            if (openShift == null)
                throw new InvalidOperationException("Cannot clock out: no active shift found for this employee.");

            var now = DateTime.UtcNow;

            // Defensive check
            if (openShift.ClockInTime >= now)
                throw new InvalidOperationException("Clock-out time must be after clock-in time.");

            openShift.ClockOutTime = now;

            _connector.UpdateAttendanceRecord(openShift);
            return openShift;
        }

        /// <summary>
        /// True if an open shift exists for the employee.
        /// </summary>
        public bool IsClockedIn(Guid employeeId)
        {
            if (employeeId == Guid.Empty)
                throw new ArgumentException("Employee ID cannot be empty.", nameof(employeeId));

            return _connector.GetOpenAttendanceRecord(employeeId) != null;
        }
    }
}
