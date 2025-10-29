using OptiTrack.Data.DBConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptiTrack.Business.Services.Auth
{
    public class RoleAuth
    {
        private readonly TableModelsConnector _connector;

        public RoleAuth()
        {
            _connector = new TableModelsConnector();
        }

        public bool HasRole(Guid appUserId, string requiredRole)
        {
            return _connector.UserHasRole(appUserId, requiredRole);
        }

        public List<string> GetRoles(Guid appUserId)
        {
            return _connector.GetRolesForUser(appUserId);
        }
    }
}
