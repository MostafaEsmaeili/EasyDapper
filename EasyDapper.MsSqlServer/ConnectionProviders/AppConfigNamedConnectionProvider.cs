using System.Configuration;

namespace EasyDapper.MsSqlServer.ConnectionProviders
{
    public class AppConfigNamedConnectionProvider : MsSqlConnectionProvider
    {
        private readonly string connectionName;

        public AppConfigNamedConnectionProvider(string connectionName)
        {
            this.connectionName = connectionName;
            ConnectionString = ConfigurationManager.ConnectionStrings[this.connectionName]
                .ConnectionString;
        }
    }
}
