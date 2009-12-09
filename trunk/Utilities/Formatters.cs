using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Langevin.Core.Utilities
{
    public class Formatters
    {
        public static string FormatPhoneNumber(string phoneNumber)
        {
            phoneNumber = Regex.Replace(phoneNumber, @"[^\d]", "");
            phoneNumber = phoneNumber.TrimStart('1');
            return string.Format("({0}) {1}-{2}", phoneNumber.Substring(0, 3), phoneNumber.Substring(3, 3), phoneNumber.Substring(6, 4));
        }

    }
}
