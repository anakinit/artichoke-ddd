using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;

namespace Artichoke.Persistance
{
    public interface ISessionManager
    {
        ISessionFactory BuildSessionFactory(string dbKey);
    }
}
