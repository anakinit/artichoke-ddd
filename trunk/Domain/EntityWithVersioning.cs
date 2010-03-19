using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Artichoke.Domain
{
    public abstract class EntityWithVersioning : EntityWithAudit, IWithVersioning
    {
        private int _version;

        public virtual int Version
        {
            get { return _version; }
            protected set { _version = value; }
        }
    }
}
