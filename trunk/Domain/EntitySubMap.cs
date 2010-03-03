using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;

namespace Artichoke.Domain
{
    public abstract class EntitySubMap<TEntity> : SubclassMap<TEntity> where TEntity : IEntityBase
    {
    }
}
