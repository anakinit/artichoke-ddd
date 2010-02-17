using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using NHibernate.Cfg;
using NHibernate;

namespace Artichoke.Persistance
{
    public static class CurrentApplication : HttpApplication
    {
        public static readonly Configuration Configuration;
        public static readonly ISessionFactory SessionFactory;

        static CurrentApplication()
        {
            log4net.Config.XmlConfigurator.Configure();
            Configuration = new Configuration();

            SessionFactory = Configuration.BuildSessionFactory();
        }

        public static ISession GetCurrentSession()
        {
            return SessionFactory.GetCurrentSession();
        }
    }
}
