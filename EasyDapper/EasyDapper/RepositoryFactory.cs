using EasyDapper.Abstractions;

namespace EasyDapper
{
    public class RepositoryFactory : IRepositoryFactory
    {
        private readonly IStatementFactoryProvider statementFactoryProvider;

        public RepositoryFactory(IStatementFactoryProvider statementFactoryProvider)
        {
            this.statementFactoryProvider = statementFactoryProvider;
        }

        public IRepository<TEntity> Create<TEntity>() where TEntity : class, new()
        {
            var repository = new Repository<TEntity>(statementFactoryProvider.Provide());
            if (repository.GetConnectionProvider == null)
                repository.UseConnectionProvider(statementFactoryProvider.GetConnectionProvider);
            return repository;
        }

        public IRepository Create()
        {
            var repository = new Repository(statementFactoryProvider.Provide());
            if (repository.GetConnectionProvider == null)
                repository.UseConnectionProvider(statementFactoryProvider.GetConnectionProvider);
            return repository;
        }
    }
}