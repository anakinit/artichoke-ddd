using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Linq;
using Artichoke.Persistance;

namespace Artichoke.Model
{
    public abstract class DaoBase<TModel>
        : IRepository where TModel : class
    {
        private ITransaction _transaction;

        public DaoBase()
            : this(CONSTANTS.DEFAULT_DB_KEY)
        { /* Do Nothing */ }

        public DaoBase(string dbKey)
            : this(SessionHelper.GetSession(typeof(TModel), dbKey)) { /* Do Nothing */ }

        //~Repository()
        //{
        //    var session = base.Session;
        //    if (session != null && session.IsOpen)
        //    {
        //        session.Flush();
        //        session.Close();
        //    }
        //}

        protected IOrderedQueryable<TModel> Query
        {
            get { return CurrentApplication.GetCurrentSession().Linq<TModel>(); }
        }

        protected IOrderedQueryable<TQueryModel> GetQuery<TQueryModel>()
        {
            return CurrentApplication.GetCurrentSession().Linq<TQueryModel>();
        }

        public virtual void Save(object item)
        {
            Save(item as TModel);
        }

        public virtual void Save(TModel item)
        {
            var session = CurrentApplication.GetCurrentSession();
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
            var session = CurrentApplication.GetCurrentSession();
            using (var transaction = session.BeginTransaction())
            {
                session.SaveOrUpdate(item);
                transaction.Commit();
            }
        }
    }

    public interface IRepository : IDisposable
    {
        void Save(object item);
        void Delete(object item);
    }

}
