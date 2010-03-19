using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Linq;
using Artichoke.Domain;

namespace Artichoke.Persistence
{
    public abstract class Dao<TModel>
        : IDao where TModel : class
    {
        private string dbKey;
        private Type modelType;

        protected Dao()
            : this(CONSTANTS.DEFAULT_DB_KEY)
        { /* Do Nothing */ }

        protected Dao(string dbKey)
        {
            this.dbKey = dbKey;
            this.modelType = typeof(TModel);
        }

        public ISession CurrentSession
        {
            get { return WebApplication.GetCurrentSession(modelType, dbKey); }
        }

        protected IOrderedQueryable<TModel> Query
        {
            get { return CurrentSession.Linq<TModel>(); }
        }

        public virtual void Save(object item)
        {
            Save(item as TModel);
        }

        public virtual void Save(TModel item)
        {
            using (var transaction = CurrentSession.BeginTransaction())
            {
                CurrentSession.SaveOrUpdate(item);
                transaction.Commit();
            }
        }

        public virtual void Delete(object item)
        {
            Delete(item as TModel);
        }

        public virtual void Delete(TModel item)
        {
            using (var transaction = CurrentSession.BeginTransaction())
            {
                CurrentSession.Delete(item);
                transaction.Commit();
            }
        }
    }

    public interface IDao
    {
        void Save(object item);
        void Delete(object item);
    }

}
