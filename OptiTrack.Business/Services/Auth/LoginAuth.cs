using System;
using System.Collections.Generic;
using OptiTrack.Business.PasswordHasher;
using OptiTrack.Data.DBConnector;
using OptiTrack.Data.DBMLs;

namespace OptiTrack.Business.Services.Auth
{
    public class LoginAuth
    {
        private readonly ViewModelsConnector _vConnector;

        public LoginAuth()
        {
            _vConnector = new ViewModelsConnector();
        }

        /// <summary>
        /// Authenticates a user by email and password.
        /// Returns the FULL vw_AppUserWithRole record if valid, otherwise null.
        /// Out parameter 'roles' holds all roles assigned to the user.
        /// </summary>
        public vw_AppUserWithRole Authenticate(string email, string password, out List<string> roles)
        {
            roles = new List<string>();

            // 1. Fetch user from view
            vw_AppUserWithRole userView = _vConnector.GetUserByEmail(email);
            if (userView == null)
                return null;

            // 2. Verify password using SecurePasswordHasher
            bool isValid = SecurePasswordHasher.VerifyPassword(password, userView.PasswordHash, userView.PasswordSalt);
            if (!isValid)
                return null;

            // 3. Fetch all roles for the user
            roles = _vConnector.GetRolesForUser(userView.AppUserID);

            // ✅ Directly return full vw model (contains FirstName, LastName, DepartmentName)
            return userView;
        }

        /// <summary>
        /// Checks if a user has a specific role.
        /// </summary>
        public bool UserHasRole(Guid appUserId, string requiredRole)
        {
            return _vConnector.UserHasRole(appUserId, requiredRole);
        }
    }
}
