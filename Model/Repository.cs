using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Linq;
using Artichoke.Persistance;

namespace Artichoke.Model
{
    public abstract class Repository<TModel>
        : NHibernateContext, IRepository where TModel : class
    {
        private ITransaction _transaction;

        public Repository()
            : this(CONSTANTS.DEFAULT_DB_KEY)
        { /* Do Nothing */ }

        public Repository(string dbKey)
            : this(SessionHelper.GetSession(typeof(TModel), dbKey)) { /* Do Nothing */ }

        public Repository(ISession session)
            : base(session) { /* Do Nothing */}

        //~Repository()
        //{
        //    base.Session.Flush();
        //}

        protected IOrderedQueryable<TModel> Query
        {
            get { return base.Session.Linq<TModel>(); }
        }

        protected IOrderedQueryable<TQueryModel> GetQuery<TQueryModel>()
        {
            return base.Session.Linq<TQueryModel>();
        }

        protected override ISession ProvideSession()
        {
            return base.ProvideSession();
        }

        public virtual void SaveItem(object item)
        {
            SaveItem(item as TModel);
        }

        public virtual void SaveItem(TModel item)
        {
            base.Session.SaveOrUpdate(item);
            Session.Flush();
        }

        public virtual void DeleteItem(object item)
        {
            DeleteItem(item as TModel);
        }

        public virtual void DeleteItem(TModel item)
        {
            base.Session.Delete(item);
            Session.Flush();
        }

        public void BeginTransaction()
        {
            _transaction = base.Session.BeginTransaction();
        }

        public void CommitTransaction()
        {
            if (_transaction != null)
                _transaction.Commit();
        }

        public void RollbackTransaction()
        {
            if (_transaction != null)
                _transaction.Rollback();
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }

    public interface IRepository : IDisposable
    {
        void SaveItem(object item);
        void DeleteItem(object item);
        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();
    }

}
