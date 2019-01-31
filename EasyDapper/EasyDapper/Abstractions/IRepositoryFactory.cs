namespace EasyDapper.Abstractions
{
    public interface IRepositoryFactory
    {
        IRepository<TEntity> Create<TEntity>() where TEntity : class, new();

        IRepository Create();
    }
}