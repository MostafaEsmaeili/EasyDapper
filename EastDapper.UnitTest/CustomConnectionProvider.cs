using EasyDapper.MsSqlServer.ConnectionProviders;
using Microsoft.Extensions.Configuration;

namespace EastDapper.UnitTest
{
    public class CustomConnectionProvider : MsSqlConnectionProvider
    {
        private readonly IConfiguration configuration;
        private readonly string connectionName;

        public CustomConnectionProvider( )
        {
            IConfiguration configuration;
            this.configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();;
            this.connectionName = "Test";
            this.connectionString = this.configuration.GetConnectionString(this.connectionName);
        }
    }
}