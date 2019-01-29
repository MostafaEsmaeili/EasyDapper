namespace EasyDapper.Abstractions
{
  public interface IStatementFactoryProvider
  {
    IStatementFactory Provide();

    IConnectionProvider GetConnectionProvider { get; }
  }
}
