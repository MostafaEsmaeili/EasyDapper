using System;

namespace EasyDapper.Core
{
  public abstract class OrderSpecificationBase
  {
    public string Alias { get; set; }

    public OrderByDirection Direction { get; set; }

    public Type EntityType { get; set; }

    public string Identifer { get; set; }

    public string TableName { get; set; }

    public string Schema { get; set; }
  }
}
