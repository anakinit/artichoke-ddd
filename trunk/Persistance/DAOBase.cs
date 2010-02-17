using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Linq;
using Artichoke.Persistance;

namespace Artichoke.Model
{
    public abstract class BaseDAO<TModel>
        : IRepository where TModel : class
    {
        private ITransaction _transaction;

        public BaseDAO()
            : this(CONSTANTS.DEFAULT_DB_KEY)
        { /* Do Nothing */ }

        public BaseDAO(string dbKey)
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
            using (var tx = Session.BeginTransaction())
            {
                Session.SaveOrUpdate(item);
                tx.Commit();
            }
        }

        public virtual void DeleteItem(object item)
        {
            DeleteItem(item as TModel);
        }

        public virtual void DeleteItem(TModel item)
        {
            using (var tx = Session.BeginTransaction())
            {
                Session.Delete(item);
                tx.Commit();
            }
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
