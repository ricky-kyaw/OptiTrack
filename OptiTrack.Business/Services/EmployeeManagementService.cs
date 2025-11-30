using OptiTrack.Business.PasswordHasher;
using OptiTrack.Data;
using OptiTrack.Data.DBConnector;
using OptiTrack.Data.DBMLs;
using System;
using System.Linq;

namespace OptiTrack.Business.Services
{
    /// <summary>
    /// Handles all business logic related to managing employee records and app user accounts.
    /// </summary>
    public class EmployeeManagementService
    {
        // Initializes the connector for database interaction
        private readonly TableModelsConnector _connector = new TableModelsConnector();

        /// <summary>
        /// Creates a new employee, sets up their AppUser account, and assigns a role in a single transaction.
        /// </summary>
        public Employee CreateNewEmployee(
    string firstName, string lastName, string email,
    Guid jobTitleId, Guid departmentId, Guid payTypeId,
    decimal payRate, string roleName, string initialPassword)
        {
            string salt = SecurePasswordHasher.CreateSalt();
            string hashedPassword = SecurePasswordHasher.HashPassword(initialPassword, salt);

            Guid employeeGuid = Guid.NewGuid();
            Guid appUserGuid = Guid.NewGuid();

            var newAppUser = new AppUser
            {
                AppUserID = appUserGuid,
                EmployeeID = employeeGuid,
                PasswordHash = hashedPassword,
                PasswordSalt = salt,
                IsActive = true
            };

            var newEmployee = new Employee
            {
                EmployeeID = employeeGuid,
                AppUserID = appUserGuid,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                JobTitleID = jobTitleId,
                DepartmentID = departmentId,
                PayTypeID = payTypeId,
                PayRate = payRate,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            using (var db = new TableModelsDataContext(Config.ConnectionString))
            {
                // STEP 1 — Insert AppUser FIRST
                db.AppUsers.InsertOnSubmit(newAppUser);
                db.SubmitChanges();

                // STEP 2 — Insert Employee SECOND
                db.Employees.InsertOnSubmit(newEmployee);
                db.SubmitChanges();

                // STEP 3 — Insert Role THIRD
                var role = db.Roles.First(r => r.RoleName == roleName);

                var appUserRole = new AppUserRole
                {
                    AppUserID = appUserGuid,
                    RoleID = role.RoleID
                };

                db.AppUserRoles.InsertOnSubmit(appUserRole);
                db.SubmitChanges();
            }

            return newEmployee;
        }


        // --- DASHBOARD STATS METHODS ---

        /// <summary>
        /// Retrieves the total number of active employees in the system.
        /// </summary>
        public int GetTotalActiveEmployeesCount()
        {
            // This relies on the connector method correctly counting based on AppUsers.IsActive == true
            return _connector.GetTotalEmployeesCount();
        }

        // --- Other Employee Management methods (View, Delete, Update) would go here ---
    }
}