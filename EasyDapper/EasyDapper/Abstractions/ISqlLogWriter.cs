namespace EasyDapper.Abstractions
{
  public interface ISqlLogWriter
  {
    void Log(string sql);
  }
}
