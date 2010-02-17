using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using NHibernate.Context;
using NHibernate;

namespace Artichoke.Persistance
{
    public class WebSessionModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            //context.BeginRequest += new EventHandler(Application_BeginRequest);
            context.EndRequest += new EventHandler(Application_EndRequest);
        }
        
        public void Dispose()
        {
        }

        private void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        private void Application_EndRequest(object sender, EventArgs e)
        {
            WebApplication.UnbindSessions();
        }

    }
}
