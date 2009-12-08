using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Runtime.Remoting.Messaging;

namespace Artichoke.Persistance
{
    internal static class ContextStore
    {
        private static readonly bool inWebContext;

        static ContextStore()
        {
            inWebContext = (HttpContext.Current != null);
        }

        public static object GetData(string key)
        {
            if (inWebContext)
                return HttpContext.Current.Items[key];
            else
                return CallContext.GetData(key);
        }

        public static T GetData<T>(string key) where T : class
        {
            if (inWebContext)
                return HttpContext.Current.Items[key] as T;
            else
                return CallContext.GetData(key) as T;
        }

        public static void SetData(string key, object value)
        {
            if (inWebContext)
                HttpContext.Current.Items[key] = value;
            else
                CallContext.SetData(key, value);
        }

        public static void SetData<T>(string key, T value)
        {
            if (inWebContext)
                HttpContext.Current.Items[key] = value;
            else
                CallContext.SetData(key, value);
        }

        public static object GetDomainData(string key)
        {
            return AppDomain.CurrentDomain.GetData(key);
        }

        public static T GetDomainData<T>(string key) where T : class
        {
            return AppDomain.CurrentDomain.GetData(key) as T;
        }

        public static void SetDomainData(string key, object value)
        {
            AppDomain.CurrentDomain.SetData(key, value);
        }

        public static void SetDomainData<T>(string key, T value)
        {
            AppDomain.CurrentDomain.SetData(key, value);
        }
    }
}
