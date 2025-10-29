using System.Collections.Generic;
using OptiTrack.Business.PasswordHasher;
using OptiTrack.Data.DBConnector;
using OptiTrack.Data.DBMLs;

namespace OptiTrack.Business.Services.Auth
{
    public class LoginAuth
    {
        private readonly TableModelsConnector _connector;

        public LoginAuth()
        {
            _connector = new TableModelsConnector();
        }

        public AppUser Authenticate(string email, string password, out List<string> roles)
        {
            roles = new List<string>();

            var user = _connector.GetUserByEmail(email);
            if (user == null) return null;

            bool valid = SecurePasswordHasher.VerifyPassword(password, user.PasswordHash, user.PasswordSalt);
            if (!valid) return null;

            roles = _connector.GetRolesForUser(user.AppUserID);
            return user;
        }
    }
}