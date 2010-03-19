using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;

namespace Artichoke.Persistence
{
    public interface ISessionManager
    {
        ISessionFactory BuildSessionFactory(string dbKey);
    }
}
