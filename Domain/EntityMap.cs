using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using FluentNHibernate;

namespace Artichoke.Domain
{
    public abstract class EntityMap<TEntity> : ClassMap<TEntity> where TEntity : IEntityBase
    {
        private void Initialize() {
            var interfaces = typeof(TEntity).GetInterfaces();

            if (interfaces.Contains(typeof(IWithVersioning)))
            {
                var property = typeof(TEntity).GetProperty("Version");
                if (property == null)
                    throw new MissingFieldException("Version");
                else
                    Version(MemberExtensions.ToMember(property)).Column("Version");
            }

            if (interfaces.Contains(typeof(IWithAudit)))
            {
                var created = typeof(TEntity).GetProperty("Created");
                if (created == null)
                    throw new MissingFieldException("Created");
                else
                    Map(MemberExtensions.ToMember(created), "Created").Insert().Not.Update();

                var modified = typeof(TEntity).GetProperty("Modified");
                if (modified == null)
                    throw new MissingFieldException("Modified");
                else
                    Map(MemberExtensions.ToMember(modified), "Modified").Not.Update().Insert();
            }
        }



        protected EntityMap()
            : base()
        {
            Initialize();
        }
    }
}
