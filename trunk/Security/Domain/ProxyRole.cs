using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artichoke.Domain;

namespace Transaktion.Core.Security.Domain
{
    public class ProxyRole : EntityBase
    {
        private Guid _roleKey;
        private string _applicationName;
        private string _roleName;
        private IList<ProxyUser> _users;

        protected ProxyRole() { }

        internal ProxyRole(string applicationName, Guid roleKey, string roleName)
        {
            _roleKey = roleKey;
            _applicationName = applicationName;
            _roleName = roleName.ToUpper();
            _users = new List<ProxyUser>();
        }

        public virtual Guid RoleKey
        {
            get { return _roleKey; }
            protected set { _roleKey = value; }
        }

        public virtual string ApplicationName
        {
            get { return _applicationName; }
            protected set { _applicationName = value; }
        }

        public virtual string RoleName
        {
            get { return _roleName; }
            protected set { _roleName = value; }
        }

        public virtual IList<ProxyUser> Users
        {
            get { return _users; }
            protected set { _users = value; }
        }

        public override string ToString()
        {
            return string.Format("ProxyRole#{0}-{1}", ApplicationName.ToLower(), RoleName.ToLower());
        }
    }

    public class ProxyRoleMap : EntityMap<ProxyRole>
    {
        public ProxyRoleMap()
        {
            Table("security_role");
            Id(x => x.RoleKey, "RoleKey")
                .GeneratedBy.Assigned();
            Map(x => x.ApplicationName);
            Map(x => x.RoleName);
            //HasManyToMany<ProxyUser>(x => x.Users)
            //    .ParentKeyColumn("RoleKey")
            //    .ChildKeyColumn("UserKey")
            //    .Table("security_user_roles")
            //    .Cascade.None();
        }
    }
}
