using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Artichoke.Services.Validation
{
    public class TestValidationDictionary : Dictionary<string, string>, IValidationDictionary
    {
        public void AddError(string key, string errorMessage)
        {
            if (!this.ContainsKey(key))
                this.Add(key, errorMessage);
            else
                this[key] = errorMessage;

        }

        public bool IsValid
        {
            get { return this.Count == 0; }
        }
    }
}
