using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using Transaktion.Core.Security.Domain;
using System.Text.RegularExpressions;
using System.Configuration.Provider;

namespace Transaktion.Core.Security
{
    public class TransaktionRoleProvider : RoleProvider
    {
        private static readonly Regex reRoleNameCheck = new Regex(@"[^\w-]", RegexOptions.IgnoreCase & RegexOptions.Compiled);

        private static readonly ProxyRoleDao roleDao = new ProxyRoleDao();
        private static readonly ProxyUserDao userDao = new ProxyUserDao();

        private string _applicationName;

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            if (string.IsNullOrEmpty(name))
                name = "TransaktionRoleProvider";

            base.Initialize(name, config);

            _applicationName = GetConfigValue(config["applicationName"], System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
        }

        private string GetConfigValue(string value, string defaultValue)
        {
            if (string.IsNullOrEmpty(value))
                return defaultValue;
            return value;
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            foreach (var username in usernames)
            {
                var user = userDao.GetUser(this.ApplicationName, username);
                if (user != null)
                {
                    foreach (var roleName in roleNames)
                    {
                        if (user.Roles.FirstOrDefault(x => x.RoleName.Equals(roleName, StringComparison.CurrentCultureIgnoreCase)) == null)
                        {
                            user.Roles.Add(roleDao.GetRoleByRoleName(ApplicationName, roleName));
                        }
                    }
                    userDao.Save(user);
                }
            }
        }

        public override string ApplicationName
        {
            get
            {
                return _applicationName;
            }
            set
            {
                // Do Nothing
            }
        }

        public override void CreateRole(string roleName)
        {
            if (reRoleNameCheck.IsMatch(roleName))
                throw new ArgumentException("RoleName must contain alphanumeric characters only.");

            if (RoleExists(roleName))
                throw new ProviderException("RoleName already exists.");

            var role = new ProxyRole(this.ApplicationName, Guid.NewGuid(), roleName);

            roleDao.Save(role);
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            var role = roleDao.GetRoleByRoleName(this.ApplicationName, roleName);

            return (
                    from u in role.Users
                    where u.Username.StartsWith(usernameToMatch, StringComparison.CurrentCultureIgnoreCase) ||
                            u.Username.EndsWith(usernameToMatch, StringComparison.CurrentCultureIgnoreCase) ||
                            u.Username.Contains(usernameToMatch)
                    select u.Username).ToArray();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetRolesForUser(string username)
        {
            var user = userDao.GetUser(this.ApplicationName, username);

            return (from r in user.Roles
                    select r.RoleName).ToArray();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            var role = roleDao.GetRoleByRoleName(this.ApplicationName, roleName);

            return (from u in role.Users
                    select u.Username).ToArray();

        }

        public override bool IsUserInRole(string username, string roleName)
        {
            var roles = GetRolesForUser(username);

            return (roles.FirstOrDefault(x => x.Equals(roleName, StringComparison.CurrentCultureIgnoreCase)) != null);
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            var users = userDao.GetUsersByUsernames(this.ApplicationName, usernames);
            var roles = roleDao.GetRolesByRoleNames(this.ApplicationName, roleNames);

            foreach (var user in users)
            {
                foreach (var role in roles)
                {
                    if (user.Roles.Contains(role))
                        user.Roles.Remove(role);
                }

                userDao.Save(user);
            }
        }

        public override bool RoleExists(string roleName)
        {
            return roleDao.GetRoleByRoleName(this.ApplicationName, roleName) != null;
        }
    }
}
