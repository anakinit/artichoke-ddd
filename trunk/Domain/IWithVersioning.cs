using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Artichoke.Domain
{
    public interface IWithVersioning : IModelBase, IWithAudit
    {
        int Version
        {
            get; 
        }
    }
}
