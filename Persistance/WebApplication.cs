using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using NHibernate.Cfg;
using NHibernate;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using NHibernate.Context;


namespace Artichoke.Persistance
{
    public class WebApplication : HttpApplication
    {
        private static readonly IDictionary<string, ISessionFactory> sessionFactories;
        private static readonly IWindsorContainer container;
        private static readonly object lockObject;

        static WebApplication()
        {
            lockObject = new Object();
            container = new WindsorContainer(new XmlInterpreter());
            sessionFactories = new Dictionary<string, ISessionFactory>();
        }

        private static string GetKey(Type type, string dbKey)
        {
            return string.Concat(type.Assembly.GetName().Name, "#", dbKey);
        }


        private static ISessionFactory GetSessionFactory(Type type, string dbKey)
        {
            var key = GetKey(type, dbKey);

            lock (lockObject)
            {
                if (!sessionFactories.ContainsKey(key) || sessionFactories[key] == null)
                {
                    var assemblyName = type.Assembly.GetName().Name;
                    var daoConfiguration = container[assemblyName] as IDaoConfiguration;

                    if (daoConfiguration == null)
                        throw new InvalidProgramException(string.Format("Unable to load IDaoConfiguration for '{0}'", assemblyName));

                    var configuration = daoConfiguration.BuildConfiguration(dbKey);
                    configuration.Properties.Add("current_session_context_class", "managed_web");

                    var sessionFactory = configuration.BuildSessionFactory();

                    sessionFactories.Add(key, sessionFactory);
                }
            }

            return sessionFactories[key];
        }

        public static void UnbindSessions()
        {
            foreach (var key in sessionFactories.Keys)
            {
                var sessionFactory = sessionFactories[key];
                if (ManagedWebSessionContext.HasBind(HttpContext.Current, sessionFactory))
                {
                    var session = ManagedWebSessionContext.Unbind(HttpContext.Current, sessionFactory);
                    if (session != null)
                    {
                        if (session.Transaction.IsActive)
                            session.Transaction.Rollback();
                        else
                            session.Flush();
                        session.Close();
                    }
                }
            }
        }

        public static ISession GetCurrentSession(Type type, string dbKey)
        {
            var sessionfactory = GetSessionFactory(type, dbKey);
            if (!ManagedWebSessionContext.HasBind(HttpContext.Current, sessionfactory)) {
                ManagedWebSessionContext.Bind(HttpContext.Current, sessionfactory.OpenSession());
            }
            return sessionfactory.GetCurrentSession();
        }
    }
}
