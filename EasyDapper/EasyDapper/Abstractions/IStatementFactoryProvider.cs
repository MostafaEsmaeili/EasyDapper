namespace EasyDapper.Abstractions
{
    public interface IStatementFactoryProvider
    {
        IConnectionProvider GetConnectionProvider { get; }
        IStatementFactory Provide();
    }
}