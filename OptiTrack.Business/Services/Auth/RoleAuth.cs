using OptiTrack.Data.DBConnector;
using System;
using System.Collections.Generic;

namespace OptiTrack.Business.Services.Auth
{
    public class RoleAuth
    {
        private readonly ViewModelsConnector _vConnector;

        public RoleAuth()
        {
            _vConnector = new ViewModelsConnector();
        }

        /// <summary>
        /// Checks if a user has a specific role.
        /// </summary>
        public bool HasRole(Guid appUserId, string requiredRole)
        {
            return _vConnector.UserHasRole(appUserId, requiredRole);
        }

        /// <summary>
        /// Returns all roles assigned to a user.
        /// </summary>
        public List<string> GetRoles(Guid appUserId)
        {
            return _vConnector.GetRolesForUser(appUserId);
        }
    }
}
