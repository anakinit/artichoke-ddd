using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Artichoke.Services.Validation
{
    public interface IValidation
    {
        void AddError(string key, string errorMessage);
        bool IsValid { get; }
    }
}
