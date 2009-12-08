using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Artichoke.Model
{
    public interface IWithAudit : IModelBase
    {
        DateTime Created
        {
            get;
        }

        DateTime Modified
        {
            get;
        }
    }
}
