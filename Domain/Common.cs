using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Artichoke.Domain
{
    public static class Common
    {
        public static string GetString(string value)
        {
            return (value + "").Trim();
        }

        public static void SetValue(object container, string propertyName, object value, ref bool dirty)
        {
            bool isDirty = false;

            PropertyInfo propInfo = container.GetType().GetProperty(propertyName);
            if (propInfo == null)
                throw new ArgumentException("Invalid property name or property not found in container.");
            if (!propInfo.CanRead || !propInfo.CanWrite)
                throw new ArgumentException("Property must be able to both Read and Write.");

            if (propInfo.GetValue(container, null) != value)
            {
                propInfo.SetValue(container, value, null);
                isDirty = true;
            }
            dirty |= isDirty;
        }

        public static bool SetValue(ref object variable, object value)
        {
            if (variable != value)
            {
                variable = value;
                return true;
            }
            return false;
        }
    }
}
