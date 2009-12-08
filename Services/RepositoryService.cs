using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artichoke.Model;
using Artichoke.Services.Validation;
using Artichoke.Services.Exceptions;

namespace Artichoke.Services
{
    public abstract class RepositoryService<TRepository> : Service where TRepository : IRepository
    {
        private readonly TRepository repository;
       
        public RepositoryService(TRepository repository, IValidationDictionary validation)
            : base(validation)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        ~RepositoryService()
        {
            repository.Dispose();
        }

        public TRepository Repository
        {
            get { return repository; }
        }
    }
}
