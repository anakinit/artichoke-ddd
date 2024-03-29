﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artichoke.Persistence;
using Artichoke.Services.Validation;
using Artichoke.Services.Exceptions;

namespace Artichoke.Services
{
    public abstract class DaoService<TDao> : Service where TDao : IDao
    {
        private readonly TDao dao;
       
        protected DaoService(TDao dao, IValidation validation)
            : base(validation)
        {
            if (dao == null) throw new ArgumentNullException("dao");
            this.dao = dao;
        }

        public TDao Dao
        {
            get { return dao; }
        }
    }
}
