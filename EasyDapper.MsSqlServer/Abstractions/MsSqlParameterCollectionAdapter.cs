// Decompiled with JetBrains decompiler
// Type: SqlRepoEx.MsSqlServer.Abstractions.MsSqlParameterCollectionAdapter
// Assembly: SqlRepoEx.MsSqlServer, Version=2.2.5.0, Culture=neutral, PublicKeyToken=null
// MVID: E8CD94CF-96EF-4129-BE7F-C7A630E6EE1D
// Assembly location: C:\Users\Royan Developer\.nuget\packages\sqlrepoex.mssqlserver\2.2.5\lib\netstandard2.0\SqlRepoEx.MsSqlServer.dll

using System.Data;
using System.Data.SqlClient;
using EasyDapper.Core.Abstractions;

namespace EasyDapper.MsSqlServer.Abstractions
{
    public class MsSqlParameterCollectionAdapter : ISqlParameterCollection
    {
        private readonly SqlParameterCollection parameters;

        public MsSqlParameterCollectionAdapter(SqlParameterCollection parameters)
        {
            this.parameters = parameters;
        }

        public void AddWithValue(
            string name,
            object value,
            bool isnullable,
            DbType dbType,
            int size = 0,
            ParameterDirection direction = ParameterDirection.Input)
        {
            var parameters = this.parameters;
            var sqlParameter = new SqlParameter();
            sqlParameter.ParameterName = name;
            sqlParameter.DbType = dbType;
            sqlParameter.IsNullable = isnullable;
            sqlParameter.Size = size;
            sqlParameter.Value = value;
            sqlParameter.Direction = direction;
            parameters.Add(sqlParameter);
        }

        public IDataParameterCollection GetParameter()
        {
            return parameters;
        }
    }
}