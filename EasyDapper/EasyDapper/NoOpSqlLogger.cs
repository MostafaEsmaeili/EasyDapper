using EasyDapper.Abstractions;

namespace EasyDapper
{
  public class NoOpSqlLogger : ISqlLogWriter
  {
    public void Log(string sql)
    {
    }
  }
}
