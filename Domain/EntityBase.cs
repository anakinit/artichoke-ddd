using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Artichoke.Domain
{
    public abstract class EntityBase : IEntityBase
    {
        public EntityBase()
        {
        }

        public abstract override string ToString();

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (this.GetType() == obj.GetType())
                return obj.GetHashCode() == this.GetHashCode();
            return false;
        }
    }
}
