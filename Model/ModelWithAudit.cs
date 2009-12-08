using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Artichoke.Model
{
    public abstract class ModelWithAudit : ModelBase, IWithAudit
    {
        private DateTime _created = DateTime.MinValue;
        private DateTime _modified = DateTime.MinValue;

        public virtual DateTime Created
        {
            get { return (_created == DateTime.MinValue ? DateTime.Now : _created); }
            protected set { _created = value; }
        }

        public virtual DateTime Modified
        {
            get { return (_modified == DateTime.MinValue ? DateTime.Now : _modified); }
            protected set { _modified = value; }
        }
    }
}
