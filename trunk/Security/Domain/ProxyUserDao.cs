using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artichoke.Persistance;

namespace Transaktion.Core.Security.Domain
{
    internal class ProxyUserDao : Dao<ProxyUser>
    {

        public ProxyUser GetUser(string applicationName, string username)
        {
            return Query
                    .Where(x => x.ApplicationName.Equals(applicationName, StringComparison.CurrentCultureIgnoreCase))
                    .Where(x => x.IsApproved)
                    .FirstOrDefault(x => x.Username.Equals(username, StringComparison.CurrentCultureIgnoreCase));
        }

        public ProxyUser GetUser(Guid userKey)
        {
            return Query.FirstOrDefault(x => x.UserKey == userKey);
        }

        public string GetUsernameByEmail(string email)
        {
            var user = Query.FirstOrDefault(x => x.Email.Equals(email, StringComparison.CurrentCultureIgnoreCase));
            if (user != null)
                return user.Email;
            return "";
        }

        public IEnumerable<ProxyUser> SearchUsersByEmail(string applicationName, string email, int pageIndex, int pageSize, ref int records) 
        {
            records = Query
                        .Where(x => x.ApplicationName.Equals(applicationName, StringComparison.CurrentCultureIgnoreCase))
                        .Where(x => x.Email.StartsWith(email) || x.Email.EndsWith(email) || x.Email.Contains(email)).Count();
            return Query
                    .Where(x => x.ApplicationName.Equals(applicationName, StringComparison.CurrentCultureIgnoreCase))
                    .Where(x => x.Email.StartsWith(email) || x.Email.EndsWith(email) || x.Email.Contains(email))
                    .Skip(pageIndex * pageSize)
                    .Take(pageSize).AsEnumerable();

        }

        public IEnumerable<ProxyUser> SearchUsersByName(string applicationName, string name, int pageIndex, int pageSize, ref int records)
        {
            records = Query
                        .Where(x => x.ApplicationName.Equals(applicationName, StringComparison.CurrentCultureIgnoreCase))
                        .Where(x => x.Username.StartsWith(name) || x.Username.EndsWith(name) || x.Username.Contains(name)).Count();

            return Query
                    .Where(x => x.ApplicationName.Equals(applicationName, StringComparison.CurrentCultureIgnoreCase))
                    .Where(x => x.Username.StartsWith(name) || x.Username.EndsWith(name) || x.Username.Contains(name))
                    .Skip(pageIndex * pageSize)
                    .Take(pageSize).AsEnumerable();

        }

        public IEnumerable<ProxyUser> GetAllUsers(string applicationName, int pageIndex, int pageSize, ref int records)
        {
            records = Query
                    .Where(x => x.ApplicationName.Equals(applicationName, StringComparison.CurrentCultureIgnoreCase)).Count();

            return Query
                    .Where(x => x.ApplicationName.Equals(applicationName, StringComparison.CurrentCultureIgnoreCase))
                    .Skip(pageIndex * pageSize)
                    .Take(pageSize).AsEnumerable();
        }

        public IEnumerable<ProxyUser> GetUsersByUsernames(string applicationName, string[] usernames)
        {
            string[] match =    
                    (
                        from username in usernames
                        select username.ToUpper()
                    ).ToArray();


            return
                (
                    from user in Query.AsEnumerable()
                    where user.ApplicationName.Equals(applicationName, StringComparison.CurrentCultureIgnoreCase)
                    where match.Contains(user.Username.ToUpper())
                    select user
                ).AsEnumerable<ProxyUser>();
        }
    }
}
