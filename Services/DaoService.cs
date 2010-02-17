using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artichoke.Persistance;
using Artichoke.Services.Validation;
using Artichoke.Services.Exceptions;

namespace Artichoke.Services
{
    public abstract class DaoService<TDao> : Service where TDao : IDaoBase
    {
        private readonly TDao dao;
       
        public DaoService(TDao dao, IValidationDictionary validation)
            : base(validation)
        {
            if (dao == null) throw new ArgumentNullException("repository");
            this.dao = dao;
        }

        public TDao Dao
        {
            get { return dao; }
        }
    }
}
