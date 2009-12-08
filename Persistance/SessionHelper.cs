using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using NHibernate;

namespace Artichoke.Persistance
{
    internal static class SessionHelper
    {
        private static readonly ISessionManager sessionManager;
        private static object locker = new Object();
        
        static SessionHelper()
        {
            var windsorContainer = new WindsorContainer(new XmlInterpreter());
            sessionManager = windsorContainer.Resolve<ISessionManager>();

            if (sessionManager == null)
                throw new ApplicationException("Session Manager not initialized.");
        }

        public static ISession GetSession(string dbKey)
        {
            if (!CurrentSessions.ContainsKey(dbKey) || CurrentSessions[dbKey] == null || !CurrentSessions[dbKey].IsOpen)
            {
                CurrentSessions.Add(dbKey, GetSessionFactory(dbKey).OpenSession());
            }
            return CurrentSessions[dbKey];
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

        private static ISessionFactory GetSessionFactory(string dbKey)
        {
            lock (locker)
            {
                if (!SessionFactories.ContainsKey(dbKey))
                {
                    var sessionFactory = sessionManager.BuildSessionFactory(dbKey);
                    SessionFactories.Add(dbKey, sessionFactory);
                }
                return SessionFactories[dbKey];
            }
        }



    }
}
