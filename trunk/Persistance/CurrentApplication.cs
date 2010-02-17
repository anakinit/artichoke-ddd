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
    public class CurrentApplication : HttpApplication
    {
        private static readonly IDictionary<string, ISessionFactory> sessionFactories;
        private static readonly IWindsorContainer container;
        private static readonly object lockObject;

        static CurrentApplication()
        {
            lockObject = new Object();
            container = new WindsorContainer(new XmlInterpreter());
            sessionFactories = new Dictionary<string, ISessionFactory>();
        }

        private static string GetKey(Type type, string dbKey)
        {
            return string.Concat(type.Assembly.FullName, "#", dbKey);
        }


        private static ISessionFactory GetSessionFactory(Type type, string dbKey)
        {
            var key = GetKey(type, dbKey);

            lock (lockObject)
            {
                if (!sessionFactories.ContainsKey(key) || sessionFactories[key] == null)
                {
                    var assemblyName = type.Assembly.FullName;
                    var configuration = container[assemblyName] as IDaoConfiguration;

                    if (configuration == null)
                        throw new InvalidProgramException(string.Format("Unable to load IDaoConfiguration for '{0}'", assemblyName));

                    var sessionFactory = configuration.BuildConfiguration(dbKey).BuildSessionFactory();

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
            var session = GetSessionFactory(type, dbKey).GetCurrentSession();
            ManagedWebSessionContext.Bind(HttpContext.Current, session);
            return session;
        }
    }
}
