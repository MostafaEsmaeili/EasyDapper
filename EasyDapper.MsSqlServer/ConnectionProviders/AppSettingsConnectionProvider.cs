// Decompiled with JetBrains decompiler
// Type: SqlRepoEx.MsSqlServer.ConnectionProviders.AppSettingsConnectionProvider
// Assembly: SqlRepoEx.MsSqlServer, Version=2.2.5.0, Culture=neutral, PublicKeyToken=null
// MVID: E8CD94CF-96EF-4129-BE7F-C7A630E6EE1D
// Assembly location: C:\Users\Royan Developer\.nuget\packages\sqlrepoex.mssqlserver\2.2.5\lib\netstandard2.0\SqlRepoEx.MsSqlServer.dll

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
            connectionString = this.configuration.GetConnectionString(this.connectionName);
        }
    }
}