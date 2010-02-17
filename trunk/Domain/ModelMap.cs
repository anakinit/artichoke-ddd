using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;

namespace Artichoke.Domain
{
    public abstract class ModelMap<TModel> : ClassMap<TModel> where TModel : IModelBase
    {
        public ModelMap()
        {
            var interfaces = typeof(TModel).GetInterfaces();

            if (interfaces.Contains(typeof(IWithVersioning)))
            {
                var property = typeof(TModel).GetProperty("Version");
                if (property == null)
                    throw new MissingFieldException("Version");
                else
                    Version(property).Column("Version");
            }

            if (interfaces.Contains(typeof(IWithAudit)))
            {
                var created = typeof(TModel).GetProperty("Created");
                if (created == null)
                    throw new MissingFieldException("Created");
                else
                    Map(created, "Created").Insert().Not.Update();

                var modified = typeof(TModel).GetProperty("Modified");
                if (modified == null)
                    throw new MissingFieldException("Modified");
                else
                    Map(modified, "Modified").Not.Update().Insert();
            }
        }
    }
}
