using System;

namespace EasyDapper.Core
{
  public class MissingOrderByException : Exception
  {
    public MissingOrderByException()
      : base("The Order By is missing, SQL Server Using paging needs Order By.")
    {
    }
  }
}
