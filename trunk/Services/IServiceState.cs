using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artichoke.Services.Validation;

namespace Artichoke.Services
{
    public interface IServiceState
    {
        IValidation Validation { get; }
        IList<IServiceMessage> Messages { get; }

        void AddValidationError(string key, string message);
        void AddMessage(ServiceMessageLevel level, string message);

        bool IsValid();
        bool HasMessage();
    }

    public enum ServiceMessageLevel
    {
        Debug,
        Info,
        Warn,
        Error,
        Fatal
    }

    public interface IServiceMessage
    { 
        ServiceMessageLevel Level { get; }
        string Message { get; }
    }
}
