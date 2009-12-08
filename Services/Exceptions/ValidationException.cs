using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Artichoke.Services.Exceptions
{
    public class ValidationException : Exception
    {
        private readonly string key;
        
        public ValidationException(string key, string errorMessage)
            : base(errorMessage)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            if (string.IsNullOrEmpty(errorMessage)) throw new ArgumentNullException("errorMessage");

            this.key = key;
        }

        public string Key
        {
            get { return key; }
        }

    }
}
