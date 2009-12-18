using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artichoke.Services.Validation;
using Artichoke.Services.Exceptions;

namespace Artichoke.Services
{

    public abstract class Service
    {
        private readonly IValidationDictionary validation;
        private bool _isValid = true;

        public Service(IValidationDictionary validation)
        {
            this.validation = validation;
        }

        protected virtual void AddError(string key, string errorMessage) 
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            if (string.IsNullOrEmpty(errorMessage)) throw new ArgumentNullException("errorMessage");

            if (validation == null)
                throw new ValidationException(key, errorMessage);
            else
            {
                validation.AddError(key, errorMessage);
                _isValid = false;
            }
        }

        public IValidationDictionary Validation
        {
            get { return validation; }
        }

        public bool HasValidationDictionary
        {
            get { return validation != null; }
        }

        public bool IsValid
        {
            get
            {
                if (validation == null)
                    return _isValid;
                else
                    return validation.IsValid;
            }
        }
    }
}
