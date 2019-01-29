namespace EasyDapper.MsSqlServer.ConnectionProviders
{
  public class ConnectionStringConnectionProvider : MsSqlConnectionProvider
  {
    public ConnectionStringConnectionProvider(string connectionString)
    {
      this.ConnectionString = connectionString;
    }
  }
}
