using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptiTrack.Business.PasswordHasher
{
    public static class PasswordHasher
    {
        public static string HashPassword(string password, string salt)
        {
            // In a real application, you would use a stronger, salted, iterative hash function like PBKDF2.
            // This is a simple concatenation hash for demonstration purposes only.
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var inputBytes = Encoding.UTF8.GetBytes(password + salt);
                var hashBytes = sha256.ComputeHash(inputBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }
    }

    public enum UserRole { Employee, Admin }

    // Mock Models based on the provided schema (simplified for login)
    public class Role
    {
        public int RoleID { get; set; }
        public UserRole RoleName { get; set; }
    }

    public class AppUser
    {
        public int AppUserID { get; set; }
        public int EmployeeID { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
    }

    public class AppUserRole
    {
        public int AppUserID { get; set; }
        public int RoleID { get; set; }
    }

    public class Employee
    {
        public int EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class TableModelsConnector
    {
        // Mock data to simulate the database state
        private List<Role> Roles = new List<Role>
        {
            new Role { RoleID = 1, RoleName = UserRole.Admin },
            new Role { RoleID = 2, RoleName = UserRole.Employee }
        };

        // Define salts and create pre-hashed passwords for 'admin' and 'user'
        private readonly string AdminSalt = "AdminSecureSalt";
        private readonly string EmployeeSalt = "EmployeeSecureSalt";

        private readonly string HashedAdminPassword; // Password: "adminpass"
        private readonly string HashedEmployeePassword; // Password: "userpass"

        private List<AppUser> AppUsers;

        private List<AppUserRole> AppUserRoles = new List<AppUserRole>
        {
            new AppUserRole { AppUserID = 101, RoleID = 1 }, // Admin
            new AppUserRole { AppUserID = 102, RoleID = 2 }  // Employee
        };

        private List<Employee> Employees = new List<Employee>
        {
            new Employee { EmployeeID = 1, FirstName = "Alice", LastName = "Admin" },
            new Employee { EmployeeID = 2, FirstName = "Bob", LastName = "User" }
        };


        public TableModelsConnector()
        {
            HashedAdminPassword = PasswordHasher.HashPassword("adminpass", AdminSalt);
            HashedEmployeePassword = PasswordHasher.HashPassword("userpass", EmployeeSalt);

            AppUsers = new List<AppUser>
            {
                // Admin User
                new AppUser { AppUserID = 101, EmployeeID = 1, Username = "admin", PasswordHash = HashedAdminPassword, PasswordSalt = AdminSalt },
                // Employee User
                new AppUser { AppUserID = 102, EmployeeID = 2, Username = "user", PasswordHash = HashedEmployeePassword, PasswordSalt = EmployeeSalt }
            };
        }

        // Authentication logic
        public bool AuthenticateUser(string username, string password, UserRole requiredRole)
        {
            var user = AppUsers.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

            if (user == null)
            {
                return false; // User not found
            }

            // 1. Verify password
            string inputHash = PasswordHasher.HashPassword(password, user.PasswordSalt);
            if (!inputHash.Equals(user.PasswordHash))
            {
                return false; // Password mismatch
            }

            // 2. Verify role
            var userRoleLink = AppUserRoles.FirstOrDefault(aul => aul.AppUserID == user.AppUserID);
            if (userRoleLink == null)
            {
                return false; // No role assigned
            }

            var role = Roles.FirstOrDefault(r => r.RoleID == userRoleLink.RoleID);

            // Check if the user's role matches the required role selected on the login page
            return role != null && role.RoleName == requiredRole;
        }

        public Employee GetEmployeeDetails(int employeeId)
        {
            return Employees.FirstOrDefault(e => e.EmployeeID == employeeId);
        }
    }
}
