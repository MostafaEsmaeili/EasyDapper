using System.Configuration;

namespace EasyDapper.MsSqlServer.ConnectionProviders
{
    public class AppConfigFirstConnectionProvider : MsSqlConnectionProvider
    {
        public AppConfigFirstConnectionProvider()
        {
            ConnectionString = ConfigurationManager.ConnectionStrings[0].ConnectionString;
        }
    }
}
