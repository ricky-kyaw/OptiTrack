using OptiTrack.Data.DBMLs;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Linq;

namespace OptiTrack.Data.DBConnector
{
    public class TableModelsConnector
    {
        private TableModelsDataContext dataContext;

        /// <summary>
        /// Provides access to the single, long-lived DataContext instance (per user preference).
        /// </summary>
        public TableModelsDataContext dataContextCaller
        {
            get
            {
                if (dataContext == null)
                {
                    dataContext = new TableModelsDataContext(Properties.Resources.connectionString);
                }
                return dataContext;
            }
        }

        // --- ATTENDANCE METHODS ---

        public Attendance GetOpenAttendanceRecord(Guid employeeId)
        {
            return dataContextCaller.Attendances
                                    .FirstOrDefault(a => a.EmployeeID == employeeId && a.ClockOutTime == null);
        }

        public void AddAttendanceRecord(Attendance record)
        {
            dataContextCaller.Attendances.InsertOnSubmit(record);
            dataContextCaller.SubmitChanges();
        }

        public void UpdateAttendanceRecord(Attendance record)
        {
            dataContextCaller.SubmitChanges();
        }


        // --- EMPLOYEE MANAGEMENT METHOD (ADD/CREATE) ---

        public void AddEmployeeAccount(Employee employee, AppUser appUser, string roleName)
        {
            var role = dataContextCaller.Roles.FirstOrDefault(r => r.RoleName == roleName);
            if (role == null)
            {
                throw new InvalidOperationException($"Role '{roleName}' not found in the database. Cannot assign user.");
            }

            var appUserRole = new AppUserRole
            {
                AppUserID = appUser.AppUserID,
                RoleID = role.RoleID
            };

            dataContextCaller.AppUsers.InsertOnSubmit(appUser);
            dataContextCaller.Employees.InsertOnSubmit(employee);
            dataContextCaller.AppUserRoles.InsertOnSubmit(appUserRole);

            try
            {
                dataContextCaller.SubmitChanges();
            }
            catch (ChangeConflictException)
            {
                throw new InvalidOperationException("Concurrency conflict occurred during employee creation. Please try again.");
            }
            catch (Exception ex)
            {
                throw new Exception("Database error during employee creation. Check for duplicate email or invalid FKs.", ex);
            }
        }

        // --- DASHBOARD STATISTICS METHODS ---

        /// <summary>
        /// Retrieves the total number of active employee records.
        /// FIX: Rewritten using method-chaining to explicitly join AppUsers and safely check AppUser.IsActive.
        /// </summary>
        public int GetTotalEmployeesCount()
        {
            // Assuming the table collection is named 'Employees' and the class is 'Employee' (standard behavior).
            // We join Employees (e) and AppUsers (au) and filter the result on the AppUser's IsActive property.
            return dataContextCaller.Employees
                .Join(dataContextCaller.AppUsers,
                    e => e.AppUserID,
                    au => au.AppUserID,
                    (e, au) => new { Employee = e, AppUser = au })
                .Count(j => j.AppUser.IsActive == true);
        }

        /// <summary>
        /// Retrieves the count of current open shifts (employees clocked in).
        /// </summary>
        public int GetClockedInCount()
        {
            // Count Attendance records where ClockOutTime is NULL
            return dataContextCaller.Attendances.Count(a => a.ClockOutTime == null);
        }
        public TableModelsDataContext GetContext()
        {
            return new TableModelsDataContext(Config.ConnectionString);
        }

    }
}