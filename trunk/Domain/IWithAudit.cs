using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Artichoke.Domain
{
    public interface IWithAudit : IEntityBase
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
