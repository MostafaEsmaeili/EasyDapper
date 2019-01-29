using System.Data;
using System.Data.SqlClient;
using EasyDapper.Core.Abstractions;

namespace EasyDapper.MsSqlServer.Abstractions
{
  public class MsSqlParameterCollectionAdapter : ISqlParameterCollection
  {
    private readonly SqlParameterCollection _parameters;

    public MsSqlParameterCollectionAdapter(SqlParameterCollection parameters)
    {
      _parameters = parameters;
    }

    public void AddWithValue(
      string name,
      object value,
      bool isNullable,
      DbType dbType,
      int size = 0,
      ParameterDirection direction = ParameterDirection.Input)
    {
      var parameters = _parameters;
      var sqlParameter = new SqlParameter
      {
          ParameterName = name,
          DbType = dbType,
          IsNullable = isNullable,
          Size = size,
          Value = value,
          Direction = direction
      };
      parameters.Add(sqlParameter);
    }

    public IDataParameterCollection GetParameter()
    {
      return _parameters;
    }
  }
}
