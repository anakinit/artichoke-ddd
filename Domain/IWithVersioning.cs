using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Artichoke.Domain
{
    public interface IWithVersioning : IEntityBase, IWithAudit
    {
        int Version
        {
            get; 
        }
    }
}
