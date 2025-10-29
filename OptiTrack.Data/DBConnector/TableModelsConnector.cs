using OptiTrack.Data.DBMLs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptiTrack.Data.DBConnector
{
    public class TableModelsConnector
    {
        TableModelsDataContext dataContext;

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
        public AppUser GetUserByEmail(string email)
        {
            // Join Employees → AppUsers using EmployeeID
            var query = from emp in dataContextCaller.Employees
                        join au in dataContextCaller.AppUsers
                            on emp.EmployeeID equals au.EmployeeID
                        where emp.Email == email
                        select au;

            return query.FirstOrDefault();
        }
        public List<string> GetRolesForUser(Guid appUserId)
        {
            return (from aur in dataContextCaller.AppUserRoles
                    join r in dataContextCaller.Roles
                        on aur.RoleID equals r.RoleID
                    where aur.AppUserID == appUserId
                    select r.RoleName).ToList();
        }
        public bool UserHasRole(Guid appUserId, string requiredRole)
        {
            return GetRolesForUser(appUserId)
                .Any(r => r.Equals(requiredRole, StringComparison.OrdinalIgnoreCase));
        }
    }
}

