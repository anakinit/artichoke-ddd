using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Linq;
using Artichoke.Domain;

namespace Artichoke.Persistance
{
    public abstract class DaoBase<TModel>
        : IDaoBase where TModel : class
    {
        private string dbKey;
        private Type modelType;

        public DaoBase()
            : this(CONSTANTS.DEFAULT_DB_KEY)
        { /* Do Nothing */ }

        public DaoBase(string dbKey)
        {
            this.dbKey = dbKey;
            this.modelType = typeof(TModel);
        }

        public ISession GetCurrentSession()
        {
            return WebApplication.GetCurrentSession(modelType, dbKey);
        }

        protected IOrderedQueryable<TModel> Query
        {
            get { return GetCurrentSession().Linq<TModel>(); }
        }

        protected IOrderedQueryable<TQueryModel> GetQuery<TQueryModel>()
        {
            return GetCurrentSession().Linq<TQueryModel>();
        }

        public virtual void Save(object item)
        {
            Save(item as TModel);
        }

        public virtual void Save(TModel item)
        {
            var session = GetCurrentSession();
            using (var transaction = session.BeginTransaction())
            {
                session.SaveOrUpdate(item);
                transaction.Commit();
            }
        }

        public virtual void Delete(object item)
        {
            Delete(item as TModel);
        }

        public virtual void Delete(TModel item)
        {
            var session = GetCurrentSession();
            using (var transaction = session.BeginTransaction())
            {
                session.SaveOrUpdate(item);
                transaction.Commit();
            }
        }
    }

    public interface IDaoBase
    {
        void Save(object item);
        void Delete(object item);
    }

}
