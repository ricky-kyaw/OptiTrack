using OptiTrack.Data.DBMLs;
using System;
using System.Linq;
using System.Collections.Generic;

namespace OptiTrack.Data.DBConnector
{
    public class TableModelsConnector
    {
        private TableModelsDataContext dataContext;

        /// <summary>
        /// Provides access to the single, long-lived DataContext instance.
        /// WARNING: This approach can lead to concurrency issues in multi-threaded environments.
        /// </summary>
        public TableModelsDataContext dataContextCaller
        {
            get
            {
                if (dataContext == null)
                {
                    // This assumes Properties.Resources.connectionString is a static resource containing the SQL connection string.
                    dataContext = new TableModelsDataContext(Properties.Resources.connectionString);
                }
                return dataContext;
            }
        }

        // --- ATTENDANCE METHODS ---

        /// <summary>
        /// Retrieves the single open attendance record for an employee (ClockOutTime is NULL).
        /// </summary>
        public Attendance GetOpenAttendanceRecord(Guid employeeId)
        {
            // Use the established dataContextCaller property
            return dataContextCaller.Attendances
                                    .FirstOrDefault(a => a.EmployeeID == employeeId && a.ClockOutTime == null);
        }

        /// <summary>
        /// Adds a new Attendance record (Clock In).
        /// </summary>
        public void AddAttendanceRecord(Attendance record)
        {
            dataContextCaller.Attendances.InsertOnSubmit(record);
            dataContextCaller.SubmitChanges();
        }

        /// <summary>
        /// Updates an existing Attendance record (Clock Out).
        /// </summary>
        public void UpdateAttendanceRecord(Attendance record)
        {
            // IMPORTANT: Because the DataContext is persistent, we rely on the object passed
            // to this method (the 'record') being the same instance tracked by the DataContext,
            // or we must manually attach it.

            // To be safe in this pattern, we must ensure LINQ to SQL is tracking the changes
            // and the object isn't stale. Since we assume the record was retrieved via
            // GetOpenAttendanceRecord from this same DataContext, we proceed to submit changes.

            // If the object was retrieved and modified elsewhere, you would need to use 
            // dataContextCaller.Attendances.Attach(record, true/false) and then SubmitChanges().

            dataContextCaller.SubmitChanges();
        }

        // --- Other necessary methods (E.g., for AppUser CRUD) would go here ---
    }
}