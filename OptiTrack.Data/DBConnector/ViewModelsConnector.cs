using OptiTrack.Data.DBMLs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OptiTrack.Data.DBConnector
{
    /// <summary>
    /// Provides data access methods specifically using View Models from the DBML.
    /// Uses the recommended pattern of creating and disposing the DataContext per method call
    /// for thread safety and connection efficiency.
    /// </summary>
    public sealed class ViewModelsConnector
    {
        // Private property to simplify connection string access
        private static string ConnectionString => Properties.Resources.connectionString;

        /// <summary>
        /// Retrieves the complete user and role data for authentication based on email.
        /// </summary>
        /// <param name="email">The email address of the user.</param>
        /// <returns>The user's view model data, or null if not found.</returns>
        public vw_AppUserWithRole GetUserByEmail(string email)
        {
            using (var dataContext = new ViewModelsDataContext(ConnectionString))
            {
                return dataContext.vw_AppUserWithRoles
                                  .FirstOrDefault(u => u.Email == email);
            }
        }

        /// <summary>
        /// Checks if a specified user has a required role name.
        /// </summary>
        /// <param name="appUserId">The ID of the application user (GUID).</param>
        /// <param name="requiredRole">The role name to check.</param>
        /// <returns>True if the user has the role, otherwise False.</returns>
        public bool UserHasRole(Guid appUserId, string requiredRole)
        {
            using (var dataContext = new ViewModelsDataContext(ConnectionString))
            {
                return dataContext.vw_AppUserWithRoles
                                  .Any(u => u.AppUserID == appUserId &&
                                            u.RoleName.Equals(requiredRole));
            }
        }

        /// <summary>
        /// Retrieves a list of all role names associated with a user.
        /// </summary>
        /// <param name="appUserId">The ID of the application user (GUID).</param>
        /// <returns>A list of unique role names.</returns>
        public List<string> GetRolesForUser(Guid appUserId)
        {
            using (var dataContext = new ViewModelsDataContext(ConnectionString))
            {
                return dataContext.vw_AppUserWithRoles
                                  .Where(u => u.AppUserID == appUserId)
                                  .Select(u => u.RoleName)
                                  .Distinct()
                                  .ToList();
            }
        }
        public List<Employee> GetAllEmployees()
        {
            using (var db = new TableModelsDataContext(Properties.Resources.connectionString))
            {
                return db.Employees
                         .OrderBy(e => e.FirstName)
                         .ThenBy(e => e.LastName)
                         .ToList();
            }
        }

        public List<Attendance> GetAllAttendanceLogs()
        {
            using (var db = new TableModelsDataContext(Properties.Resources.connectionString))
            {
                return db.Attendances
                         .OrderByDescending(a => a.ClockInTime)
                         .ToList();
            }
        }
        public List<dynamic> GetAllEmployeesFull()
        {
            using (var db = new TableModelsDataContext(Properties.Resources.connectionString))
            {
                var list = (from e in db.Employees
                            join d in db.Departments on e.DepartmentID equals d.DepartmentID
                            join j in db.JobTitles on e.JobTitleID equals j.JobTitleID
                            select new
                            {
                                e.EmployeeID,
                                e.FirstName,
                                e.LastName,
                                e.Email,
                                Department = d.DepartmentName,
                                JobTitle = j.TitleName,
                                e.PayRate
                            }).ToList<dynamic>();

                return list;
            }
        }

        public List<dynamic> GetAllAttendanceLogsFull()
        {
            using (var db = new TableModelsDataContext(Properties.Resources.connectionString))
            {
                var logs = (from a in db.Attendances
                            join e in db.Employees on a.EmployeeID equals e.EmployeeID
                            orderby a.ClockInTime descending
                            select new
                            {
                                Employee = e.FirstName + " " + e.LastName,
                                a.ClockInTime,
                                a.ClockOutTime
                            }).ToList<dynamic>();

                return logs;
            }
        }

    }
}
