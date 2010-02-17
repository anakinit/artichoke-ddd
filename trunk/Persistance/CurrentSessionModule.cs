using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using NHibernate.Context;
using NHibernate;

namespace Artichoke.Persistance
{
    public class CurrentSessionModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.BeginRequest +=new EventHandler(Application_BeginRequest);
        }
        
        public void Dispose()
        {
        }

        private void Application_BeginRequest(object sender, EventArgs e)
        {
            ManagedWebSessionContext.Bind(HttpContext.Current, CurrentApplication.SessionFactory.OpenSession());
        }

        private void Application_EndRequest(object sender, EventArgs e)
        {
            ISession session = ManagedWebSessionContext.Unbind(HttpContext.Current, CurrentApplication.SessionFactory);

            if (session != null)
            {
                if (session.Transaction.IsActive)
                    session.Transaction.Rollback();

                session.Close();
            }
        }

    }
}
