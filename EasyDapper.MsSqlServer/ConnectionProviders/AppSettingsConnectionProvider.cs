using Microsoft.Extensions.Configuration;

namespace EasyDapper.MsSqlServer.ConnectionProviders
{
  public class AppSettingsConnectionProvider : MsSqlConnectionProvider
  {
    private readonly IConfiguration configuration;
    private readonly string connectionName;

    public AppSettingsConnectionProvider(IConfiguration configuration, string connectionName)
    {
      this.configuration = configuration;
      this.connectionName = connectionName;
      ConnectionString = this.configuration.GetConnectionString(this.connectionName);
    }
  }
}
