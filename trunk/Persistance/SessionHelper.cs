using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using NHibernate;
using System.Reflection;

namespace Artichoke.Persistence
{
    internal static class SessionHelper
    {
        private static readonly IWindsorContainer container;
        //private static readonly ISessionManager sessionManager;
        private static object locker = new Object();
        
        static SessionHelper()
        {
            container = new WindsorContainer(new XmlInterpreter());
        }
        
        private static string GetAssemblyName(Type type)
        {
            return type.Assembly.GetName().Name;
        }

        private static string GetKey(Type type, string dbKey)
        {
            return string.Concat(GetAssemblyName(type) + "#" + dbKey);
        }


        public static ISession GetSession(Type type, string dbKey)
        {
            var key = GetKey(type, dbKey);
            if (!CurrentSessions.ContainsKey(key) || CurrentSessions[key] == null || !CurrentSessions[key].IsOpen)
            {
                CurrentSessions.Add(key, GetSessionFactory(type, dbKey).OpenSession());
            }
            return CurrentSessions[key];
        }

        public static void CloseSession(string dbKey)
        {
            if (CurrentSessions.ContainsKey(dbKey))
                CurrentSessions[dbKey].Close();
        }
        
        private static IDictionary<string, ISession> CurrentSessions
        {
            get
            {
                IDictionary<string, ISession> sessions = ContextStore.GetData<IDictionary<string, ISession>>(CONSTANTS.SESSIONS_KEY);
                if (sessions == null)
                {
                    sessions = new Dictionary<string, ISession>();
                    ContextStore.SetData(CONSTANTS.SESSIONS_KEY, sessions);
                }
                return sessions;
            }
        }

        private static IDictionary<string, ISessionFactory> SessionFactories
        {
            get
            {
                IDictionary<string, ISessionFactory> sessionFactories = ContextStore.GetDomainData<IDictionary<string, ISessionFactory>>(CONSTANTS.SESSION_FACTORIES_KEY);
                if (sessionFactories == null)
                {
                    sessionFactories = new Dictionary<string, ISessionFactory>();
                    ContextStore.SetDomainData(CONSTANTS.SESSION_FACTORIES_KEY, sessionFactories);
                }
                return sessionFactories;
            }
        }

        private static ISessionFactory GetSessionFactory(Type type, string dbKey)
        {
            var key = GetKey(type, dbKey);
            lock (locker)
            {
                if (!SessionFactories.ContainsKey(key))
                {
                    ISessionManager sessionManager = null;
                    sessionManager = (ISessionManager)container[GetAssemblyName(type)];
                    if (sessionManager == null)
                        sessionManager = (ISessionManager)container[typeof(ISessionManager)];
                    if (sessionManager == null)
                        throw new InvalidProgramException("Crap is going to crap!");

                    var sessionFactory = sessionManager.BuildSessionFactory(dbKey);
                    SessionFactories.Add(key, sessionFactory);
                }
                return SessionFactories[key];
            }
        }



    }
}
