using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Langevin.Core.Utilities
{
    public static class Validators
    {
        public static bool ValidatePhoneNumber(string phoneNumber) {
            phoneNumber = Regex.Replace(phoneNumber, @"[^\d]", "");
            phoneNumber = phoneNumber.TrimStart('1');
            if (phoneNumber.Length != 10)
                return false;
            return true;
        }

        public static bool ValidateEmail(string email)
        {
            return Regex.IsMatch(email, @"[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}", RegexOptions.IgnoreCase);
        }
    }
}
