using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Cfg;

namespace Artichoke.Persistance
{
    public interface IDaoConfiguration
    {
        Configuration BuildConfiguration(string dbKey);
    }
}
