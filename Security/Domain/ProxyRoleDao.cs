using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artichoke.Persistance;

namespace Transaktion.Core.Security.Domain
{
    internal class ProxyRoleDao : Dao<ProxyRole>
    {
        internal ProxyRole[] GetRolesByRoleNames(string applicationName, string[] roleNames)
        {
            var lookup = roleNames.Select(x => x.ToUpper()).ToArray();

            return Query
                .Where(x => x.ApplicationName.Equals(applicationName, StringComparison.CurrentCultureIgnoreCase))
                .ToArray();

            //.Where(x => lookup.Contains(x.RoleName.ToUpper()))
        }

        internal ProxyRole GetRoleByRoleName(string applicationName, string roleName)
        {
            return Query
                .Where(x => x.ApplicationName.Equals(applicationName, StringComparison.CurrentCultureIgnoreCase))
                .Where(x => x.RoleName.Equals(roleName, StringComparison.CurrentCultureIgnoreCase))
                .Select(x => x) as ProxyRole;
                    
        }
    }

    
}
